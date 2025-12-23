using System.ComponentModel.DataAnnotations;

namespace AppBoleteriaApi.DTOs
{
    public class EventCreateDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public int? VenueId { get; set; }

        public string? Location { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public int? Capacity { get; set; }
        public DateTime EventDateTime { get; set; }
        public string? BannerUrl { get; set; }
    }
}