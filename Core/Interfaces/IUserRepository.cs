using Core.Entities;

namespace Core.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {

        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);

    }
}
