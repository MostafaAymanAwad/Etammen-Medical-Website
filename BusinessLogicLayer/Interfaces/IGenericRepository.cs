using System.Linq.Expressions;

namespace BusinessLogicLayer.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(string[] includes = null);
    Task<T> FindBy(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task<IEnumerable<T>> FindAllBy(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task Add(T entity);
    void Update(T entity);

    Task Delete(int id);
}
