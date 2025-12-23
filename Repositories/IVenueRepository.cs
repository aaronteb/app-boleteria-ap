using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Repositories
{
    public interface IVenueRepository
    {
        Task<Venue> CreateAsync(Venue venue);
        Task<Venue?> GetByIdAsync(int id);
        Task<IEnumerable<Venue>> GetByCompanyIdAsync(int companyId);
        Task<Venue> UpdateAsync(Venue venue);
        Task<bool> DeleteAsync(int id);
    }
}