using System.Linq.Expressions;

namespace PermissionsApp.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithIncludeAsync<TProperty>(Expression<Func<T, TProperty>> includeProperty);

        Task<T?> GetByIdWithIncludeAsync<TProperty>(int id, Expression<Func<T, TProperty>> includeProperty);
        
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
