using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket> CreateAsync(Ticket ticket);
        Task<Ticket?> GetByIdAsync(int id);
        Task<Ticket?> GetByQrCodeAsync(string qrCode, int companyId);
        Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId, int companyId);
        Task<Ticket> UpdateAsync(Ticket ticket);
        Task<bool> DeleteAsync(int id);
    }
}