using API.Dtos;

namespace API.Services.Interfaces
{
    public interface IUserService
    {

        Task<string> RegisterAsync(RegisterDto model);
        Task<UserDataDto> GetTokenAsync(LoginDto model);
        Task<UserDataDto> RefreshTokenAsync(string refreshToken);
        Task<string> AddRoleAsync(AddRoleDto model);

    }
}
