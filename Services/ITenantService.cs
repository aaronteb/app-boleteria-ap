namespace AppBoleteriaApi.Services
{
    public interface ITenantService
    {
        int GetCompanyId();
        void SetCompanyId(int companyId);
        string GetCompanySlug();
        void SetCompanySlug(string slug);
    }
}