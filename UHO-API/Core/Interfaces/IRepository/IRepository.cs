using System.Linq.Expressions;

namespace UHO_API.Core.Interfaces.IRepository;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(string includeProperties = null);
    Task<IEnumerable<T>> GetAllBy(Expression<Func<T, bool>> predicate,string includeProperties = null);
    Task<T?> Get(Expression<Func<T,bool>> predicate , string includeProperties = null);
    Task<T?> GetById(int id);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task SoftDelete(int id);
   // Task<T?> GetActiveById(int id,string includeProperties = null);
    Task<IEnumerable<T>> GetAllActive(string includeProperties = null);
    Task<T> GetActiveBy(Expression<Func<T,bool>> predicate ,string includeProperties = null);
    Task<T?> GetActiveById(int id,string includeProperties = null);
    Task<IEnumerable<T>> GetDeleted(string includeProperties = null);
    Task<IEnumerable<T>> GetPaginatedAsync(int page, int pageSize);
    void Update(T entity);
    
    void Add(T entity);
}