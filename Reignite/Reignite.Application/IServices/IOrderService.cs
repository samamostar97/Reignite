using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IOrderService
    {
        Task<PagedResult<OrderResponse>> GetPagedAsync(OrderQueryFilter filter);
        Task<OrderResponse> GetByIdAsync(int id);
    }
}
