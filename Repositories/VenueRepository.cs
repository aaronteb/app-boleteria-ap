using AppBoleteriaApi.Data;
using AppBoleteriaApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly AppDbContext _context;

        public VenueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Venue> CreateAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
            await _context.SaveChangesAsync();
            return venue;
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _context.Venues
                .Include(v => v.Company)
                .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);
        }

        public async Task<IEnumerable<Venue>> GetByCompanyIdAsync(int companyId)
        {
            return await _context.Venues
                .Include(v => v.Company)
                .Where(v => v.CompanyId == companyId && v.IsActive)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<Venue> UpdateAsync(Venue venue)
        {
            _context.Venues.Update(venue);
            await _context.SaveChangesAsync();
            return venue;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return false;

            venue.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}