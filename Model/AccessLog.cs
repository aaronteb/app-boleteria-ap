namespace AppBoleteriaApi.Model
{
    public class AccessLog
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int? StaffId { get; set; }
        public DateTime? ScannedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true; 

        public Ticket? Ticket { get; set; }
        public User? Staff { get; set; }
    }
}