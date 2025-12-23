namespace AppBoleteriaApi.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<User>? Users { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; } 
    }
}