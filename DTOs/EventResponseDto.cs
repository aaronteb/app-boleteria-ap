namespace AppBoleteriaApi.DTOs
{
    public class EventResponseDto
    {
        public int Id { get; set; }

        // ✅ Información de la compañía
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;

        public int? VenueId { get; set; }
        public string? VenueName { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? Location { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? Capacity { get; set; }

        public DateTime EventDateTime { get; set; }
        public string? BannerUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public List<TicketTypeWithSalesDto>? TicketTypes { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TicketTypeWithSalesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }
        public int Available { get; set; }
        public decimal Revenue { get; set; }
    }
}