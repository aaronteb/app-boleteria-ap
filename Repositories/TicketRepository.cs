using AppBoleteriaApi.Data;
using AppBoleteriaApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.TicketType)
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
        }

        public async Task<Ticket?> GetByQrCodeAsync(string qrCode, int companyId)
        {
            return await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.TicketType)
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t => t.QrCode == qrCode && t.CompanyId == companyId && t.IsActive);
        }

        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId, int companyId)
        {
            return await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.TicketType)
                .Include(t => t.Company)
                .Where(t => t.UserId == userId && t.CompanyId == companyId && t.IsActive)
                .OrderByDescending(t => t.PurchaseDate)
                .ToListAsync();
        }

        public async Task<Ticket> UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;

            ticket.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}