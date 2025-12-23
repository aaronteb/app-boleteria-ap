namespace AppBoleteriaApi.DTOs
{
    public class CompanyResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Logo { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}