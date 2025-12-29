using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Repositories;

namespace AppBoleteriaApi.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repo;

        public CompanyService(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<CompanyResponseDto> CreateAsync(CompanyCreateDto dto)
        {
            var company = new Company
            {
                Name = dto.Name,
                Slug = dto.Slug.ToLower().Replace(" ", "-"),
                Logo = dto.Logo,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                PayPhoneEnabled = false // Nuevo campo - por defecto deshabilitado
            };

            var created = await _repo.CreateAsync(company);

            return new CompanyResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Slug = created.Slug,
                Logo = created.Logo,
                ContactEmail = created.ContactEmail,
                ContactPhone = created.ContactPhone,
                CreatedAt = created.CreatedAt,
                IsActive = created.IsActive
            };
        }

        public async Task<CompanyResponseDto?> GetByIdAsync(int id)
        {
            var company = await _repo.GetByIdAsync(id);
            if (company == null) return null;

            return new CompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                Slug = company.Slug,
                Logo = company.Logo,
                ContactEmail = company.ContactEmail,
                ContactPhone = company.ContactPhone,
                CreatedAt = company.CreatedAt,
                IsActive = company.IsActive
            };
        }

        public async Task<CompanyResponseDto?> GetBySlugAsync(string slug)
        {
            var company = await _repo.GetBySlugAsync(slug);
            if (company == null) return null;

            return new CompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                Slug = company.Slug,
                Logo = company.Logo,
                ContactEmail = company.ContactEmail,
                ContactPhone = company.ContactPhone,
                CreatedAt = company.CreatedAt,
                IsActive = company.IsActive
            };
        }

        public async Task<IEnumerable<CompanyResponseDto>> GetAllAsync()
        {
            var companies = await _repo.GetAllAsync();
            return companies.Select(c => new CompanyResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Logo = c.Logo,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive
            });
        }

        // ============================================
        // MÉTODOS NUEVOS PARA PAYPHONE
        // ============================================

        public async Task<CompanyPayPhoneStatusDto> ConfigurePayPhoneAsync(int companyId, CompanyPayPhoneConfigDto dto)
        {
            var company = await _repo.GetByIdAsync(companyId);
            if (company == null)
                throw new Exception("Compañía no encontrada");

            if (string.IsNullOrWhiteSpace(dto.PayPhoneToken))
                throw new Exception("El token de PayPhone es requerido");

            // Actualizar configuración de PayPhone
            company.PayPhoneToken = dto.PayPhoneToken;
            company.PayPhoneEnabled = dto.PayPhoneEnabled;

            await _repo.UpdateAsync(company);

            return new CompanyPayPhoneStatusDto
            {
                CompanyId = company.Id,
                CompanyName = company.Name,
                PayPhoneEnabled = company.PayPhoneEnabled,
                HasToken = !string.IsNullOrEmpty(company.PayPhoneToken),
                Message = "Configuración de PayPhone actualizada exitosamente"
            };
        }

        public async Task<CompanyPayPhoneStatusDto> GetPayPhoneStatusAsync(int companyId)
        {
            var company = await _repo.GetByIdAsync(companyId);
            if (company == null)
                throw new Exception("Compañía no encontrada");

            return new CompanyPayPhoneStatusDto
            {
                CompanyId = company.Id,
                CompanyName = company.Name,
                PayPhoneEnabled = company.PayPhoneEnabled,
                HasToken = !string.IsNullOrEmpty(company.PayPhoneToken),
                Message = company.PayPhoneEnabled
                    ? "PayPhone está habilitado para esta compañía"
                    : "PayPhone no está habilitado"
            };
        }

        public async Task<bool> DisablePayPhoneAsync(int companyId)
        {
            var company = await _repo.GetByIdAsync(companyId);
            if (company == null)
                throw new Exception("Compañía no encontrada");

            company.PayPhoneEnabled = false;
            await _repo.UpdateAsync(company);

            return true;
        }
    }
}