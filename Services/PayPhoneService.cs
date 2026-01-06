using AppBoleteriaApi.Data;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace AppBoleteriaApi.Services
{
    public class PayPhoneService : IPayPhoneService
    {
        private readonly AppDbContext _context;
        private readonly ITicketService _ticketService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PayPhoneService> _logger;
        private readonly string _baseUrl = "https://pay.payphonetodoesposible.com/api";

        public PayPhoneService(
            AppDbContext context,
            ITicketService ticketService,
            HttpClient httpClient,
            ILogger<PayPhoneService> logger)
        {
            _context = context;
            _ticketService = ticketService;
            _httpClient = httpClient;
            _logger = logger;
        }

        #region Iniciar Pago (Preparar datos para la Cajita)

        public async Task<InitiatePaymentResponse> InitiatePaymentAsync(int userId, InitiatePaymentDto dto)
        {
            _logger.LogInformation($"🎫 Preparando datos para Cajita - Usuario: {userId}, TicketType: {dto.TicketTypeId}, Cantidad: {dto.Quantity}");

            // 1. Obtener usuario que compra
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            // 2. Obtener ticket type con compañía y evento
            var ticketType = await _context.TicketTypes
                .Include(tt => tt.Company)
                .Include(tt => tt.Event)
                .FirstOrDefaultAsync(tt => tt.Id == dto.TicketTypeId && tt.IsActive);

            if (ticketType == null)
                throw new Exception("Tipo de ticket no encontrado");

            if (ticketType.Stock < dto.Quantity)
                throw new Exception($"Stock insuficiente. Solo quedan {ticketType.Stock} tickets disponibles.");

            var company = ticketType.Company!;
            var eventInfo = ticketType.Event!;

            // 3. Validar configuración de PayPhone
            if (!company.PayPhoneEnabled)
                throw new Exception("Lo sentimos, los pagos en línea no están disponibles en este momento. Por favor contacta al organizador.");

            if (string.IsNullOrEmpty(company.PayPhoneToken))
                throw new Exception("Configuración de pagos incompleta. Por favor contacta al organizador.");

            if (string.IsNullOrEmpty(company.PayPhoneStoreId))
                throw new Exception("Configuración de pagos incompleta. Por favor contacta al organizador.");

            // 4. Calcular montos (en CENTAVOS)
            var unitPriceInCents = (int)(ticketType.Price * 100);
            var totalAmountInCents = unitPriceInCents * dto.Quantity;
            var reference = $"BOL-{company.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}";

            _logger.LogInformation($"💰 Monto: ${ticketType.Price} x {dto.Quantity} = ${ticketType.Price * dto.Quantity} (En centavos: {totalAmountInCents})");

            // 5. Crear transacción pendiente en BD
            var transaction = new Transaction
            {
                CompanyId = company.Id,
                UserId = userId,
                Amount = ticketType.Price * dto.Quantity,
                PaymentMethod = "PayPhone",
                Status = "Pending",
                Reference = reference,
                TicketTypeId = dto.TicketTypeId,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ Transacción creada - ID: {transaction.Id}, Referencia: {reference}");

            // 6. ✅ RETORNAR DATOS PARA LA CAJITA (el pago se completa en el frontend)
            return new InitiatePaymentResponse
            {
                Success = true,
                Message = "Datos preparados para iniciar pago con la Cajita",
                TransactionId = transaction.Id.ToString(),
                Reference = reference,
                TotalAmount = ticketType.Price * dto.Quantity,
                EventTitle = eventInfo.Title,
                Quantity = dto.Quantity,

                // 🆕 Datos necesarios para inicializar la Cajita en el frontend
                Token = company.PayPhoneToken!,
                StoreId = company.PayPhoneStoreId!,
                AmountInCents = totalAmountInCents,
                PhoneNumber = $"{user.CountryCode ?? "593"}{user.Phone}",
                Email = user.Email,
                Currency = company.PayPhoneCurrency ?? "USD",
                TimeZone = company.PayPhoneTimeZone ?? -5
            };
        }

        #endregion

        #region Confirmar Pago desde la Cajita (API Button/V2/Confirm)

        /// <summary>
        /// Confirma el pago después de que el usuario complete el proceso en la Cajita
        /// Este método se llama desde la URL de respuesta configurada en PayPhone
        /// </summary>
        public async Task<CajitaConfirmResponse> ConfirmPaymentFromCajitaAsync(ConfirmPaymentFromCajitaDto dto)
        {
            _logger.LogInformation($"🔍 Confirmando pago desde Cajita - PayPhone ID: {dto.Id}, ClientTxId: {dto.ClientTxId}");

            // 1. Buscar transacción por Reference
            var transaction = await _context.Transactions
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t => t.Reference == dto.ClientTxId);

            if (transaction == null)
                throw new Exception("Transacción no encontrada");

            if (transaction.Company == null || string.IsNullOrEmpty(transaction.Company.PayPhoneToken))
                throw new Exception("Configuración de pago no encontrada");

            try
            {
                // 2. Llamar a la API Button/V2/Confirm de PayPhone
                var confirmRequest = new
                {
                    id = dto.Id,
                    clientTxId = dto.ClientTxId
                };

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/button/V2/Confirm");
                request.Headers.Add("Authorization", $"Bearer {transaction.Company.PayPhoneToken}");
                request.Content = new StringContent(
                    JsonSerializer.Serialize(confirmRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📥 Respuesta PayPhone Confirm: {(int)response.StatusCode} - {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"❌ Error al confirmar en PayPhone: {responseContent}");
                    throw new Exception("No se pudo confirmar el pago con PayPhone");
                }

                var confirmResponse = JsonSerializer.Deserialize<CajitaConfirmResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (confirmResponse == null)
                    throw new Exception("Respuesta inválida de PayPhone");

                // 3. Procesar según el estado
                if (confirmResponse.StatusCode == 3) // APROBADO
                {
                    _logger.LogInformation($"✅ Pago APROBADO - Generando tickets...");

                    // Actualizar transacción
                    transaction.Status = "Approved";
                    transaction.PayPhoneTransactionId = confirmResponse.TransactionId.ToString();
                    transaction.UpdatedAt = DateTime.UtcNow;

                    // Generar tickets
                    var purchaseDto = new TicketPurchaseDto
                    {
                        TicketTypeId = transaction.TicketTypeId,
                        Quantity = transaction.Quantity
                    };

                    var tickets = await _ticketService.PurchaseTicketsAsync(
                        transaction.UserId ?? 0,
                        purchaseDto
                    );

                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"🎫 {tickets.Count} ticket(s) generados exitosamente");
                }
                else if (confirmResponse.StatusCode == 2) // CANCELADO
                {
                    _logger.LogWarning($"❌ Pago CANCELADO");

                    transaction.Status = "Cancelled";
                    transaction.PayPhoneTransactionId = confirmResponse.TransactionId.ToString();
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return confirmResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en ConfirmPaymentFromCajitaAsync: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Consultar Estado (mantener para verificaciones)

        public async Task<PayPhoneStatusResponse> CheckPaymentStatusAsync(string transactionId)
        {
            _logger.LogInformation($"🔍 Consultando estado - TransactionId: {transactionId}");

            // Buscar transacción
            var transaction = await _context.Transactions
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t =>
                    t.Id.ToString() == transactionId ||
                    t.Reference == transactionId ||
                    t.PayPhoneTransactionId == transactionId);

            if (transaction == null)
                throw new Exception("Transacción no encontrada");

            if (transaction.Company == null || string.IsNullOrEmpty(transaction.Company.PayPhoneToken))
                throw new Exception("Configuración de pago no encontrada");

            try
            {
                // Consultar estado en PayPhone
                string url;
                if (!string.IsNullOrEmpty(transaction.PayPhoneTransactionId))
                {
                    url = $"{_baseUrl}/Sale/{transaction.PayPhoneTransactionId}";
                }
                else
                {
                    url = $"{_baseUrl}/Sale/client/{transaction.Reference}";
                }

                _logger.LogInformation($"📡 Consultando: {url}");

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {transaction.Company.PayPhoneToken}");

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"❌ Error al consultar estado: {responseContent}");
                    throw new Exception("No se pudo verificar el estado del pago. Por favor intenta nuevamente.");
                }

                var statusResponse = JsonSerializer.Deserialize<PayPhoneStatusResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (statusResponse == null)
                    throw new Exception("No se pudo obtener el estado del pago");

                _logger.LogInformation($"📊 Estado PayPhone: {statusResponse.StatusCode} ({statusResponse.TransactionStatus})");

                return statusResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en CheckPaymentStatusAsync: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Confirmar Pago (DEPRECADO - usar ConfirmPaymentFromCajitaAsync)

        [Obsolete("Usar ConfirmPaymentFromCajitaAsync para la Cajita de Pagos")]
        public async Task<bool> ConfirmPaymentAsync(string transactionId)
        {
            _logger.LogInformation($"⚠️ DEPRECADO: ConfirmPaymentAsync llamado - Transacción: {transactionId}");

            // Mantener por compatibilidad, pero ya no se usa con la Cajita
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t =>
                    t.Id.ToString() == transactionId ||
                    t.Reference == transactionId);

            if (transaction == null)
                throw new Exception("Transacción no encontrada");

            if (transaction.Status == "Approved")
            {
                _logger.LogInformation($"✅ Transacción ya aprobada: {transaction.Reference}");
                return true;
            }

            return false;
        }

        #endregion

        #region Obtener Estado de Transacción

        public async Task<TransactionStatusDto?> GetTransactionStatusAsync(string transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t =>
                    t.Id.ToString() == transactionId ||
                    t.Reference == transactionId);

            if (transaction == null)
                return null;

            // Obtener info del evento
            var ticketType = await _context.TicketTypes
                .Include(tt => tt.Event)
                .FirstOrDefaultAsync(tt => tt.Id == transaction.TicketTypeId);

            // Crear DTO
            return new TransactionStatusDto
            {
                Id = transaction.Id,
                Reference = transaction.Reference ?? "",
                Status = transaction.Status ?? "Unknown",
                Amount = transaction.Amount ?? 0,
                EventTitle = ticketType?.Event?.Title ?? "",
                Quantity = transaction.Quantity,
                CreatedAt = transaction.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = transaction.UpdatedAt,
                PayPhoneTransactionId = transaction.PayPhoneTransactionId
            };
        }

        #endregion

        #region Verificar Transacciones Pendientes

        public async Task VerifyAndUpdatePendingTransactionsAsync(int userId)
        {
            _logger.LogInformation($"🔍 Verificando transacciones pendientes para usuario: {userId}");

            var pendingTransactions = await _context.Transactions
                .Include(t => t.Company)
                .Where(t =>
                    t.UserId == userId &&
                    t.Status == "Pending" &&
                    t.IsActive &&
                    t.CreatedAt >= DateTime.UtcNow.AddDays(-1))
                .ToListAsync();

            if (!pendingTransactions.Any())
            {
                _logger.LogInformation($"✅ No hay transacciones pendientes para verificar");
                return;
            }

            _logger.LogInformation($"📋 Encontradas {pendingTransactions.Count} transacciones pendientes para verificar");

            foreach (var transaction in pendingTransactions)
            {
                try
                {
                    // Solo verificar si tiene PayPhoneTransactionId
                    if (string.IsNullOrEmpty(transaction.PayPhoneTransactionId))
                    {
                        _logger.LogDebug($"⏭️ Transacción {transaction.Reference} sin PayPhoneTransactionId (aún no completada en Cajita)");
                        continue;
                    }

                    _logger.LogInformation($"🔄 Verificando transacción: {transaction.Reference}");

                    var status = await CheckPaymentStatusAsync(transaction.Id.ToString());

                    if (status == null)
                    {
                        _logger.LogWarning($"⚠️ No se pudo obtener estado para transacción: {transaction.Reference}");
                        continue;
                    }

                    if (status.StatusCode == 3 && transaction.Status != "Approved")
                    {
                        _logger.LogInformation($"✅ Transacción APROBADA: {transaction.Reference}");

                        try
                        {
                            var purchaseDto = new TicketPurchaseDto
                            {
                                TicketTypeId = transaction.TicketTypeId,
                                Quantity = transaction.Quantity
                            };

                            var tickets = await _ticketService.PurchaseTicketsAsync(
                                transaction.UserId ?? 0,
                                purchaseDto
                            );

                            transaction.Status = "Approved";
                            transaction.UpdatedAt = DateTime.UtcNow;

                            _logger.LogInformation($"🎫 {tickets.Count} ticket(s) generados para transacción: {transaction.Reference}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"❌ Error al generar tickets para transacción {transaction.Reference}: {ex.Message}");
                        }
                    }
                    else if (status.StatusCode == 2 && transaction.Status != "Rejected")
                    {
                        _logger.LogWarning($"❌ Transacción RECHAZADA: {transaction.Reference}");
                        transaction.Status = "Rejected";
                        transaction.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (status.StatusCode == 4)
                    {
                        _logger.LogInformation($"🚫 Transacción CANCELADA: {transaction.Reference}");
                        transaction.Status = "Cancelled";
                        transaction.UpdatedAt = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error al verificar transacción {transaction.Reference}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"✅ Verificación de transacciones completada");
        }

        #endregion
    }
}