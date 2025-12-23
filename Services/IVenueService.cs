using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface IVenueService
    {
        Task<VenueResponseDto> CreateAsync(VenueCreateDto dto);
        Task<VenueResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<VenueResponseDto>> GetAllAsync();
        Task<VenueResponseDto?> UpdateAsync(int id, VenueCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}