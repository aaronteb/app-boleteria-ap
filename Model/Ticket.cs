namespace AppBoleteriaApi.Model
{
    public class Ticket
    {
        public int Id { get; set; }
        public int CompanyId { get; set; } 
        public int UserId { get; set; }
        public int TicketTypeId { get; set; }
        public string QrCode { get; set; }
        public bool Used { get; set; } = false;
        public DateTime? PurchaseDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public Company? Company { get; set; } 
        public User? User { get; set; }
        public TicketType? TicketType { get; set; }
        public ICollection<AccessLog>? AccessLogs { get; set; }
    }
}