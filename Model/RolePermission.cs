namespace AppBoleteriaApi.Model
{
    public class RolePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}