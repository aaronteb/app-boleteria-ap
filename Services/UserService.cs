using AppBoleteriaApi.Data;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppBoleteriaApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;
        private readonly IMenuRouteService _menuRouteService;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserService(IUserRepository repo, IConfiguration configuration, ITenantService tenantService, IMenuRouteService menuRouteService, AppDbContext context , IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _configuration = configuration;
            _tenantService = tenantService;
            _menuRouteService = menuRouteService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> CreateAsync(UserRegisterDto userDto)


        {
            int? companyId = null;

            if (userDto.RoleId == 1 || userDto.RoleId == 3 || userDto.RoleId == 4) 
            {
                companyId = _tenantService.GetCompanyId();

                if (companyId == 0)
                    throw new Exception("Company no especificada. Use el header X-Company-Slug para crear Admin/Organizer/Staff");
            }

            var user = new User
            {
                CompanyId = companyId,
                FullName = userDto.FullName,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Phone = userDto.Phone,
                RoleId = userDto.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return await _repo.CreateAsync(user);
        }

        public async Task<UserLoginResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _repo.GetByEmailAsync(loginDto.Email);

            if (user == null)
                return null;

            if (!user.IsActive)
                throw new Exception("Usuario inactivo. Contacte al administrador.");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;
            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    throw new Exception("Usuario no tiene compañía asignada en el sistema");
            }

            var token = GenerateJwtToken(user);
            var menuRoutes = await _menuRouteService.GetMenuByRoleAsync(user.Role?.Name ?? "User");

            return new UserLoginResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = token,
                RoleName = user.Role?.Name ?? "Unknown",
                CompanyId = user.CompanyId ?? 0,
                CompanyName = user.Company?.Name ?? "",
                MenuRoutes = menuRoutes
            };
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
                    new Claim("CompanyId", user.CompanyId.ToString()),
                    new Claim("CompanySlug", user.Company?.Slug ?? "")
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task ToggleUserStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var currentUserId = GetCurrentUserId(); // Obtiene del token
            var users = await _repo.GetAllAsync();

            return users
                .Where(u => u.Id != currentUserId) // Excluye automáticamente
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    RoleId = u.RoleId,
                    RoleName = u.Role?.Name ?? "Unknown",
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company?.Name,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                });
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByCompanyAsync(int companyId)
        {
            var currentUserId = GetCurrentUserId(); 
            var users = await _repo.GetByCompanyIdAsync(companyId);

            return users
                .Where(u => u.Id != currentUserId)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    RoleId = u.RoleId,
                    RoleName = u.Role?.Name ?? "Unknown",
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company?.Name,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                });
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? "Unknown",
                CompanyId = user.CompanyId,
                CompanyName = user.Company?.Name,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    
    private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Usuario no autenticado");

            return int.Parse(userIdClaim);
        }
    }
}

