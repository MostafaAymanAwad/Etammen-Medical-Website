using BusinessLogicLayer.Interfaces;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{

    public Task<IQueryable<T>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<T> GetById(int id)
    {
        throw new NotImplementedException();
    }
    public Task<T> FindBy(Expression<Func<T, bool>> criteria)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<T>> FindAllBy(Expression<Func<T, bool>> criteria)
    {
        throw new NotImplementedException();
    }


    public void Add(T entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(T entity)
    {
        throw new NotImplementedException();
    }


    public void Update(T entity)
    {
        throw new NotImplementedException();
    }
}
