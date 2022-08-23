namespace Core.Interfaces
{
    public interface IUnitOfWork
    {

        IBrandRepository Brands { get; }
        ICategoryRepository Categories { get; }
        IRoleRepository Roles { get; }
        IUserRepository Users { get; }

        Task<int> SaveAsync();

    }
}
