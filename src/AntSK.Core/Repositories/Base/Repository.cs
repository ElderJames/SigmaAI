using AntSK.Domain.Repositories.Base;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace AntSK.Core.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        private DbContext _db;

        public Repository(DbContext db)
        {
            _db = db;
        }

        public int Count(Expression<Func<T, bool>> whereExpression)
        {
            return _db.Set<T>().Count(whereExpression);
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> whereExpression)
        {
            return _db.Set<T>().CountAsync(whereExpression);
        }

        public bool Delete(string id)
        {
            return _db.Set<T>().Where(x => x.Id == id).ExecuteDelete() > 0;
        }

        public bool Delete(T obj)
        {
            _db.Set<T>().Remove(obj);
            return _db.SaveChanges() > 0;
        }

        public bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return _db.Set<T>().Where(whereExpression).ExecuteDelete() > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return (await _db.Set<T>().Where(x => x.Id == id).ExecuteDeleteAsync()) > 0;
        }

        public async Task<bool> DeleteAsync(T obj)
        {
            _db.Set<T>().Remove(obj);
            return (await _db.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            return (await _db.Set<T>().Where(whereExpression).ExecuteDeleteAsync()) > 0;
        }

        public bool DeleteByIds(string[] ids)
        {
            return (_db.Set<T>().Where(x => ids.Contains(x.Id)).ExecuteDelete()) > 0;
        }

        public async Task<bool> DeleteByIdsAsync(string[] ids)
        {
            return (await _db.Set<T>().Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync()) > 0;
        }

        public T? GetById(int id)
        {
            return _db.Set<T>().Find(id);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public T GetFirst(Expression<Func<T, bool>> whereExpression)
        {
            return _db.Set<T>().Where(whereExpression).First();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _db.Set<T>().Where(whereExpression).FirstAsync();
        }

        public List<T> GetList()
        {
            return _db.Set<T>().ToList();
        }

        public List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return _db.Set<T>().Where(whereExpression).ToList();
        }

        public async Task<List<T>> GetListAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _db.Set<T>().Where(whereExpression).ToListAsync();
        }

        public bool Insert(T obj)
        {
            _db.Set<T>().Add(obj);
            return _db.SaveChanges() > 0;
        }

        public bool IsAny(Expression<Func<T, bool>> whereExpression)
        {
            return _db.Set<T>().Any(whereExpression);
        }

        public bool Update(T obj)
        {
            _db.Update(obj);
            return _db.SaveChanges() > 0;
        }
    }
}