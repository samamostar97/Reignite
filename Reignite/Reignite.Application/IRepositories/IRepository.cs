using Reignite.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.IRepositories
{
    public interface IRepository<T, TKey>
        where T : class
    {
        Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<PagedResult<T>> GetPagedAsync(IQueryable<T> query, PaginationRequest request, CancellationToken cancellationToken = default);
        IQueryable<T> AsQueryable();
    }
}

