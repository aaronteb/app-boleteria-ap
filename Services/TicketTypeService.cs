using AppBoleteriaApi.Data;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppBoleteriaApi.Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly ITicketTypeRepository _repo;
        private readonly IEventRepository _eventRepo;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketTypeService(
            ITicketTypeRepository repo,
            IEventRepository eventRepo,
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _eventRepo = eventRepo;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TicketTypeResponseDto> CreateAsync(TicketTypeCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                throw new Exception("Usuario no autenticado");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            if (user.RoleId == 1 || user.RoleId == 3)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    throw new Exception("Usuario no tiene compañía asignada");

                var companyId = user.CompanyId.Value;

                var eventModel = await _eventRepo.GetByIdAsync(dto.EventId);
                if (eventModel == null)
                    throw new Exception("Evento no encontrado");

                if (eventModel.CompanyId != companyId)
                    throw new Exception("No tienes permiso para crear tipos de ticket en este evento");

                var ticketType = new TicketType
                {
                    CompanyId = companyId,
                    EventId = dto.EventId,
                    Name = dto.Name,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    IsActive = true
                };

                var created = await _repo.CreateAsync(ticketType);

                return new TicketTypeResponseDto
                {
                    Id = created.Id,
                    EventId = created.EventId,
                    EventTitle = eventModel.Title,
                    Name = created.Name,
                    Price = created.Price,
                    Stock = created.Stock,
                    IsActive = created.IsActive
                };
            }
            else
            {
                throw new Exception("Solo Admin/Organizer pueden crear tipos de ticket");
            }
        }

        public async Task<TicketTypeResponseDto?> GetByIdAsync(int id)
        {
            var ticketType = await _repo.GetByIdAsync(id);
            if (ticketType == null) return null;

            var userId = GetCurrentUserId();
            if (userId == 0) return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    return null;

                if (ticketType.CompanyId != user.CompanyId.Value)
                    return null;
            }
            var eventModel = await _eventRepo.GetByIdAsync(ticketType.EventId);

            return new TicketTypeResponseDto
            {
                Id = ticketType.Id,
                EventId = ticketType.EventId,
                EventTitle = eventModel?.Title ?? "",
                Name = ticketType.Name,
                Price = ticketType.Price,
                Stock = ticketType.Stock,
                IsActive = ticketType.IsActive
            };
        }

        public async Task<IEnumerable<TicketTypeResponseDto>> GetByEventIdAsync(int eventId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                throw new Exception("Usuario no autenticado");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    throw new Exception("Usuario no tiene compañía asignada");

                var companyId = user.CompanyId.Value;
                var eventModel = await _eventRepo.GetByIdAsync(eventId);

                if (eventModel == null || eventModel.CompanyId != companyId)
                    return new List<TicketTypeResponseDto>();

                var ticketTypes = await _repo.GetByEventIdAsync(eventId, companyId);

                return ticketTypes.Select(tt => new TicketTypeResponseDto
                {
                    Id = tt.Id,
                    EventId = tt.EventId,
                    EventTitle = eventModel.Title,
                    Name = tt.Name,
                    Price = tt.Price,
                    Stock = tt.Stock,
                    IsActive = tt.IsActive
                });
            }
            else
            {
                var eventModel = await _eventRepo.GetByIdAsync(eventId);
                if (eventModel == null || !eventModel.IsActive)
                    return new List<TicketTypeResponseDto>();

                var ticketTypes = await _context.TicketTypes
                    .Where(tt => tt.EventId == eventId && tt.IsActive)
                    .ToListAsync();

                return ticketTypes.Select(tt => new TicketTypeResponseDto
                {
                    Id = tt.Id,
                    EventId = tt.EventId,
                    EventTitle = eventModel.Title,
                    Name = tt.Name,
                    Price = tt.Price,
                    Stock = tt.Stock,
                    IsActive = tt.IsActive
                });
            }
        }

        public async Task<TicketTypeResponseDto?> UpdateAsync(int id, TicketTypeUpdateDto dto)
        {
            var ticketType = await _repo.GetByIdAsync(id);
            if (ticketType == null) return null;

            var userId = GetCurrentUserId();
            if (userId == 0) return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            if (user.RoleId == 1 || user.RoleId == 3)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    return null;

                if (ticketType.CompanyId != user.CompanyId.Value)
                    return null;

                if (!string.IsNullOrEmpty(dto.Name))
                    ticketType.Name = dto.Name;
                if (dto.Price.HasValue)
                    ticketType.Price = dto.Price.Value;
                if (dto.Stock.HasValue)
                    ticketType.Stock = dto.Stock.Value;

                var updated = await _repo.UpdateAsync(ticketType);
                var eventModel = await _eventRepo.GetByIdAsync(updated.EventId);

                return new TicketTypeResponseDto
                {
                    Id = updated.Id,
                    EventId = updated.EventId,
                    EventTitle = eventModel?.Title ?? "",
                    Name = updated.Name,
                    Price = updated.Price,
                    Stock = updated.Stock,
                    IsActive = updated.IsActive
                };
            }

            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticketType = await _repo.GetByIdAsync(id);
            if (ticketType == null) return false;

            var userId = GetCurrentUserId();
            if (userId == 0) return false;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            if (user.RoleId == 1 || user.RoleId == 3)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    return false;

                if (ticketType.CompanyId != user.CompanyId.Value)
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