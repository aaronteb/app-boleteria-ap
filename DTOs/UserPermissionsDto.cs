namespace AppBoleteriaApi.DTOs
{
    public class UserPermissionsDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public List<string> Permissions { get; set; }
        public List<MenuRouteDto> MenuRoutes { get; set; }
    }
}