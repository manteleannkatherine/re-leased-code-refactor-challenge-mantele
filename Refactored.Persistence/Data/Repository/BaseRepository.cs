using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Refactored.Persistence.Data.Repository.Interface;

namespace Refactored.Persistence.Data.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private bool _bResult;
        private string _errorMessage = string.Empty;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddCommit(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }

        public async Task<(T, bool result)> AddCommitAsync(T entity)
        {
            await _context.AddAsync(entity);

            _bResult = await _context.SaveChangesAsync() >= 0;
            return (entity, _bResult);
        }

        public async Task<(bool, string)> AddCommitWithErrorMessageAsync(T entity)
        {
            _bResult = true;
            try
            {
                await _context.AddAsync(entity);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage);
        }

        public async Task<(bool, string, T)> AddCommitEntityWithErrorMessageAsync(T entity)
        {
            _bResult = true;
            try
            {
                await _context.AddAsync(entity);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage, entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _context.AddRangeAsync(entities);

        public async Task<(bool, string)> AddRangeWithErrorMessageAsync(IEnumerable<T> entities)
        {
            _bResult = true;
            try
            {
                await _context.AddRangeAsync(entities);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage);
        }

        public IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return query.AsEnumerable();
        }

        public virtual IQueryable<T> AllThenInclude(params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = includeProperty(query);
                }
            }
            return query;
        }

        public virtual IQueryable<T> AllThenInclude(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = includeProperty(query);
                }
            }
            return query.Where(predicate);
        }

        public async Task<IEnumerable<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public async Task<(bool, string)> DeleteCommitWithErrorMessageAsync(T entity)
        {
            _bResult = true;

            try
            {
                Delete(entity);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage);
        }

        public void DeleteRange(IEnumerable<T> entities) => _context.RemoveRange(entities);

        public async Task<(bool, string)> DeleteRangeWithErrorMessageAsync(IEnumerable<T> entities)
        {
            _bResult = true;
            try
            {
                _context.RemoveRange(entities);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage);
        }

        public async Task<(bool, string)> UpdateWithErrorMessageAsync(T entity)
        {
            _bResult = true;
            try
            {
                _context.Entry(entity).CurrentValues.SetValues(entity);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage);
        }

        public void UpdateCommit(T entity)
        {
            _context.Entry(entity).CurrentValues.SetValues(entity);
            _context.SaveChanges();
        }

        public async Task<(bool, string)> UpdateRangeWithErrorMessageAsync(IEnumerable<T> entities)
        {
            _bResult = true;
            try
            {
                _context.Set<T>().UpdateRange(entities);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage);
        }

        public async Task<(bool, string)> UpdateRangeWithErrorMessageAsync(T entity)
        {
            _bResult = true;
            try
            {
                _context.Set<T>().UpdateRange(entity);
                _bResult = await _context.SaveChangesAsync() >= 0;
            }
            catch (Exception err)
            {
                _bResult = false;
                _errorMessage = err.ToString();
            }

            return (_bResult, _errorMessage);
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate) => _context.Set<T>().Where(predicate);

        public async Task<IEnumerable<T>> FindByAndAllIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return await query.Where(predicate).ToListAsync();
        }

        public IEnumerable<T> FindByAndAllIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return query.Where(predicate).ToList();
        }

        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();

        public virtual IQueryable<T> FindByIncludingAsQueryable(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(predicate).AsQueryable();
        }

        public async Task<IEnumerable<T>> GetByAllIncludingAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (include != null) query = include(query);

            return await query.Where(predicate).ToListAsync();
        }

        public IQueryable<TType> GetCustomSelect<TType>(Expression<Func<T, TType>> @select) where TType : class
        {
            return _context.Set<T>().Select(select);
        }
    }
}

