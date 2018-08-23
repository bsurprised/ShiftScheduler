using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Onyx.ShiftScheduler.Core.Common;

namespace Onyx.ShiftScheduler.Core.Interfaces
{
    public interface IRepository<T, TPrimaryKey> where T : IEntity<TPrimaryKey>
    {
        void Delete(T entity);
        Task DeleteAsync(T entity);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(TPrimaryKey id);
        Task<List<T>> GetAllListAsync();
        Task<List<T>> GetAllListAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetAsync(TPrimaryKey id);
        Task<T> InsertAsync(T entity);
        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(T entity);
        Task<T> InsertOrUpdateAsync(T entity);
        IQueryable<T> Query();
        IQueryable<T> QueryNoTracking();
        void SaveChanges();
        Task SaveChangesAsync();
        Task<T> UpdateAsync(T entity);
    }
}