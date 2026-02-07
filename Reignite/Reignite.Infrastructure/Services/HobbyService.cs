using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class HobbyService : BaseService<Hobby, HobbyResponse, CreateHobbyRequest, UpdateHobbyRequest, HobbyQueryFilter, int>, IHobbyService
    {
        public HobbyService(IRepository<Hobby, int> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public async Task<List<HobbyResponse>> GetAllAsync()
        {
            var hobbies = await _repository.AsQueryable()
                .OrderBy(h => h.Name)
                .ToListAsync();

            return _mapper.Map<List<HobbyResponse>>(hobbies);
        }

        protected override IQueryable<Hobby> ApplyFilter(IQueryable<Hobby> query, HobbyQueryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.Name.ToLower().Contains(filter.Search.ToLower()));

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "namedesc" => query.OrderByDescending(x => x.Name),
                    "createdatdesc" => query.OrderByDescending(x => x.CreatedAt),
                    _ => query.OrderBy(x => x.Name)
                };
                return query;
            }

            query = query.OrderBy(x => x.Name);
            return query;
        }
    }
}
