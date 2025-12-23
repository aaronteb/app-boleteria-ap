namespace AppBoleteriaApi.Model
{
    public class Event
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int OrganizerId { get; set; }
        public int? VenueId { get; set; }  

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }


        public string? Location { get; set; }
        public string? City { get; set; }           
        public string? Country { get; set; }        
        public int? Capacity { get; set; }          

        public DateTime EventDateTime { get; set; }
        public string? BannerUrl { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public Company? Company { get; set; }
        public User? Organizer { get; set; }
        public Venue? Venue { get; set; }
        public ICollection<TicketType>? TicketTypes { get; set; }
    }
}