namespace AppBoleteriaApi.DTOs
{
    public class MenuRouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Icon { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; }
        public List<MenuRouteDto>? Children { get; set; }
    }
}