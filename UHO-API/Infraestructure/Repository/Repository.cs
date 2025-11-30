using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UHO_API.Data;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;

namespace UHO_API.Infraestructure.Repository;

public class Repository<T>:IRepository<T> where T:class,ISoftDeletable
{
    private readonly ApplicationDbContext  _context;
    internal DbSet<T> dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        dbSet = _context.Set<T>();
    }
    public async Task<IEnumerable<T>> GetAll(string includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        
        query = IncludeProperties(query,includeProperties);
      
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllBy(Expression<Func<T, bool>> predicate, string includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        
        query = query.Where(predicate);
        
        query = IncludeProperties(query,includeProperties);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<T?> Get(Expression<Func<T, bool>> predicate, string includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        
        query = query.Where(predicate);
        
        query = IncludeProperties(query,includeProperties);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> GetById(int id) =>  await dbSet.FindAsync(id);
    

    public void Delete(T entity) => _context.Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => _context.RemoveRange(entities);

    public async Task SoftDelete(int id)
    {
        var entity = await GetById(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            Update(entity);
        }
    }

    public async Task<IEnumerable<T>> GetPaginatedAsync(int page, int pageSize)
    {
        return await dbSet
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public void Update(T entity) =>  _context.Update(entity);
    public void Add(T entity)
    {
        _context.Add(entity);
    }


    public IQueryable<T> IncludeProperties(IQueryable<T> query, string includeProperties)
    {
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query;
    }
}