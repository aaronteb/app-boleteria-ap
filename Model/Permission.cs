namespace AppBoleteriaApi.Model
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } 
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}