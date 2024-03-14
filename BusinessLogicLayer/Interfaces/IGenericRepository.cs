using System.Linq.Expressions;

namespace BusinessLogicLayer.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(string[] includes = null);
    Task<IEnumerable<T>> GetAllWithExpression(Dictionary<Expression<Func<T, object>>, Expression<Func<object, object>>> includes, Expression<Func<T, bool>> criteria);
    Task<T> FindByWithExpression(Expression<Func<T, bool>> criteria, Dictionary<Expression<Func<T, object>>, Expression<Func<object, object>>> includes);
    Task<IEnumerable<T>> FindByWithTwoThenIncludes(Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>> includes);

    Task<T> FindBy(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task<IEnumerable<T>> FindAllBy(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task AddAsync(T entity);
    void Update(T entity);

    Task Delete(int id);
}
