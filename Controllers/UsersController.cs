using Microsoft.AspNetCore.Mvc;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Services;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            try
            {
                var created = await _service.CreateAsync(userDto);
                return Ok(new
                {
                    success = true,
                    message = "Usuario registrado exitosamente",
                    user = new
                    {
                        id = created.Id,
                        email = created.Email,
                        fullName = created.FullName
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al registrar usuario",
                    error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var result = await _service.LoginAsync(loginDto);

                if (result == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Email o contraseña incorrectos"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Login exitoso",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al iniciar sesión",
                    error = ex.Message
                });
            }
        }
    }
}