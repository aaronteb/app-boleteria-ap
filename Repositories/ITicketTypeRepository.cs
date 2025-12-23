using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Repositories
{
    public interface ITicketTypeRepository
    {
        Task<TicketType> CreateAsync(TicketType ticketType);
        Task<TicketType?> GetByIdAsync(int id);
        Task<IEnumerable<TicketType>> GetByEventIdAsync(int eventId, int companyId);
        Task<TicketType> UpdateAsync(TicketType ticketType);
        Task<bool> DeleteAsync(int id);
    }
}