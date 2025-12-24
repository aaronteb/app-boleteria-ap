using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Services
{
    public interface IUserService
    {
        Task<User> CreateAsync(UserRegisterDto userDto);
        Task<UserLoginResponseDto?> LoginAsync(UserLoginDto loginDto);

        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<IEnumerable<UserResponseDto>> GetUsersByCompanyAsync(int companyId);
        Task<UserResponseDto?> GetUserByIdAsync(int id);

        Task ToggleUserStatusAsync(int userId, bool isActive);
    }
}