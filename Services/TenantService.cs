namespace AppBoleteriaApi.Services
{
    public class TenantService : ITenantService
    {
        private int _companyId;
        private string _companySlug;

        public int GetCompanyId() => _companyId;
        public void SetCompanyId(int companyId) => _companyId = companyId;

        public string GetCompanySlug() => _companySlug;
        public void SetCompanySlug(string slug) => _companySlug = slug;
    }
}