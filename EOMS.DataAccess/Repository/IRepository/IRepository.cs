using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EOMS.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

        T GetFirstOrDefault(Expression<Func<T, bool>> predicate, string? includeProperties = null);

        T GetT(Expression<Func<T, bool>> filter, string? includeProperties = null);

        IEnumerable<T> GetAllPagedAndSorted(int pageNumber, int pageSize, string sortColumn, bool isAscending, Expression<Func<T, bool>>? predicate = null, string? includeProperties = null);

    }
}
