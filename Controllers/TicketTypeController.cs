using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketTypeController : ControllerBase
    {
        private readonly ITicketTypeService _service;

        public TicketTypeController(ITicketTypeService service)
        {
            _service = service;
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEventId(int eventId)
        {
            try
            {
                var ticketTypes = await _service.GetByEventIdAsync(eventId);
                return Ok(new
                {
                    success = true,
                    data = ticketTypes
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
                var ticketType = await _service.GetByIdAsync(id);
                if (ticketType == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Tipo de ticket no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = ticketType
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

        [Authorize(Roles = "Admin,Organizer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TicketTypeCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tipo de ticket creado exitosamente",
                    data = created
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

        [Authorize(Roles = "Admin,Organizer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TicketTypeUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Tipo de ticket no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Tipo de ticket actualizado exitosamente",
                    data = updated
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

        [Authorize(Roles = "Admin,Organizer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Tipo de ticket no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Tipo de ticket eliminado exitosamente"
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