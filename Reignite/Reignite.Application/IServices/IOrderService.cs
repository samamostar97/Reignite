using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IOrderService : IService<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest, OrderQueryFilter, int>
    {
    }
}
