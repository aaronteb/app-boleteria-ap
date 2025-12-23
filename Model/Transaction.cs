namespace AppBoleteriaApi.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CompanyId { get; set; } 
        public int? UserId { get; set; }
        public int? TicketId { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public Company? Company { get; set; } 
        public User? User { get; set; }
        public Ticket? Ticket { get; set; }
    }
}