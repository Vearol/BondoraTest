using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAll(System.Linq.Expressions.Expression<Func<TEntity, bool>> navigationPropertyPath);
        IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate);
        TEntity GetById(int id);
        Task<TEntity> GetByIdAsync(Guid id);
        void Create(TEntity entity);
        Task CreateAsync(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
        void Save();
        Task SaveAsync();
    }
}
