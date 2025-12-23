using AppBoleteriaApi.Data;
using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repo;
        private readonly ITicketTypeRepository _ticketTypeRepo;
        private readonly IEventRepository _eventRepo;
        private readonly ITenantService _tenantService;
        private readonly AppDbContext _context; 

        public TicketService(
            ITicketRepository repo,
            ITicketTypeRepository ticketTypeRepo,
            IEventRepository eventRepo,
            ITenantService tenantService,
            AppDbContext context) 
        {
            _repo = repo;
            _ticketTypeRepo = ticketTypeRepo;
            _eventRepo = eventRepo;
            _tenantService = tenantService;
            _context = context; 
        }

        public async Task<List<TicketResponseDto>> PurchaseTicketsAsync(int userId, TicketPurchaseDto dto)
        {
            var ticketType = await _ticketTypeRepo.GetByIdAsync(dto.TicketTypeId);
            if (ticketType == null)
                throw new Exception("Tipo de ticket no encontrado");

            var companyId = ticketType.CompanyId;

            if (ticketType.Stock < dto.Quantity)
                throw new Exception("No hay suficiente stock disponible");

            var eventModel = await _eventRepo.GetByIdAsync(ticketType.EventId);
            var tickets = new List<TicketResponseDto>();

            for (int i = 0; i < dto.Quantity; i++)
            {
                var ticket = new Ticket
                {
                    CompanyId = companyId,
                    UserId = userId, 
                    TicketTypeId = dto.TicketTypeId,
                    QrCode = Guid.NewGuid().ToString(),
                    Used = false,
                    PurchaseDate = DateTime.UtcNow,
                    IsActive = true
                };

                var created = await _repo.CreateAsync(ticket);

                tickets.Add(new TicketResponseDto
                {
                    Id = created.Id,
                    UserId = created.UserId,
                    UserName = "",
                    TicketTypeId = created.TicketTypeId,
                    TicketTypeName = ticketType.Name,
                    EventTitle = eventModel?.Title ?? "",
                    QrCode = created.QrCode,
                    Used = created.Used,
                    PurchaseDate = created.PurchaseDate,
                    IsActive = created.IsActive
                });
            }

            ticketType.Stock -= dto.Quantity;
            await _ticketTypeRepo.UpdateAsync(ticketType);

            return tickets;
        }

        public async Task<TicketResponseDto?> GetByIdAsync(int id)
        {
            var ticket = await _repo.GetByIdAsync(id);
            if (ticket == null) return null;

            var eventModel = await _eventRepo.GetByIdAsync(ticket.TicketType?.EventId ?? 0);

            return new TicketResponseDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                UserName = ticket.User?.FullName ?? "",
                TicketTypeId = ticket.TicketTypeId,
                TicketTypeName = ticket.TicketType?.Name ?? "",
                EventTitle = eventModel?.Title ?? "",
                QrCode = ticket.QrCode,
                Used = ticket.Used,
                PurchaseDate = ticket.PurchaseDate,
                IsActive = ticket.IsActive
            };
        }

        public async Task<TicketResponseDto?> GetByQrCodeAsync(string qrCode)
        {
            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t => t.QrCode == qrCode && t.IsActive);

            if (ticket == null) return null;

            return new TicketResponseDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                UserName = ticket.User?.FullName ?? "",
                TicketTypeId = ticket.TicketTypeId,
                TicketTypeName = ticket.TicketType?.Name ?? "",
                EventTitle = ticket.TicketType?.Event?.Title ?? "",
                QrCode = ticket.QrCode,
                Used = ticket.Used,
                PurchaseDate = ticket.PurchaseDate,
                IsActive = ticket.IsActive
            };
        }

        public async Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int userId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Include(t => t.Company)
                .Where(t => t.UserId == userId && t.IsActive)
                .OrderByDescending(t => t.PurchaseDate)
                .ToListAsync();

            return tickets.Select(t => new TicketResponseDto
            {
                Id = t.Id,
                UserId = t.UserId,
                UserName = t.User?.FullName ?? "",
                TicketTypeId = t.TicketTypeId,
                TicketTypeName = t.TicketType?.Name ?? "",
                EventTitle = t.TicketType?.Event?.Title ?? "",
                QrCode = t.QrCode,
                Used = t.Used,
                PurchaseDate = t.PurchaseDate,
                IsActive = t.IsActive
            });
        }

        public async Task<bool> UseTicketAsync(string qrCode, int staffId)
        {
            var staffUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == staffId);

            if (staffUser == null)
                throw new Exception("Staff no encontrado");
            if (staffUser.RoleId == 1 || staffUser.RoleId == 3 || staffUser.RoleId == 4)
            {
                if (staffUser.CompanyId == null || staffUser.CompanyId == 0)
                    throw new Exception("Usuario no tiene compañía asignada");
                var companyId = staffUser.CompanyId.Value;

                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.QrCode == qrCode && t.CompanyId == companyId && t.IsActive);

                if (ticket == null)
                    throw new Exception("Ticket no encontrado");

                if (ticket.Used)
                    throw new Exception("Este ticket ya fue usado");

                ticket.Used = true;
                await _context.SaveChangesAsync();

                return true;
            }
            throw new Exception("Solo Staff/Organizer/Admin pueden validar tickets");
        }
    }
}