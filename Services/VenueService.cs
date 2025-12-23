using AppBoleteriaApi.Data;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Repositories;
using Microsoft.EntityFrameworkCore; // ✅ Agregar este using
using System.Security.Claims; // ✅ Agregar este using

namespace AppBoleteriaApi.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _repo;
        private readonly ITenantService _tenantService;
        private readonly AppDbContext _context; // ✅ Agregar
        private readonly IHttpContextAccessor _httpContextAccessor; // ✅ Agregar

        public VenueService(
            IVenueRepository repo,
            ITenantService tenantService,
            AppDbContext context, // ✅ Agregar
            IHttpContextAccessor httpContextAccessor) // ✅ Agregar
        {
            _repo = repo;
            _tenantService = tenantService;
            _context = context; // ✅ Inicializar
            _httpContextAccessor = httpContextAccessor; // ✅ Inicializar
        }

        public async Task<VenueResponseDto> CreateAsync(VenueCreateDto dto)
        {
            // ✅ CAMBIO: Obtener userId del token JWT
            var userId = GetCurrentUserId();
            if (userId == 0)
                throw new Exception("Usuario no autenticado");

            // ✅ Obtener usuario para saber su compañía
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            // Solo Admin/Organizer/Staff pueden crear venues
            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    throw new Exception("Usuario no tiene compañía asignada");

                // ✅ Usar LA COMPAÑÍA DEL USUARIO
                var companyId = user.CompanyId.Value;

                var venue = new Venue
                {
                    CompanyId = companyId, // ✅ Usar companyId del usuario
                    Name = dto.Name,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State,
                    Country = dto.Country,
                    PostalCode = dto.PostalCode,
                    Phone = dto.Phone,
                    Capacity = dto.Capacity,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var created = await _repo.CreateAsync(venue);

                return new VenueResponseDto
                {
                    Id = created.Id,
                    CompanyId = created.CompanyId,
                    CompanyName = created.Company?.Name ?? "",
                    Name = created.Name,
                    Address = created.Address,
                    City = created.City,
                    State = created.State,
                    Country = created.Country,
                    PostalCode = created.PostalCode,
                    Phone = created.Phone,
                    Capacity = created.Capacity,
                    CreatedAt = created.CreatedAt,
                    IsActive = created.IsActive
                };
            }
            else
            {
                throw new Exception("Solo Admin/Organizer/Staff pueden crear venues");
            }
        }

        public async Task<VenueResponseDto?> GetByIdAsync(int id)
        {
            var venue = await _repo.GetByIdAsync(id);
            if (venue == null) return null;

            // ✅ CAMBIO: Obtener userId del token
            var userId = GetCurrentUserId();
            if (userId == 0) return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            // Para Admin/Organizer/Staff, verificar que pertenezca a su compañía
            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    return null;

                if (venue.CompanyId != user.CompanyId.Value)
                    return null;
            }
            // Para usuarios normales, permitir ver cualquier venue (opcional)
            // o puedes decidir si quieres que no puedan ver venues

            return new VenueResponseDto
            {
                Id = venue.Id,
                CompanyId = venue.CompanyId,
                CompanyName = venue.Company?.Name ?? "",
                Name = venue.Name,
                Address = venue.Address,
                City = venue.City,
                State = venue.State,
                Country = venue.Country,
                PostalCode = venue.PostalCode,
                Phone = venue.Phone,
                Capacity = venue.Capacity,
                CreatedAt = venue.CreatedAt,
                IsActive = venue.IsActive
            };
        }

        public async Task<IEnumerable<VenueResponseDto>> GetAllAsync()
        {
            // ✅ CAMBIO: Obtener userId del token
            var userId = GetCurrentUserId();
            if (userId == 0)
                throw new Exception("Usuario no autenticado");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            // Para Admin/Organizer/Staff, mostrar solo venues de su compañía
            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    throw new Exception("Usuario no tiene compañía asignada");

                var companyId = user.CompanyId.Value;
                var venues = await _repo.GetByCompanyIdAsync(companyId);

                return venues.Select(v => new VenueResponseDto
                {
                    Id = v.Id,
                    CompanyId = v.CompanyId,
                    CompanyName = v.Company?.Name ?? "",
                    Name = v.Name,
                    Address = v.Address,
                    City = v.City,
                    State = v.State,
                    Country = v.Country,
                    PostalCode = v.PostalCode,
                    Phone = v.Phone,
                    Capacity = v.Capacity,
                    CreatedAt = v.CreatedAt,
                    IsActive = v.IsActive
                });
            }
            else
            {
                // Para usuarios normales, mostrar todos los venues o ninguno
                // Depende de tu lógica de negocio
                return new List<VenueResponseDto>();
            }
        }

        public async Task<VenueResponseDto?> UpdateAsync(int id, VenueCreateDto dto)
        {
            var venue = await _repo.GetByIdAsync(id);
            if (venue == null) return null;

            // ✅ CAMBIO: Obtener userId del token
            var userId = GetCurrentUserId();
            if (userId == 0) return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            // Solo Admin/Organizer/Staff pueden actualizar
            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    return null;

                if (venue.CompanyId != user.CompanyId.Value)
                    return null;

                venue.Name = dto.Name;
                venue.Address = dto.Address;
                venue.City = dto.City;
                venue.State = dto.State;
                venue.Country = dto.Country;
                venue.PostalCode = dto.PostalCode;
                venue.Phone = dto.Phone;
                venue.Capacity = dto.Capacity;

                var updated = await _repo.UpdateAsync(venue);

                return new VenueResponseDto
                {
                    Id = updated.Id,
                    CompanyId = updated.CompanyId,
                    CompanyName = updated.Company?.Name ?? "",
                    Name = updated.Name,
                    Address = updated.Address,
                    City = updated.City,
                    State = updated.State,
                    Country = updated.Country,
                    PostalCode = updated.PostalCode,
                    Phone = updated.Phone,
                    Capacity = updated.Capacity,
                    CreatedAt = updated.CreatedAt,
                    IsActive = updated.IsActive
                };
            }

            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venue = await _repo.GetByIdAsync(id);
            if (venue == null) return false;

            var userId = GetCurrentUserId();
            if (userId == 0) return false;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    return false;

                if (venue.CompanyId != user.CompanyId.Value)
                    return false;

                return await _repo.DeleteAsync(id);
            }

            return false;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            return 0;
        }
    }
}