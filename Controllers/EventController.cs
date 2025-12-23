using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var events = await _service.GetAllAsync();
                return Ok(new
                {
                    success = true,
                    data = events
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
                var eventModel = await _service.GetByIdAsync(id);
                if (eventModel == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Evento no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = eventModel
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

        [Authorize]
        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var events = await _service.GetMyEventsAsync(userId);
                return Ok(new
                {
                    success = true,
                    data = events
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
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var created = await _service.CreateAsync(userId, dto);
                return Ok(new
                {
                    success = true,
                    message = "Evento creado exitosamente",
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
        public async Task<IActionResult> Update(int id, [FromBody] EventUpdateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var updated = await _service.UpdateAsync(id, userId, dto);

                if (updated == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Evento no encontrado o no tienes permisos"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Evento actualizado exitosamente",
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
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var deleted = await _service.DeleteAsync(id, userId);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Evento no encontrado o no tienes permisos"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Evento eliminado exitosamente"
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