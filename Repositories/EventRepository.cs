using AppBoleteriaApi.Data;
using AppBoleteriaApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateAsync(Event eventModel)
        {
            try
            {
               
                eventModel.Company = null;
                eventModel.Organizer = null;
                eventModel.Venue = null;
                eventModel.TicketTypes = null;

                await _context.Events.AddAsync(eventModel);
                await _context.SaveChangesAsync();

                return eventModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR en EventRepository.CreateAsync: {ex.Message}");
                Console.WriteLine($"❌ INNER EXCEPTION: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Company)
                .Include(e => e.Venue)  // ✅ Agregar Venue
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Company)
                .Include(e => e.Venue)  // ✅ Agregar Venue
                .Where(e => e.IsActive)
                .OrderByDescending(e => e.EventDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetAllAsync(int companyId)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Company)
                .Include(e => e.Venue)  // ✅ Agregar Venue
                .Where(e => e.CompanyId == companyId && e.IsActive)
                .OrderByDescending(e => e.EventDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetByOrganizerIdAsync(int organizerId, int companyId)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Company)
                .Include(e => e.Venue)  
                .Where(e => e.OrganizerId == organizerId && e.CompanyId == companyId && e.IsActive)
                .OrderByDescending(e => e.EventDateTime)
                .ToListAsync();
        }

        public async Task<Event> UpdateAsync(Event eventModel)
        {
            try
            {
                eventModel.Company = null;
                eventModel.Organizer = null;
                eventModel.Venue = null;
                eventModel.TicketTypes = null;

                _context.Events.Update(eventModel);
                await _context.SaveChangesAsync();

                return eventModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR en EventRepository.UpdateAsync: {ex.Message}");
                Console.WriteLine($"❌ INNER EXCEPTION: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null) return false;

            eventModel.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}