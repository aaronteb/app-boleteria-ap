namespace AppBoleteriaApi.DTOs
{
    public class TicketTypeResponseDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
    }
}