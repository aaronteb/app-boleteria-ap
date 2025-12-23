using AppBoleteriaApi.Data;
using AppBoleteriaApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Company> CreateAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Company?> GetBySlugAsync(string slug)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Company> UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null) return false;

            company.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}