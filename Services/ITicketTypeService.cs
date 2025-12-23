using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface ITicketTypeService
    {
        Task<TicketTypeResponseDto> CreateAsync(TicketTypeCreateDto dto);
        Task<TicketTypeResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<TicketTypeResponseDto>> GetByEventIdAsync(int eventId);
        Task<TicketTypeResponseDto?> UpdateAsync(int id, TicketTypeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}