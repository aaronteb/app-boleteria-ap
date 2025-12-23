using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _service;

        public VenueController(IVenueService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var venues = await _service.GetAllAsync();
                return Ok(new
                {
                    success = true,
                    data = venues
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
                var venue = await _service.GetByIdAsync(id);
                if (venue == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Local no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = venue
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
        public async Task<IActionResult> Create([FromBody] VenueCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Local creado exitosamente",
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
        public async Task<IActionResult> Update(int id, [FromBody] VenueCreateDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Local no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Local actualizado exitosamente",
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
                        message = "Local no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Local eliminado exitosamente"
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