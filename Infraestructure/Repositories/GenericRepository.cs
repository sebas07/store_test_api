using Core.Entities;
using Core.Interfaces;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {

        #region Fields
        protected readonly StoreContext _context;
        #endregion Fields

        #region Constructors
        public GenericRepository(StoreContext context)
        {
            this._context = context;
        }
        #endregion 

        #region Public Methods
        public virtual void Add(T entity)
        {
            this._context.Set<T>().Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            this._context.Set<T>().AddRange(entities);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return this._context.Set<T>().Where(expression);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this._context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await this._context.Set<T>().FindAsync(id);
        }

        public virtual void Remove(T entity)
        {
            this._context.Set<T>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            this._context.Set<T>().RemoveRange(entities);
        }

        public virtual void Update(T entity)
        {
            this._context.Set<T>().Update(entity);
        }
        #endregion

    }
}
