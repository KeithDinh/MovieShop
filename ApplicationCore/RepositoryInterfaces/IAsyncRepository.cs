using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace ApplicationCore.RepositoryInterfaces
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<ICollection<T>> ListAllAsync();
        Task<ICollection<T>> ListAsync(Expression<Func<T, bool>> filter);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
        Task<int> GetCountAsync(Expression<Func<T, bool>> filter = null);
    }
}
