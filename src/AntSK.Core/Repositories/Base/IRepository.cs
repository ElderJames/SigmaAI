using AntSK.Domain.Domain.Model;
using System.Linq.Expressions;

namespace AntSK.Domain.Repositories.Base
{
    public interface IRepository<T>
    {
        List<T> GetList();
        Task<List<T>> GetListAsync();
        List<T> GetList(Expression<Func<T, bool>> whereExpression);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression);
        int Count(Expression<Func<T, bool>> whereExpression);
        Task<int> CountAsync(Expression<Func<T, bool>> whereExpression);
        T GetById(dynamic id);
        Task<T> GetByIdAsync(dynamic id);
        T GetSingle(Expression<Func<T, bool>> whereExpression);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression);
        T GetFirst(Expression<Func<T, bool>> whereExpression);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression);
        bool Insert(T obj);
        Task<bool> InsertAsync(T obj);
        bool InsertRange(List<T> objs);
        Task<bool> InsertRangeAsync(List<T> objs);
        int InsertReturnIdentity(T obj);
        Task<int> InsertReturnIdentityAsync(T obj);
        long InsertReturnBigIdentity(T obj);
        Task<long> InsertReturnBigIdentityAsync(T obj);
        bool DeleteByIds(dynamic[] ids);
        Task<bool> DeleteByIdsAsync(dynamic[] ids);
        bool Delete(dynamic id);
        Task<bool> DeleteAsync(dynamic id);
        bool Delete(T obj);
        Task<bool> DeleteAsync(T obj);
        bool Delete(Expression<Func<T, bool>> whereExpression);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression);
        bool Update(T obj);
        Task<bool> UpdateAsync(T obj);
        bool UpdateRange(List<T> objs);
        Task<bool> UpdateRangeAsync(List<T> objs);
        bool IsAny(Expression<Func<T, bool>> whereExpression);
        Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression);
    }
}
