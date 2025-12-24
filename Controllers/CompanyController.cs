using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;
using System.Security.Claims;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;

        public CompanyController(ICompanyService companyService, IUserService userService)
        {
            _companyService = companyService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companies = await _companyService.GetAllAsync();
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
                var company = await _companyService.GetByIdAsync(id);
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
                var company = await _companyService.GetBySlugAsync(slug);
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
                var created = await _companyService.CreateAsync(dto);
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

        // ====================================
        // NUEVOS ENDPOINTS PARA GESTIÓN DE USUARIOS
        // ====================================

        /// <summary>
        /// Admin: Listar usuarios de una compañía específica
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetCompanyUsers(int id)
        {
            try
            {
                var users = await _userService.GetUsersByCompanyAsync(id);
                return Ok(new
                {
                    success = true,
                    data = users
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

        /// <summary>
        /// Admin de Compañía: Listar usuarios de MI compañía
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("my-users")]
        public async Task<IActionResult> GetMyCompanyUsers()
        {
            try
            {
                var companyIdClaim = User.FindFirst("CompanyId")?.Value;
                if (string.IsNullOrEmpty(companyIdClaim) || companyIdClaim == "0")
                {
                    var allUsers = await _userService.GetAllUsersAsync();
                    return Ok(new
                    {
                        success = true,
                        data = allUsers,
                        message = "Todos los usuarios del sistema"
                    });
                }

                int companyId = int.Parse(companyIdClaim);
                var users = await _userService.GetUsersByCompanyAsync(companyId);
                return Ok(new
                {
                    success = true,
                    data = users
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

        /// <summary>
        /// Admin: Crear usuario en una compañía (requiere header X-Company-Slug)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] UserRegisterDto dto)
        {
            try
            {
                if (dto.RoleId != 1 && dto.RoleId != 3 && dto.RoleId != 4)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Solo puedes crear usuarios Admin (1), Organizer (3) o Staff (4)"
                    });
                }

                var user = await _userService.CreateAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Usuario creado exitosamente",
                    data = new
                    {
                        id = user.Id,
                        fullName = user.FullName,
                        email = user.Email,
                        roleId = user.RoleId,
                        companyId = user.CompanyId
                    }
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

        /// <summary>
        /// Admin: Activar/Desactivar usuario
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{userId}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int userId, [FromBody] ToggleStatusDto dto)
        {
            try
            {
                await _userService.ToggleUserStatusAsync(userId, dto.IsActive);
                return Ok(new
                {
                    success = true,
                    message = dto.IsActive ? "Usuario activado" : "Usuario desactivado"
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

    public class ToggleStatusDto
    {
        public bool IsActive { get; set; }
    }
}