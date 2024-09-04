using System.Linq.Expressions;

namespace Refactored.Persistence.Data.Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddRangeAsync(IEnumerable<T> entities);

        Task<(T, bool result)> AddCommitAsync(T entity);

        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();

        void DeleteRange(IEnumerable<T> entities);

        Task<(bool, string)> DeleteRangeWithErrorMessageAsync(IEnumerable<T> entities);

        Task<(bool, string)> AddRangeWithErrorMessageAsync(IEnumerable<T> entities);
    }
}

