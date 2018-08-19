using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onyx.ShiftScheduler.Core.Common;
using Onyx.ShiftScheduler.Core.Exceptions;
using Onyx.ShiftScheduler.Core.Extensions;
using Onyx.ShiftScheduler.Core.Interfaces;

namespace Onyx.ShiftScheduler.Infrastructure.Data
{
    /// <summary>
    ///     Repository class to handle database context routines
    /// </summary>
    /// <typeparam name="T">Entity class to manage by repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type for the entity</typeparam>
    public class Repository<T, TPrimaryKey> : IRepository<T, TPrimaryKey> where T : Entity<TPrimaryKey>
    {
        private readonly AppDbContext _dbContext;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return GetTable<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> QueryNoTracking<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return Query<TEntity>().AsNoTracking();
        }

        public IQueryable<TEntity> QueryIncluding<TEntity>(params Expression<Func<TEntity, object>>[] propertySelectors)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var query = GetTable<TEntity>().AsQueryable();

            if (!propertySelectors.IsNullOrEmpty())
                foreach (var propertySelector in propertySelectors)
                    query = query.Include(propertySelector);

            return query;
        }

        public IQueryable<TEntity> QueryNoTrackingIncluding<TEntity>(
            params Expression<Func<TEntity, object>>[] propertySelectors) where TEntity : class, IEntity<TPrimaryKey>
        {
            return QueryIncluding(propertySelectors).AsNoTracking();
        }

        public async Task<List<TEntity>> GetAllListAsync<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().ToListAsync();
        }

        public async Task<List<TEntity>> GetAllListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().Where(predicate).ToListAsync();
        }

        public TEntity Get<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = FirstOrDefault<TEntity>(id);
            if (entity == null) throw new EntityNotFoundException(typeof(TEntity), id);

            return entity;
        }

        public async Task<TEntity> GetAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = await FirstOrDefaultAsync<TEntity>(id);
            if (entity == null) throw new EntityNotFoundException(typeof(TEntity), id);

            return entity;
        }

        public TEntity Find<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            return FirstOrDefault<TEntity>(id);
        }

        public async Task<TEntity> FindAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            return await FirstOrDefaultAsync<TEntity>(id);
        }

        public int Count<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return Query<TEntity>().Count();
        }

        public long LongCount<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return Query<TEntity>().LongCount();
        }

        public async Task<int> CountAsync<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().CountAsync();
        }

        public async Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().Where(predicate).CountAsync();
        }

        public async Task<long> LongCountAsync<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().LongCountAsync();
        }

        public async Task<long> LongCountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().Where(predicate).LongCountAsync();
        }

        public virtual TEntity Single<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return Query<TEntity>().Single(predicate);
        }

        public async Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().SingleAsync(predicate);
        }

        public virtual TEntity FirstOrDefault<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Query<TEntity>().FirstOrDefault(CreateEqualityExpressionForId<TEntity>(id));
        }

        public TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return Query<TEntity>().FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(TPrimaryKey id)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().FirstOrDefaultAsync(CreateEqualityExpressionForId<TEntity>(id));
        }

        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await Query<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            return GetTable<TEntity>().Add(entity).Entity;
        }

        public Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.FromResult(Insert(entity));
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public TEntity Update<TEntity>(TPrimaryKey id, Action<TEntity> updateAction)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = Get<TEntity>(id);
            updateAction(entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync<TEntity>(TPrimaryKey id, Func<TEntity, Task> updateAction)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = await GetAsync<TEntity>(id);
            await updateAction(entity);
            return entity;
        }

        public TEntity InsertOrUpdate<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            return entity.IsTransient()
                ? Insert(entity)
                : Update(entity);
        }

        public async Task<TEntity> InsertOrUpdateAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return entity.IsTransient()
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }

        public TPrimaryKey InsertAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            entity = Insert(entity);

            SaveChanges();

            return entity.Id;
        }

        public async Task<TPrimaryKey> InsertAndGetIdAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            entity = await InsertAsync(entity);

            await SaveChangesAsync();

            return entity.Id;
        }

        public TPrimaryKey InsertOrUpdateAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            entity = InsertOrUpdate(entity);

            SaveChanges();

            return entity.Id;
        }

        public async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            entity = await InsertOrUpdateAsync(entity);

            await SaveChangesAsync();

            return entity.Id;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            AttachIfNot(entity);
            GetTable<TEntity>().Remove(entity);
        }

        public Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            Delete(entity);
            return Task.FromResult(0);
        }

        public void Delete<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = GetFromChangeTrackerOrNull<TEntity>(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }

            entity = FirstOrDefault<TEntity>(id);
            if (entity != null) Delete(entity);

            // else
            // Cannot find the entity, do nothing.
        }

        public Task DeleteAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            Delete<TEntity>(id);
            return Task.FromResult(0);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            foreach (var entity in Query<TEntity>().Where(predicate).ToList()) Delete(entity);
        }

        public Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            Delete(predicate);
            return Task.FromResult(0);
        }

        public void DeleteSoftly<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            AttachIfNot(entity);
            entity.IsDeleted = true;
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public Task DeleteSoftlyAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            DeleteSoftly(entity);
            return Task.FromResult(0);
        }

        public void DeleteSoftly<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            var entity = GetFromChangeTrackerOrNull<TEntity>(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                _dbContext.Entry(entity).State = EntityState.Modified;
                return;
            }

            entity = FirstOrDefault<TEntity>(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                _dbContext.Entry(entity).State = EntityState.Modified;
            }

            // else
            // Cannot find the entity, do nothing.
        }

        public Task DeleteSoftlyAsync<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            DeleteSoftly<TEntity>(id);
            return Task.FromResult(0);
        }

        public void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleDbException(ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                HandleDbException(ex);
            }
        }

        private DbSet<TEntity> GetTable<TEntity>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return _dbContext.Set<TEntity>();
        }

        private void AttachIfNot<TEntity>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null) return;

            GetTable<TEntity>().Attach(entity);
        }

        private TEntity GetFromChangeTrackerOrNull<TEntity>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entry = _dbContext.ChangeTracker.Entries()
                .FirstOrDefault(
                    ent =>
                        ent.Entity is TEntity &&
                        EqualityComparer<TPrimaryKey>.Default.Equals(id, (ent.Entity as TEntity).Id)
                );

            return entry?.Entity as TEntity;
        }

        private static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity>(TPrimaryKey id)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        private void HandleDbException(Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
                throw new ApplicationErrorException("FAILED_PROCESSING_DB_CONCURRENCY");

            if (ex is DbUpdateException dbEx)
                if (dbEx.InnerException != null)
                {
                    if (dbEx.InnerException is SqlException sqlEx)
                        switch (sqlEx.Number)
                        {
                            case 2627: // Unique constraint error
                                throw new ApplicationErrorException("FAILED_PROCESSING_DB_UNIQUE");
                            case 2601: // Duplicated key row error
                                throw new ApplicationErrorException("FAILED_PROCESSING_DB_DUPLICATE");
                            case 547: // Constraint check violation
                                throw new ApplicationErrorException("FAILED_PROCESSING_DB_CONSRTRAINT");

                            default:
                                throw new ApplicationErrorException("FAILED_PROCESSING_DB_REQUEST");
                        }

                    throw new ApplicationErrorException("FAILED_PROCESSING_DB_REQUEST");
                }

            throw new ApplicationErrorException("FAILED_PROCESSING_DB_REQUEST");
        }
    }
}