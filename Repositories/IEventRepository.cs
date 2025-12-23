using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Repositories
{
    public interface IEventRepository
    {
        Task<Event> CreateAsync(Event eventModel);
        Task<Event?> GetByIdAsync(int id);
        Task<IEnumerable<Event>> GetAllAsync(); 
        Task<IEnumerable<Event>> GetAllAsync(int companyId); 
        Task<IEnumerable<Event>> GetByOrganizerIdAsync(int organizerId, int companyId);
        Task<Event> UpdateAsync(Event eventModel);
        Task<bool> DeleteAsync(int id);
    }
}