using AppBoleteriaApi.DTOs;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Services
{
    public interface IUserService
    {
        Task<User> CreateAsync(UserRegisterDto userDto);
        Task<UserLoginResponseDto?> LoginAsync(UserLoginDto loginDto);
    }
}