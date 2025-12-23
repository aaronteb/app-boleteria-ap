namespace AppBoleteriaApi.DTOs
{
    public class CompanyCreateDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Logo { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
    }
}