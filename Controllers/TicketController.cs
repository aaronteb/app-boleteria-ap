using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;
using System.Security.Claims;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
        {
            _service = service;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] TicketPurchaseDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var tickets = await _service.PurchaseTicketsAsync(userId, dto);
                return Ok(new
                {
                    success = true,
                    message = "Tickets comprados exitosamente",
                    data = tickets
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var tickets = await _service.GetMyTicketsAsync(userId);
                return Ok(new
                {
                    success = true,
                    data = tickets
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var ticket = await _service.GetByIdAsync(id);
                if (ticket == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Ticket no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = ticket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("qr/{qrCode}")]
        public async Task<IActionResult> GetByQrCode(string qrCode)
        {
            try
            {
                var ticket = await _service.GetByQrCodeAsync(qrCode);
                if (ticket == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Ticket no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = ticket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("use/{qrCode}")]
        public async Task<IActionResult> UseTicket(string qrCode)
        {
            try
            {
                var staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _service.UseTicketAsync(qrCode, staffId);
                return Ok(new
                {
                    success = true,
                    message = "Ticket usado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}