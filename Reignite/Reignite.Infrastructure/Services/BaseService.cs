using Reignite.Application.Common;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Infrastructure.Services
{
    public class BaseService<T, TDto, TCreateDto, TUpdateDto, TQueryFilter, TKey> : IService<T, TDto, TCreateDto, TUpdateDto, TQueryFilter, TKey>
            where T : class
            where TQueryFilter : PaginationRequest
    {
        protected readonly IRepository<T, TKey> _repository;
        protected readonly IMapper _mapper;
        public BaseService(IRepository<T, TKey> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public virtual async Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default)
        {
            var result = _mapper.Map<T>(dto!);
            await BeforeCreateAsync(result, dto, cancellationToken);
            await _repository.AddAsync(result, cancellationToken);
            await AfterCreateAsync(result, dto, cancellationToken);
            return _mapper.Map<TDto>(result);
        }

        public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            // Repository throws KeyNotFoundException if not found

            await BeforeDeleteAsync(entity, cancellationToken);
            await _repository.DeleteAsync(entity, cancellationToken);
            await AfterDeleteAsync(entity, cancellationToken);
        }


        public virtual async Task<TDto> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            // Repository throws KeyNotFoundException if not found
            await AfterGetAsync(entity, cancellationToken);
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<PagedResult<TDto>> GetPagedAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _repository.AsQueryable();
            query = ApplyFilter(query, filter);
            await BeforePagedAsync(query, cancellationToken);

            var pagedEntities = await _repository.GetPagedAsync(query, filter, cancellationToken);

            await AfterPagedAsync(pagedEntities.Items, cancellationToken);

            return new PagedResult<TDto>
            {
                Items = _mapper.Map<List<TDto>>(pagedEntities.Items),
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
            };
        }

        public virtual async Task<TDto> UpdateAsync(TKey id, TUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            // Repository throws KeyNotFoundException if not found

            await BeforeUpdateAsync(entity, dto, cancellationToken);
            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity, cancellationToken);
            await AfterUpdateAsync(entity, dto, cancellationToken);

            return _mapper.Map<TDto>(entity);
        }
        //hook methods
        protected virtual IQueryable<T> ApplyFilter(IQueryable<T> query, TQueryFilter filter)
        {
            return query;
        }
        protected virtual Task BeforeCreateAsync(T entity, TCreateDto dto, CancellationToken cancellationToken = default) => Task.CompletedTask;
        protected virtual Task AfterCreateAsync(T entity, TCreateDto dto, CancellationToken cancellationToken = default) => Task.CompletedTask;


        protected virtual Task BeforeUpdateAsync(T entity, TUpdateDto dto, CancellationToken cancellationToken = default) => Task.CompletedTask;
        protected virtual Task AfterUpdateAsync(T entity, TUpdateDto dto, CancellationToken cancellationToken = default) => Task.CompletedTask;

        protected virtual Task BeforeDeleteAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        protected virtual Task AfterDeleteAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        protected virtual Task BeforePagedAsync(IQueryable<T> query, CancellationToken cancellationToken = default) => Task.CompletedTask;
        protected virtual Task AfterPagedAsync(List<T> entities, CancellationToken cancellationToken = default) => Task.CompletedTask;

        protected virtual Task BeforeGetAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        protected virtual Task AfterGetAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}

