using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Exceptions;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class HobbyService : BaseService<Hobby, HobbyResponse, CreateHobbyRequest, UpdateHobbyRequest, HobbyQueryFilter, int>, IHobbyService
    {
        private readonly IRepository<Project, int> _projectRepository;

        public HobbyService(
            IRepository<Hobby, int> repository,
            IRepository<Project, int> projectRepository,
            IMapper mapper) : base(repository, mapper)
        {
            _projectRepository = projectRepository;
        }

        protected override async Task BeforeDeleteAsync(Hobby entity)
        {
            var projectCount = await _projectRepository.AsQueryable()
                .CountAsync(p => p.HobbyId == entity.Id);

            if (projectCount > 0)
                throw new EntityHasDependentsException("hobi", "projekata", projectCount);
        }

        public async Task<List<HobbyResponse>> GetAllAsync()
        {
            var hobbies = await _repository.AsQueryable()
                .OrderBy(h => h.Name)
                .ToListAsync();

            return _mapper.Map<List<HobbyResponse>>(hobbies);
        }

        protected override async Task BeforeCreateAsync(Hobby entity, CreateHobbyRequest dto)
        {
            var exists = await _repository.AsQueryable()
                .AnyAsync(h => h.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new ConflictException($"Hobi sa imenom '{dto.Name}' već postoji.");
        }

        protected override async Task BeforeUpdateAsync(Hobby entity, UpdateHobbyRequest dto)
        {
            var exists = await _repository.AsQueryable()
                .AnyAsync(h => h.Id != entity.Id && h.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new ConflictException($"Hobi sa imenom '{dto.Name}' već postoji.");
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
