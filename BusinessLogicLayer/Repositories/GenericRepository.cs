using BusinessLogicLayer.Interfaces;
using System.Linq.Expressions;
using DataAccessLayerEF.Context;
using Microsoft.EntityFrameworkCore;
namespace BusinessLogicLayer.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly EtammenDbContext _context;
    public GenericRepository(EtammenDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<T>> GetAll(string[] includes = null)
    {
        IQueryable<T> query = _context.Set<T>();
        if (includes != null)
            foreach (var include in includes)
                  query = query.Include(include);
        return await query.ToListAsync();
    }

    public async Task<T> FindBy(Expression<Func<T, bool>> criteria, string[] includes = null)
    {
        IQueryable<T> query = _context.Set<T>();
        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);
        return await query.FirstOrDefaultAsync(criteria);
    }
    public async Task<IEnumerable<T>> FindAllBy(Expression<Func<T, bool>> criteria, string[] includes=null)
    {
        IQueryable<T> query = _context.Set<T>();
        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);
        return await query.Where(criteria).ToListAsync();
    }
    public async Task Add(T entity)
    {
       await _context.Set<T>().AddAsync(entity);
    }
    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}
