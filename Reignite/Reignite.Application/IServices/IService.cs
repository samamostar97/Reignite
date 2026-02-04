using Reignite.Application.Common;

namespace Reignite.Application.IServices
{
    public interface IService<T, TDto, TCreateDto, TUpdateDto, TQueryFilter, TKey>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<PagedResult<TDto>> GetPagedAsync(TQueryFilter filter);
        Task<TDto> GetByIdAsync(TKey id);
        Task<TDto> CreateAsync(TCreateDto dto);
        Task<TDto> UpdateAsync(TKey id, TUpdateDto dto);
        Task DeleteAsync(TKey id);
    }
}

