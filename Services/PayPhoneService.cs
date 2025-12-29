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

        #region Iniciar Pago

        public async Task<InitiatePaymentResponse> InitiatePaymentAsync(int userId, InitiatePaymentDto dto)
        {
            _logger.LogInformation($"🎫 Iniciando pago - Usuario: {userId}, TicketType: {dto.TicketTypeId}, Cantidad: {dto.Quantity}");

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
                throw new Exception($"Lo sentimos, los pagos en línea no están disponibles en este momento. Por favor contacta al organizador.");

            if (string.IsNullOrEmpty(company.PayPhoneToken))
                throw new Exception($"Configuración de pagos incompleta. Por favor contacta al organizador.");

            if (string.IsNullOrEmpty(company.PayPhoneStoreId))
                throw new Exception($"Configuración de pagos incompleta. Por favor contacta al organizador.");

            // 4. Calcular montos (en CENTAVOS según documentación)
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

            try
            {
                // 6. Preparar request para PayPhone API Sale
                var payPhoneRequest = new PayPhoneSaleRequest
                {
                    PhoneNumber = user.Phone,
                    CountryCode = user.CountryCode ?? "593",
                    Amount = totalAmountInCents,
                    AmountWithoutTax = totalAmountInCents,
                    AmountWithTax = 0,
                    Tax = 0,
                    ClientTransactionId = reference,
                    Reference = $"{dto.Quantity} ticket(s) - {eventInfo.Title}",
                    StoreId = company.PayPhoneStoreId!,
                    Currency = company.PayPhoneCurrency ?? "USD",
                    TimeZone = company.PayPhoneTimeZone ?? -5
                };

                _logger.LogInformation($"📡 Enviando a PayPhone API Sale...");

                // 7. Llamar a PayPhone API Sale
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/Sale");
                request.Headers.Add("Authorization", $"Bearer {company.PayPhoneToken}");
                request.Content = new StringContent(
                    JsonSerializer.Serialize(payPhoneRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📥 Respuesta PayPhone: {(int)response.StatusCode} - {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"❌ Error PayPhone: {responseContent}");

                    // Marcar transacción como fallida
                    transaction.Status = "Failed";
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // ⚠️ Procesar el error para dar un mensaje amigable
                    var friendlyMessage = ProcessPayPhoneError(responseContent);
                    throw new Exception(friendlyMessage);
                }

                var payPhoneResponse = JsonSerializer.Deserialize<PayPhoneSaleResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (payPhoneResponse == null || payPhoneResponse.TransactionId == 0)
                {
                    transaction.Status = "Failed";
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    throw new Exception("No se pudo procesar tu solicitud de pago. Por favor intenta nuevamente.");
                }

                // 8. Actualizar transacción con datos de PayPhone
                transaction.PayPhoneTransactionId = payPhoneResponse.TransactionId.ToString();
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Pago iniciado - PayPhone TransactionId: {payPhoneResponse.TransactionId}");

                // ✅ RETORNAR ÉXITO CON TODOS LOS DATOS
                return new InitiatePaymentResponse
                {
                    Success = true,
                    Message = "Pago iniciado correctamente",
                    TransactionId = transaction.Id.ToString(),
                    Reference = reference,
                    TotalAmount = ticketType.Price * dto.Quantity,
                    EventTitle = eventInfo.Title,
                    Quantity = dto.Quantity,
                    PayPhoneTransactionId = payPhoneResponse.TransactionId.ToString(),
                    PaymentUrl = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en InitiatePaymentAsync: {ex.Message}");

                // Marcar transacción como fallida
                transaction.Status = "Failed";
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // ⚠️ IMPORTANTE: LANZAR LA EXCEPCIÓN para que llegue al controller
                throw;
            }
        }

        #endregion

        #region Consultar Estado

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
                    throw new Exception($"No se pudo verificar el estado del pago. Por favor intenta nuevamente.");
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

        #region Confirmar Pago

        public async Task<bool> ConfirmPaymentAsync(string transactionId)
        {
            _logger.LogInformation($"✅ Confirmando pago - Transacción: {transactionId}");

            // 1. Buscar transacción
            var transaction = await _context.Transactions
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t =>
                    t.Id.ToString() == transactionId ||
                    t.Reference == transactionId);

            if (transaction == null)
                throw new Exception("Transacción no encontrada");

            // 2. Si ya está aprobada, retornar true
            if (transaction.Status == "Approved")
            {
                _logger.LogInformation($"✅ Transacción ya aprobada: {transaction.Reference}");
                return true;
            }

            // 3. Consultar estado en PayPhone
            var status = await CheckPaymentStatusAsync(transactionId);

            // 4. Procesar según el estado
            if (status.StatusCode == 3) // APROBADO
            {
                _logger.LogInformation($"💳 Pago APROBADO - Generando tickets...");

                try
                {
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

                    // Actualizar transacción
                    transaction.Status = "Approved";
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"🎫 {tickets.Count} ticket(s) generados exitosamente");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error al generar tickets: {ex.Message}");
                    throw new Exception($"Tu pago fue aprobado pero hubo un problema al generar los tickets. Por favor contacta a soporte con tu referencia: {transaction.Reference}");
                }
            }
            else if (status.StatusCode == 2) // RECHAZADO
            {
                _logger.LogWarning($"❌ Pago RECHAZADO");

                transaction.Status = "Rejected";
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return false;
            }
            else // PENDIENTE (StatusCode == 1)
            {
                _logger.LogInformation($"⏳ Pago aún PENDIENTE");
                return false;
            }
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

        #region Helpers Privados

        /// <summary>
        /// Procesa los errores de PayPhone y retorna mensajes amigables para el usuario
        /// </summary>
        private string ProcessPayPhoneError(string errorResponse)
        {
            try
            {
                var errorObj = JsonSerializer.Deserialize<PayPhoneErrorResponse>(
                    errorResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (errorObj != null && !string.IsNullOrEmpty(errorObj.Message))
                {
                    var errorCode = errorObj.ErrorCode ?? 0;

                    return errorCode switch
                    {
                        120 => "🚫 Tu número de teléfono no está registrado en PayPhone.\n\n" +
                               "📱 Para completar tu compra:\n" +
                               "1. Descarga la app PayPhone (Play Store/App Store)\n" +
                               "2. Regístrate con tu número de teléfono\n" +
                               "3. Vuelve aquí y completa tu compra\n\n" +
                               "¿Necesitas ayuda? Contacta a soporte.",

                        121 => "⚠️ Tu cuenta de PayPhone está inactiva.\n\n" +
                               "Por favor:\n" +
                               "1. Abre la app PayPhone\n" +
                               "2. Activa tu cuenta siguiendo las instrucciones\n" +
                               "3. Vuelve a intentar tu compra\n\n" +
                               "Si necesitas ayuda, contacta al soporte de PayPhone.",

                        122 => "📞 El número de teléfono no tiene el formato correcto.\n\n" +
                               "Por favor:\n" +
                               "1. Ve a tu perfil\n" +
                               "2. Verifica que tu número esté en formato correcto (ej: 0987654321)\n" +
                               "3. Actualiza tu número si es necesario\n" +
                               "4. Intenta nuevamente",

                        130 => "💰 No tienes saldo suficiente en tu cuenta PayPhone.\n\n" +
                               "Para completar tu compra:\n" +
                               "1. Abre la app PayPhone\n" +
                               "2. Recarga tu cuenta\n" +
                               "3. Vuelve aquí y completa tu compra\n\n" +
                               $"Monto requerido: Verifica en tu carrito",

                        140 => "❌ La transacción fue rechazada.\n\n" +
                               "Esto puede ocurrir por:\n" +
                               "• Saldo insuficiente\n" +
                               "• Límites de transacción excedidos\n" +
                               "• Problemas con tu cuenta\n\n" +
                               "Por favor verifica tu cuenta PayPhone e intenta nuevamente.",

                        150 => "📊 El monto supera tu límite diario de transacciones.\n\n" +
                               "Puedes:\n" +
                               "1. Aumentar tu límite desde la app PayPhone\n" +
                               "2. Intentar con menos tickets\n" +
                               "3. Esperar hasta mañana para completar la compra",

                        _ => $"⚠️ {errorObj.Message}\n\n" +
                             "Si el problema persiste:\n" +
                             "• Verifica tu cuenta PayPhone\n" +
                             "• Contacta con nuestro soporte\n" +
                             "• Intenta otro método de pago"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"No se pudo parsear error de PayPhone: {ex.Message}");
            }

            return "😕 Hubo un problema al procesar tu pago con PayPhone.\n\n" +
                   "Por favor verifica:\n" +
                   "• Tu cuenta PayPhone está activa\n" +
                   "• Tienes saldo suficiente\n" +
                   "• Tu número está registrado correctamente\n\n" +
                   "Si el problema persiste, contacta con soporte o intenta otro método de pago.";
        }
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
                    _logger.LogInformation($"🔄 Verificando transacción: {transaction.Reference}");

                    // Consultar estado en PayPhone
                    var status = await CheckPaymentStatusAsync(transaction.Id.ToString());

                    if (status == null)
                    {
                        _logger.LogWarning($"⚠️ No se pudo obtener estado para transacción: {transaction.Reference}");
                        continue;
                    }

                    // Actualizar según el estado
                    if (status.StatusCode == 3 && transaction.Status != "Approved") // APROBADO
                    {
                        _logger.LogInformation($"✅ Transacción APROBADA: {transaction.Reference}");

                        // Generar tickets
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

                            // Actualizar transacción
                            transaction.Status = "Approved";
                            transaction.UpdatedAt = DateTime.UtcNow;

                            _logger.LogInformation($"🎫 {tickets.Count} ticket(s) generados para transacción: {transaction.Reference}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"❌ Error al generar tickets para transacción {transaction.Reference}: {ex.Message}");
                            // No actualizamos el estado si falla la generación de tickets
                        }
                    }
                    else if (status.StatusCode == 2 && transaction.Status != "Rejected") // RECHAZADO
                    {
                        _logger.LogWarning($"❌ Transacción RECHAZADA: {transaction.Reference}");
                        transaction.Status = "Rejected";
                        transaction.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (status.StatusCode == 4) // CANCELADO
                    {
                        _logger.LogInformation($"🚫 Transacción CANCELADA: {transaction.Reference}");
                        transaction.Status = "Cancelled";
                        transaction.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (status.StatusCode == 1) // PENDIENTE
                    {
                        // Mantener como pendiente
                        _logger.LogDebug($"⏳ Transacción aún PENDIENTE: {transaction.Reference}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error al verificar transacción {transaction.Reference}: {ex.Message}");
                    // Continuar con la siguiente transacción
                }
            }

            // Guardar todos los cambios
            await _context.SaveChangesAsync();
            _logger.LogInformation($"✅ Verificación de transacciones completada");
        }

        #endregion
    }
}