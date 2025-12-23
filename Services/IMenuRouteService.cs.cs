using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface IMenuRouteService
    {
        Task<List<MenuRouteDto>> GetMenuByRoleAsync(string roleName);
    }
}