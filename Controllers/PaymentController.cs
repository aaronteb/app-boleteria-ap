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
        /// Iniciar proceso de pago con PayPhone
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

                // ⚠️ CRÍTICO: Si el servicio retorna success: false, devolver BadRequest
                if (!result.Success)
                {
                    _logger.LogWarning($"⚠️ Pago no iniciado: {result.Message}");

                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        data = new
                        {
                            transactionId = result.TransactionId,
                            reference = result.Reference
                        }
                    });
                }

                // ✅ Pago iniciado exitosamente
                return Ok(new
                {
                    success = true,
                    message = "Pago iniciado exitosamente",
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

        [Authorize]
        [HttpPost("confirm/{transactionId}")]
        public async Task<IActionResult> ConfirmPayment(string transactionId)
        {
            try
            {
                _logger.LogInformation($"🔍 Confirmando pago - Transacción: {transactionId}");

                var success = await _payPhoneService.ConfirmPaymentAsync(transactionId);

                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Pago confirmado exitosamente. Tickets generados.",
                        status = "Approved"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "El pago aún está pendiente o fue rechazado",
                        status = "Pending/Rejected"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en ConfirmPayment: {ex.Message}");

                return BadRequest(new
                {
                    success = false,
                    message = $"Error al confirmar el pago: {ex.Message}"
                });
            }
        }

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

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> PayPhoneWebhook([FromBody] dynamic callback)
        {
            try
            {
                _logger.LogInformation($"📨 Webhook recibido de PayPhone: {callback}");
                return Ok(new { received = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error en webhook: {ex.Message}");
                return Ok(new { received = true, error = ex.Message });
            }
        }
    }
}