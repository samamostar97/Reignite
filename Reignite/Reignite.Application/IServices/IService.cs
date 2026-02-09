using Reignite.Application.Common;

namespace Reignite.Application.IServices
{
    public interface IService<T, TDto, TCreateDto, TUpdateDto, TQueryFilter, TKey>
    {
        Task<PagedResult<TDto>> GetPagedAsync(TQueryFilter filter, CancellationToken cancellationToken = default);
        Task<TDto> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default);
        Task<TDto> UpdateAsync(TKey id, TUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    }
}

