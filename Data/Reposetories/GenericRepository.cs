using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Reposetories
{
    public abstract class GenericRepository<TContext, TEntity> :
        IGenericRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        protected GenericRepository(TContext context)
        {
            Context = context;
        }

        public TContext Context { get; set; }

        public virtual IQueryable<TEntity> GetAll()
        {
            var query = Context.Set<TEntity>();
            return query;
        }

        public virtual IQueryable<TEntity> GetAll(System.Linq.Expressions.Expression<Func<TEntity, bool>> navigationPropertyPath)
        {
            var query = Context.Set<TEntity>().Include(navigationPropertyPath);
            return query;
        }

        public IQueryable<TEntity> GetBy(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            var query = Context.Set<TEntity>().Where(predicate);
            return query;
        }

        public virtual TEntity GetById(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual Task<TEntity> GetByIdAsync(Guid id)
        {
            return Context.Set<TEntity>().FindAsync(id);
        }

        public virtual void Create(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public virtual Task CreateAsync(TEntity entity)
        {
            return Context.Set<TEntity>().AddAsync(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public virtual void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }

        public virtual Task SaveAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
