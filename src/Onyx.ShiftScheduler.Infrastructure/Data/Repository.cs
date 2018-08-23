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

        public IQueryable<T> Query()
        {
            return GetTable().AsQueryable();
        }

        public IQueryable<T> QueryNoTracking()
        {
            return Query().AsNoTracking();
        }

        public async Task<List<T>> GetAllListAsync() 
        {
            return await Query().ToListAsync();
        }

        public async Task<List<T>> GetAllListAsync(Expression<Func<T, bool>> predicate)
        {
            return await Query().Where(predicate).ToListAsync();
        }

        public async Task<T> GetAsync(TPrimaryKey id) 
        {
            var entity = await FirstOrDefaultAsync(id);
            if (entity == null) throw new EntityNotFoundException(typeof(T), id);

            return entity;
        }

        public async Task<T> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return await Query().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Query().FirstOrDefaultAsync(predicate);
        }

        public Task<T> InsertAsync(T entity) 
        {
            return Task.FromResult(GetTable().Add(entity).Entity);
        }

        public Task<T> UpdateAsync(T entity) 
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public async Task<T> InsertOrUpdateAsync(T entity)
        {
            return entity.IsTransient()
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }

        public async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(T entity)
        {
            entity = await InsertOrUpdateAsync(entity);

            await SaveChangesAsync();

            return entity.Id;
        }

        public void Delete(T entity) 
        {
            AttachIfNot(entity);
            GetTable().Remove(entity);
        }

        public Task DeleteAsync(T entity) 
        {
            Delete(entity);
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

        private DbSet<T> GetTable()
        {
            return _dbContext.Set<T>();
        }

        private void AttachIfNot(T entity) 
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null) return;

            GetTable().Attach(entity);
        }

        private static Expression<Func<T, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(T));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
            );

            return Expression.Lambda<Func<T, bool>>(lambdaBody, lambdaParam);
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