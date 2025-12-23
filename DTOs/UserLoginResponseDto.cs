namespace AppBoleteriaApi.DTOs
{
    public class UserLoginResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RoleName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public List<MenuRouteDto> MenuRoutes { get; set; } = new List<MenuRouteDto>();
    }
}