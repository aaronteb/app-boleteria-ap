using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface IEventService
    {
        Task<EventResponseDto> CreateAsync(int organizerId, EventCreateDto dto);
        Task<EventResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<EventResponseDto>> GetAllAsync();
        Task<IEnumerable<EventResponseDto>> GetMyEventsAsync(int organizerId);
        Task<EventResponseDto?> UpdateAsync(int id, int organizerId, EventUpdateDto dto);
        Task<bool> DeleteAsync(int id, int organizerId);

    }
}