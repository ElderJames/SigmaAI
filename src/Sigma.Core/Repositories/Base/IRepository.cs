using AntSK.Core.Repositories.Base;
using System.Linq.Expressions;

namespace AntSK.Domain.Repositories.Base
{
    public interface IRepository<T> where T : EntityBase
    {
        List<T> GetList();

        Task<List<T>> GetListAsync();

        List<T> GetList(Expression<Func<T, bool>> whereExpression);

        Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression);

        Task<int> CountAsync(Expression<Func<T, bool>> whereExpression);

        T GetFirst(Expression<Func<T, bool>> whereExpression);

        Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression);

        bool Insert(T obj);

        Task<bool> DeleteAsync(string id);

        Task<bool> DeleteAsync(T obj);

        Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression);

        bool Update(T obj);

        bool IsAny(Expression<Func<T, bool>> whereExpression);
    }
}