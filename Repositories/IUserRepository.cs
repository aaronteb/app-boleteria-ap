using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailAndCompanyAsync(string email, int companyId);
        Task<User?> GetByIdAsync(int id);
        Task<User> UpdateAsync(User user);

        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByCompanyIdAsync(int companyId);
    }
}