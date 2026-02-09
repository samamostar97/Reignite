using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface ISupplierService : IService<Supplier, SupplierResponse, CreateSupplierRequest, UpdateSupplierRequest, SupplierQueryFilter, int>
    {
        Task<List<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
