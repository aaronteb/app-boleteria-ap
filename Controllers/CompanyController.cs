using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companies = await _service.GetAllAsync();
                return Ok(new
                {
                    success = true,
                    data = companies
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
                var company = await _service.GetByIdAsync(id);
                if (company == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Compañía no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = company
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

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            try
            {
                var company = await _service.GetBySlugAsync(slug);
                if (company == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Compañía no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = company
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Compañía creada exitosamente",
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
    }
}