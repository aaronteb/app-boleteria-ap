namespace AppBoleteriaApi.Model
{
    public class Venue
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public Company? Company { get; set; }
        public ICollection<Event>? Events { get; set; }
    }
}