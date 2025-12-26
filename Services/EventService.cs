using AppBoleteriaApi.Data;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repo;
        private readonly ITenantService _tenantService;
        private readonly AppDbContext _context;

        public EventService(IEventRepository repo, ITenantService tenantService, AppDbContext context)
        {
            _repo = repo;
            _tenantService = tenantService;
            _context = context;
        }

        public async Task<EventResponseDto> CreateAsync(int organizerId, EventCreateDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == organizerId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            if (user.RoleId == 1 || user.RoleId == 3 || user.RoleId == 4)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                    throw new Exception("Usuario no tiene compañía asignada en el sistema");

                var companyId = user.CompanyId.Value;

                if (!dto.VenueId.HasValue && string.IsNullOrEmpty(dto.Location))
                {
                    throw new Exception("Debe especificar un Venue o proporcionar una ubicación manual");
                }

                if (dto.VenueId.HasValue)
                {
                    var venue = await _context.Venues
                        .FirstOrDefaultAsync(v => v.Id == dto.VenueId.Value && v.CompanyId == companyId && v.IsActive);

                    if (venue == null)
                    {
                        throw new Exception("El venue especificado no existe o no pertenece a su empresa");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dto.Location))
                        throw new Exception("La ubicación es requerida cuando no se especifica un venue");

                    if (!dto.Capacity.HasValue || dto.Capacity.Value <= 0)
                        throw new Exception("La capacidad debe ser mayor a 0 cuando no se especifica un venue");
                }

                var eventModel = new Event
                {
                    CompanyId = companyId, // ✅ Usar companyId del usuario
                    OrganizerId = organizerId,
                    VenueId = dto.VenueId,  // ✅ Puede ser null
                    Title = dto.Title,
                    Description = dto.Description,
                    Location = dto.Location,
                    City = dto.City,  // ✅ NUEVO
                    Country = dto.Country,  // ✅ NUEVO
                    Capacity = dto.Capacity,  // ✅ NUEVO
                    EventDateTime = DateTime.SpecifyKind(dto.EventDateTime, DateTimeKind.Utc),
                    BannerUrl = dto.BannerUrl,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var created = await _repo.CreateAsync(eventModel);

                created = await _context.Events
                    .Include(e => e.Venue)
                    .Include(e => e.Organizer)
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(e => e.Id == created.Id);

                return MapToResponseDto(created!);
            }
            else
            {
                throw new Exception("Solo Admin/Organizer/Staff pueden crear eventos");
            }
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            var companyId = _tenantService.GetCompanyId();

            IQueryable<Event> query = _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Company)  // ✅ NUEVO
                .Include(e => e.Venue)  // ✅ NUEVO
                .Include(e => e.TicketTypes.Where(tt => tt.IsActive))
                    .ThenInclude(tt => tt.Tickets.Where(t => t.IsActive))
                .Where(e => e.Id == id && e.IsActive);

            if (companyId > 0)
            {
                query = query.Where(e => e.CompanyId == companyId);
            }

            var eventModel = await query.FirstOrDefaultAsync();

            if (eventModel == null) return null;

            var response = MapToResponseDto(eventModel);

            // ✅ Agregar estadísticas de tickets
            response.TicketTypes = eventModel.TicketTypes?.Select(tt => new TicketTypeWithSalesDto
            {
                Id = tt.Id,
                Name = tt.Name,
                Price = tt.Price,
                Stock = tt.Stock,
                Sold = tt.Tickets?.Count ?? 0,
                Available = tt.Stock - (tt.Tickets?.Count ?? 0),
                Revenue = tt.Price * (tt.Tickets?.Count ?? 0)
            }).ToList();

            response.TotalTicketsSold = eventModel.TicketTypes?.Sum(tt => tt.Tickets?.Count ?? 0) ?? 0;
            response.TotalRevenue = eventModel.TicketTypes?.Sum(tt => tt.Price * (tt.Tickets?.Count ?? 0)) ?? 0;

            return response;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllAsync()
        {
            var companyId = _tenantService.GetCompanyId();

            IQueryable<Event> query = _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Company)  // ✅ NUEVO
                .Include(e => e.Venue)  // ✅ NUEVO
                .Include(e => e.TicketTypes.Where(tt => tt.IsActive))
                    .ThenInclude(tt => tt.Tickets.Where(t => t.IsActive))
                .Where(e => e.IsActive);

            if (companyId > 0)
            {
                query = query.Where(e => e.CompanyId == companyId);
            }

            var events = await query
                .OrderByDescending(e => e.EventDateTime)
                .ToListAsync();

            return events.Select(e =>
            {
                var response = MapToResponseDto(e);
                response.TicketTypes = e.TicketTypes?.Select(tt => new TicketTypeWithSalesDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    Price = tt.Price,
                    Stock = tt.Stock,
                    Sold = tt.Tickets?.Count ?? 0,
                    Available = tt.Stock - (tt.Tickets?.Count ?? 0),
                    Revenue = tt.Price * (tt.Tickets?.Count ?? 0)
                }).ToList();
                response.TotalTicketsSold = e.TicketTypes?.Sum(tt => tt.Tickets?.Count ?? 0) ?? 0;
                response.TotalRevenue = e.TicketTypes?.Sum(tt => tt.Price * (tt.Tickets?.Count ?? 0)) ?? 0;
                return response;
            });
        }

        public async Task<IEnumerable<EventResponseDto>> GetMyEventsAsync(int userId)
        {
            // Obtener el usuario para saber su compañía
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            if (user.CompanyId == null || user.CompanyId == 0)
                return new List<EventResponseDto>();

            var companyId = user.CompanyId.Value;
            var events = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Company)
                .Include(e => e.Venue)
                .Include(e => e.TicketTypes.Where(tt => tt.IsActive))
                    .ThenInclude(tt => tt.Tickets.Where(t => t.IsActive))
                .Where(e => e.CompanyId == companyId && e.IsActive) 
                .OrderByDescending(e => e.EventDateTime)
                .ToListAsync();

            return events.Select(e =>
            {
                var response = MapToResponseDto(e);
                response.TicketTypes = e.TicketTypes?.Select(tt => new TicketTypeWithSalesDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    Price = tt.Price,
                    Stock = tt.Stock,
                    Sold = tt.Tickets?.Count ?? 0,
                    Available = tt.Stock - (tt.Tickets?.Count ?? 0),
                    Revenue = tt.Price * (tt.Tickets?.Count ?? 0)
                }).ToList();
                response.TotalTicketsSold = e.TicketTypes?.Sum(tt => tt.Tickets?.Count ?? 0) ?? 0;
                response.TotalRevenue = e.TicketTypes?.Sum(tt => tt.Price * (tt.Tickets?.Count ?? 0)) ?? 0;
                return response;
            });
        }

        public async Task<EventResponseDto?> UpdateAsync(int id, int organizerId, EventUpdateDto dto)
        {
            var eventModel = await _repo.GetByIdAsync(id);
            if (eventModel == null) return null;

            var companyId = _tenantService.GetCompanyId();
            if (eventModel.CompanyId != companyId || eventModel.OrganizerId != organizerId)
                return null;

            if (!string.IsNullOrEmpty(dto.Title))
                eventModel.Title = dto.Title;
            if (dto.Description != null)
                eventModel.Description = dto.Description;
            if (dto.Location != null)
                eventModel.Location = dto.Location;
            if (dto.EventDateTime.HasValue)
                eventModel.EventDateTime = DateTime.SpecifyKind(dto.EventDateTime.Value, DateTimeKind.Utc);
            if (dto.BannerUrl != null)
                eventModel.BannerUrl = dto.BannerUrl;

            var updated = await _repo.UpdateAsync(eventModel);

            updated = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.Id == updated.Id);

            return MapToResponseDto(updated!);
        }

        public async Task<bool> DeleteAsync(int id, int organizerId)
        {
            var eventModel = await _repo.GetByIdAsync(id);
            if (eventModel == null) return false;

            var companyId = _tenantService.GetCompanyId();
            if (eventModel.CompanyId != companyId || eventModel.OrganizerId != organizerId)
                return false;

            return await _repo.DeleteAsync(id);
        }

        private EventResponseDto MapToResponseDto(Event eventModel)
        {
            return new EventResponseDto
            {
                Id = eventModel.Id,
                CompanyId = eventModel.CompanyId,
                CompanyName = eventModel.Company?.Name,
                OrganizerId = eventModel.OrganizerId,
                OrganizerName = eventModel.Organizer?.FullName ?? "",
                VenueId = eventModel.VenueId,
                VenueName = eventModel.Venue?.Name,
                Title = eventModel.Title,
                Description = eventModel.Description,
                Location = eventModel.VenueId.HasValue ? eventModel.Venue?.Address : eventModel.Location,
                City = eventModel.VenueId.HasValue ? eventModel.Venue?.City : eventModel.City,
                Country = eventModel.VenueId.HasValue ? eventModel.Venue?.Country : eventModel.Country,
                Capacity = eventModel.VenueId.HasValue ? eventModel.Venue?.Capacity : eventModel.Capacity,
                EventDateTime = eventModel.EventDateTime,
                BannerUrl = eventModel.BannerUrl,
                CreatedAt = eventModel.CreatedAt ?? DateTime.UtcNow,
                IsActive = eventModel.IsActive,
                TicketTypes = null,
                TotalTicketsSold = 0,
                TotalRevenue = 0
            };
        }
    }
}