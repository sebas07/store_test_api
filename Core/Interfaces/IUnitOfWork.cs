namespace Core.Interfaces
{
    public interface IUnitOfWork
    {

        IRoleRepository Roles { get; }
        IUserRepository Users { get; }

        Task<int> SaveAsync();

    }
}
