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
    public class SupplierService : BaseService<Supplier, SupplierResponse, CreateSupplierRequest, UpdateSupplierRequest, SupplierQueryFilter, int>, ISupplierService
    {
        private readonly IRepository<Product, int> _productRepository;

        public SupplierService(
            IRepository<Supplier, int> repository,
            IRepository<Product, int> productRepository,
            IMapper mapper) : base(repository, mapper)
        {
            _productRepository = productRepository;
        }

        protected override async Task BeforeDeleteAsync(Supplier entity, CancellationToken cancellationToken = default)
        {
            var productCount = await _productRepository.AsQueryable()
                .CountAsync(p => p.SupplierId == entity.Id, cancellationToken);

            if (productCount > 0)
                throw new EntityHasDependentsException("dobavljača", "proizvoda", productCount);
        }

        public async Task<List<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var suppliers = await _repository.AsQueryable()
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<SupplierResponse>>(suppliers);
        }

        protected override async Task BeforeCreateAsync(Supplier entity, CreateSupplierRequest dto, CancellationToken cancellationToken = default)
        {
            var exists = await _repository.AsQueryable()
                .AnyAsync(s => s.Name.ToLower() == dto.Name.ToLower(), cancellationToken);

            if (exists)
                throw new ConflictException($"Dobavljač sa imenom '{dto.Name}' već postoji.");
        }

        protected override async Task BeforeUpdateAsync(Supplier entity, UpdateSupplierRequest dto, CancellationToken cancellationToken = default)
        {
            var exists = await _repository.AsQueryable()
                .AnyAsync(s => s.Id != entity.Id && s.Name.ToLower() == dto.Name.ToLower(), cancellationToken);

            if (exists)
                throw new ConflictException($"Dobavljač sa imenom '{dto.Name}' već postoji.");
        }

        protected override IQueryable<Supplier> ApplyFilter(IQueryable<Supplier> query, SupplierQueryFilter filter)
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
