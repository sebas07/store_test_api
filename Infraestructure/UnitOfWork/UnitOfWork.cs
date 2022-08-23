using Core.Entities;
using Core.Interfaces;
using Infraestructure.Data;
using Infraestructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {

        private readonly StoreContext _context;
        private IRoleRepository _roleRepository;
        private IUserRepository _userRepository;
        private IBrandRepository _brandRepository;
        private ICategoryRepository _categoryRepository;

        public UnitOfWork(StoreContext context)
        {
            this._context = context;
        }

        public IBrandRepository Brands
        {
            get
            {
                if (this._brandRepository == null)
                {
                    this._brandRepository = new BrandRepository(this._context);
                }
                return this._brandRepository;
            }
        }

        public ICategoryRepository Categories
        {
            get
            {
                if (this._categoryRepository == null)
                    this._categoryRepository = new CategoryRepository(this._context);
                return this._categoryRepository;
            }
        }

        public IRoleRepository Roles
        {
            get
            {
                if (this._roleRepository == null)
                    this._roleRepository = new RoleRepository(this._context);
                return this._roleRepository;
            }
        }

        public IUserRepository Users
        {
            get
            {
                if (this._userRepository == null)
                    this._userRepository = new UserRepository(this._context);
                return this._userRepository;
            }
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await this._context.SaveChangesAsync();
        }

    }
}
