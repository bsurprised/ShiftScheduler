using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Onyx.ShiftScheduler.Core.Interfaces
{
    public interface IRepository<T, TPrimaryKey> where T : IEntity<TPrimaryKey>
    {
        int Count<TEntity>() where TEntity : class, IEntity<TPrimaryKey>;
        Task<int> CountAsync<TEntity>() where TEntity : class, IEntity<TPrimaryKey>;

        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>;

        void Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;
        void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        void Delete<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;

        Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>;

        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task DeleteAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;
        void DeleteSoftly<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete;
        void DeleteSoftly<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete;
        Task DeleteSoftlyAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete;
        Task DeleteSoftlyAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete;
        TEntity Find<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TEntity> FindAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;

        TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>;

        TEntity FirstOrDefault<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;

        Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>;

        Task<TEntity> FirstOrDefaultAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;
        TEntity Get<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;
        Task<List<TEntity>> GetAllListAsync<TEntity>() where TEntity : class, IEntity<TPrimaryKey>;

        Task<List<TEntity>> GetAllListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>;

        Task<TEntity> GetAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;
        TEntity Insert<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        TPrimaryKey InsertAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TPrimaryKey> InsertAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        TEntity InsertOrUpdate<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        TPrimaryKey InsertOrUpdateAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;

        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>;

        Task<TEntity> InsertOrUpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        long LongCount<TEntity>() where TEntity : class, IEntity<TPrimaryKey>;
        Task<long> LongCountAsync<TEntity>() where TEntity : class, IEntity<TPrimaryKey>;

        Task<long> LongCountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>;

        IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity<TPrimaryKey>;

        IQueryable<TEntity> QueryIncluding<TEntity>(params Expression<Func<TEntity, object>>[] propertySelectors)
            where TEntity : class, IEntity<TPrimaryKey>;

        IQueryable<TEntity> QueryNoTracking<TEntity>() where TEntity : class, IEntity<TPrimaryKey>;

        IQueryable<TEntity> QueryNoTrackingIncluding<TEntity>(
            params Expression<Func<TEntity, object>>[] propertySelectors) where TEntity : class, IEntity<TPrimaryKey>;

        void SaveChanges();
        Task SaveChangesAsync();
        TEntity Single<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;

        Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>;

        TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;

        TEntity Update<TEntity>(TPrimaryKey id, Action<TEntity> updateAction)
            where TEntity : class, IEntity<TPrimaryKey>;

        Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;

        Task<TEntity> UpdateAsync<TEntity>(TPrimaryKey id, Func<TEntity, Task> updateAction)
            where TEntity : class, IEntity<TPrimaryKey>;
    }
}