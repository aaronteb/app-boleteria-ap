using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface ICompanyService
    {
        Task<CompanyResponseDto> CreateAsync(CompanyCreateDto dto);
        Task<CompanyResponseDto?> GetByIdAsync(int id);
        Task<CompanyResponseDto?> GetBySlugAsync(string slug);
        Task<IEnumerable<CompanyResponseDto>> GetAllAsync();
    }
}