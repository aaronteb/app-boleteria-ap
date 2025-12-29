using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface ICompanyService
    {
        // Métodos existentes
        Task<CompanyResponseDto> CreateAsync(CompanyCreateDto dto);
        Task<CompanyResponseDto?> GetByIdAsync(int id);
        Task<CompanyResponseDto?> GetBySlugAsync(string slug);
        Task<IEnumerable<CompanyResponseDto>> GetAllAsync();

        Task<CompanyPayPhoneStatusDto> ConfigurePayPhoneAsync(int companyId, CompanyPayPhoneConfigDto dto);
        Task<CompanyPayPhoneStatusDto> GetPayPhoneStatusAsync(int companyId);
        Task<bool> DisablePayPhoneAsync(int companyId);
    }
}