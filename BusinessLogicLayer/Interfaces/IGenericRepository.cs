using System.Linq.Expressions;

namespace BusinessLogicLayer.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IQueryable<T>> GetAll();
    Task<T> GetById(int id);
    Task<T> FindBy(Expression<Func<T, bool>> criteria);
    Task<IQueryable<T>> FindAllBy(Expression<Func<T, bool>> criteria);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}
