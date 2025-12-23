namespace AppBoleteriaApi.Model
{
    public class MenuRoute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Icon { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; }
        public string RequiredRole { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public MenuRoute? Parent { get; set; }
        public ICollection<MenuRoute>? Children { get; set; }
    }
}