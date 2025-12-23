namespace AppBoleteriaApi.DTOs
{
    public class TicketResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
        public string EventTitle { get; set; }
        public string QrCode { get; set; }
        public bool Used { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool IsActive { get; set; }
    }
}