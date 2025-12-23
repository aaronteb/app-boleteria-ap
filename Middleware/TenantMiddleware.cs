using AppBoleteriaApi.Services;
using AppBoleteriaApi.Repositories;

namespace AppBoleteriaApi.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService, ICompanyRepository companyRepo)
        {
            // Obtener el slug de la compañía desde el header o subdomain
            var companySlug = context.Request.Headers["X-Company-Slug"].FirstOrDefault();

            if (!string.IsNullOrEmpty(companySlug))
            {
                var company = await companyRepo.GetBySlugAsync(companySlug);
                if (company != null && company.IsActive)
                {
                    tenantService.SetCompanyId(company.Id);
                    tenantService.SetCompanySlug(company.Slug);
                }
            }

            await _next(context);
        }
    }
}