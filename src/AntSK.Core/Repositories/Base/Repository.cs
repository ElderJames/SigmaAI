using AntSK.Domain.Repositories.Base;
using System.Linq.Expressions;

namespace AntSK.Core.Repositories.Base
{
    public class Repository<T> : IRepository<T>
    {
        public int Count(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public bool Delete(dynamic id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T obj)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(dynamic id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public bool DeleteByIds(dynamic[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdsAsync(dynamic[] ids)
        {
            throw new NotImplementedException();
        }

        public T GetById(dynamic id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(dynamic id)
        {
            throw new NotImplementedException();
        }

        public T GetFirst(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public List<T> GetList()
        {
            throw new NotImplementedException();
        }

        public List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public bool Insert(T obj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public bool InsertRange(List<T> objs)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertRangeAsync(List<T> objs)
        {
            throw new NotImplementedException();
        }

        public long InsertReturnBigIdentity(T obj)
        {
            throw new NotImplementedException();
        }

        public Task<long> InsertReturnBigIdentityAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public int InsertReturnIdentity(T obj)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertReturnIdentityAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public bool IsAny(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public bool Update(T obj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRange(List<T> objs)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRangeAsync(List<T> objs)
        {
            throw new NotImplementedException();
        }
    }
}