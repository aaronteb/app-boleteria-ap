namespace AppBoleteriaApi.DTOs
{
    public class EventUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string? BannerUrl { get; set; }
    }
}