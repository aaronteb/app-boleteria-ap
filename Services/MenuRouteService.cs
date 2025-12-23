using AppBoleteriaApi.Data;
using AppBoleteriaApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppBoleteriaApi.Services
{
    public class MenuRouteService : IMenuRouteService
    {
        private readonly AppDbContext _context;

        public MenuRouteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuRouteDto>> GetMenuByRoleAsync(string roleName)
        {
            var routes = await _context.MenuRoutes
                .Where(m => m.IsActive && m.ParentId == null && (m.RequiredRole == roleName || m.RequiredRole == "All"))
                .OrderBy(m => m.Order)
                .Include(m => m.Children.Where(c => c.IsActive && (c.RequiredRole == roleName || c.RequiredRole == "All")))
                .ToListAsync();

            return routes.Select(m => MapToDto(m, roleName)).ToList();
        }

        private MenuRouteDto MapToDto(Model.MenuRoute route, string roleName)
        {
            return new MenuRouteDto
            {
                Id = route.Id,
                Name = route.Name,
                Path = route.Path,
                Icon = route.Icon,
                Order = route.Order,
                Children = route.Children?
                    .Where(c => c.IsActive && (c.RequiredRole == roleName || c.RequiredRole == "All"))
                    .OrderBy(c => c.Order)
                    .Select(c => MapToDto(c, roleName))
                    .ToList()
            };
        }
    }
}