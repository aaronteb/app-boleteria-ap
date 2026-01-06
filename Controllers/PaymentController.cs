using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;
using System.Security.Claims;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPayPhoneService _payPhoneService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPayPhoneService payPhoneService,
            ILogger<PaymentController> logger)
        {
            _payPhoneService = payPhoneService;
            _logger = logger;
        }

        /// <summary>
        /// Iniciar proceso de pago - Retorna datos para la Cajita de Pagos
        /// </summary>
        [Authorize]
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                {
                    _logger.LogWarning("❌ Usuario no autenticado intentando iniciar pago");
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Usuario no autenticado"
                    });
                }

                _logger.LogInformation($"🎫 Usuario {userId} iniciando pago - Ticket: {dto.TicketTypeId}, Qty: {dto.Quantity}");

                var result = await _payPhoneService.InitiatePaymentAsync(userId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Pago no iniciado: {result.Message}");

                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }

                // ✅ Retornar datos para inicializar la Cajita en el frontend
                return Ok(new
                {
                    success = true,
                    message = "Datos preparados para la Cajita de Pagos",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en InitiatePayment: {ex.Message}");

                return BadRequest(new
                {
                    success = false,
                    message = $"Error al procesar el pago: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 🆕 NUEVO: Confirmar pago desde la URL de respuesta de la Cajita
        /// Este endpoint se llama cuando PayPhone redirige después del pago
        /// Parámetros vienen en la URL: ?id=123&clientTransactionId=BOL-1-20250106120000
        /// </summary>
        [AllowAnonymous] // Permitir acceso sin auth porque viene de PayPhone
        [HttpGet("confirm-cajita")]
        public async Task<IActionResult> ConfirmPaymentFromCajita([FromQuery] long id, [FromQuery] string clientTransactionId)
        {
            try
            {
                _logger.LogInformation($"🔔 WEBHOOK recibido de PayPhone - ID: {id}, ClientTxId: {clientTransactionId}");

                var dto = new ConfirmPaymentFromCajitaDto
                {
                    Id = id,
                    ClientTxId = clientTransactionId
                };

                var result = await _payPhoneService.ConfirmPaymentFromCajitaAsync(dto);

                _logger.LogInformation($"✅ Confirmación procesada - StatusCode: {result.StatusCode}, Status: {result.TransactionStatus}");

                // 🎯 Redirigir según el resultado
                if (result.StatusCode == 3) // APROBADO
                {
                    // ✅ Redirigir a una URL que el WebView detectará
                    // Puedes usar un scheme personalizado o una ruta específica
                    return Redirect($"/payment-success?ref={clientTransactionId}&txId={result.TransactionId}");
                }
                else if (result.StatusCode == 2) // CANCELADO
                {
                    // ❌ Redirigir a página de cancelación
                    return Redirect($"/payment-cancelled?ref={clientTransactionId}");
                }
                else // PENDIENTE u otro estado
                {
                    // ⏳ Redirigir a página de pendiente
                    return Redirect($"/payment-pending?ref={clientTransactionId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en ConfirmPaymentFromCajita: {ex.Message}");

                // Redirigir a página de error
                return Redirect($"/payment-error?message={Uri.EscapeDataString(ex.Message)}");
            }
        }

        /// <summary>
        /// Obtener estado de una transacción desde la BD
        /// </summary>
        [Authorize]
        [HttpGet("status/{transactionId}")]
        public async Task<IActionResult> GetTransactionStatus(string transactionId)
        {
            try
            {
                var status = await _payPhoneService.GetTransactionStatusAsync(transactionId);

                if (status == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Transacción no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en GetTransactionStatus: {ex.Message}");

                return BadRequest(new
                {
                    success = false,
                    message = $"Error al obtener estado: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Consultar estado directamente en PayPhone (para verificaciones manuales)
        /// </summary>
        [Authorize]
        [HttpGet("check-payphone/{transactionId}")]
        public async Task<IActionResult> CheckPayPhoneStatus(string transactionId)
        {
            try
            {
                _logger.LogInformation($"📡 Consultando estado en PayPhone - ID: {transactionId}");

                var status = await _payPhoneService.CheckPaymentStatusAsync(transactionId);

                return Ok(new
                {
                    success = true,
                    data = status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en CheckPayPhoneStatus: {ex.Message}");

                return BadRequest(new
                {
                    success = false,
                    message = $"Error al consultar PayPhone: {ex.Message}"
                });
            }
        }
    }
}