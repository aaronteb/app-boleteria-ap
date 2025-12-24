using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;

namespace AppBoleteriaApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IEventService _eventService;

        public AdminController(
            IUserService userService,
            ICompanyService companyService,
            IEventService eventService)
        {
            _userService = userService;
            _companyService = companyService;
            _eventService = eventService;
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetAllCompanies()
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

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
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

        [HttpGet("events")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllAsync();
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

        [HttpGet("companies/{companyId}/users")]
        public async Task<IActionResult> GetUsersByCompany(int companyId)
        {
            try
            {
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

        [HttpPost("companies")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto dto)
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

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] UserRegisterDto dto)
        {
            try
            {
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
    }
}