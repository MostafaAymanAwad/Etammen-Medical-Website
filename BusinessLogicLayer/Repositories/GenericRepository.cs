using BusinessLogicLayer.Interfaces;
using System.Linq.Expressions;
using DataAccessLayerEF.Context;
using Microsoft.EntityFrameworkCore;
using DataAccessLayerEF.Models;
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

    public async Task<IEnumerable<T>> GetAllWithExpression(Dictionary<Expression<Func<T, object>>, Expression<Func<object, object>>> includes, Expression<Func<T, bool>> criteria)
    {
        IQueryable<T> query = _context.Set<T>();
        foreach (var pair in includes)
        {
            query = query.Include(pair.Key).ThenInclude(pair.Value);
        }
        query = query.Where(criteria);
        return await query.ToListAsync();
    }


    public async Task<T> FindBy(Expression<Func<T, bool>> criteria, string[] includes = null)
    {
        IQueryable<T> query = _context.Set<T>();
        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);
       var entity =  await query.FirstOrDefaultAsync(criteria);
        return entity;

    }
    public async Task<T> FindByWithExpression(Expression<Func<T, bool>> criteria, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>> includes)
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes != null)
        {
            foreach (var pair in includes)
            {
                var includeQuery = query.Include(pair.Key);


                foreach (var thenIncludeExpression in pair.Value)
                {
                    includeQuery = includeQuery.ThenInclude(thenIncludeExpression);
                }


                query = includeQuery;
            }
        }


        var entity = await query.FirstOrDefaultAsync(criteria);
        return entity;

    }
    public async Task<IEnumerable<T>> FindByWithTwoThenIncludes(Expression<Func<T, bool>> criteria, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>> includes)
    {
        IQueryable<T> query = _context.Set<T>();
        if (includes != null)
        {
            foreach (var pair in includes)
            {

                var includeQuery = query.Include(pair.Key);


                foreach (var thenIncludeExpression in pair.Value)
                {
                    includeQuery = includeQuery.ThenInclude(thenIncludeExpression);
                }


                query = includeQuery;
            }
        }
        return await query.Where(criteria).ToListAsync();
    }
        public async Task<IEnumerable<T>> FindAllBy(Expression<Func<T, bool>> criteria, string[] includes = null)
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
    public async Task Delete(int id, bool isHardDeleted)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (isHardDeleted)
            _context.Set<T>().Remove(entity);
        else
            _context.Set<T>().Update(entity);
    }
    public async Task Delete(int id) 
    { 
        var entity = await _context.Set<T>().FindAsync(id);

       _context.Update(entity);
    }
}
