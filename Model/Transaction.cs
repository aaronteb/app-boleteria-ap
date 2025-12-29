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

        // Campos nuevos para PayPhone
        public string? PayPhoneTransactionId { get; set; }
        public string? PayPhonePaymentId { get; set; }
        public string? Reference { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navegación
        public Company? Company { get; set; }
        public User? User { get; set; }
        public Ticket? Ticket { get; set; }
    }
}