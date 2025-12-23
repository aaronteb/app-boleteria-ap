namespace AppBoleteriaApi.DTOs
{
    public class TicketTypeCreateDto
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}