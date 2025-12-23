using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company> CreateAsync(Company company);
        Task<Company?> GetByIdAsync(int id);
        Task<Company?> GetBySlugAsync(string slug);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company> UpdateAsync(Company company);
        Task<bool> DeleteAsync(int id);
    }
}