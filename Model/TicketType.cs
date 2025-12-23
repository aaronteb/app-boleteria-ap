namespace AppBoleteriaApi.Model
{
    public class TicketType
    {
        public int Id { get; set; }
        public int CompanyId { get; set; } 
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;

        public Company? Company { get; set; }
        public Event? Event { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
    }
}