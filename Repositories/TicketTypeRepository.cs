using AppBoleteriaApi.Data;
using AppBoleteriaApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Repositories
{
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly AppDbContext _context;

        public TicketTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TicketType> CreateAsync(TicketType ticketType)
        {
            await _context.TicketTypes.AddAsync(ticketType);
            await _context.SaveChangesAsync();
            return ticketType;
        }

        public async Task<TicketType?> GetByIdAsync(int id)
        {
            return await _context.TicketTypes
                .Include(tt => tt.Company)
                .FirstOrDefaultAsync(tt => tt.Id == id && tt.IsActive);
        }

        public async Task<IEnumerable<TicketType>> GetByEventIdAsync(int eventId, int companyId)
        {
            return await _context.TicketTypes
                .Include(tt => tt.Company)
                .Where(tt => tt.EventId == eventId && tt.CompanyId == companyId && tt.IsActive)
                .ToListAsync();
        }

        public async Task<TicketType> UpdateAsync(TicketType ticketType)
        {
            _context.TicketTypes.Update(ticketType);
            await _context.SaveChangesAsync();
            return ticketType;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticketType = await _context.TicketTypes.FindAsync(id);
            if (ticketType == null) return false;

            ticketType.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}