using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(Roles = "Admin")]
    public class OrderController : BaseController<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest, OrderQueryFilter, int>
    {
        public OrderController(IOrderService service) : base(service)
        {
        }

        // Only expose GET and PUT - no POST (create) or DELETE
        // Orders are created by users through checkout flow (not admin)

        [HttpGet]
        public override Task<ActionResult<PagedResult<OrderResponse>>> GetAllPagedAsync([FromQuery] OrderQueryFilter filter)
            => base.GetAllPagedAsync(filter);

        [HttpGet("{id}")]
        public override Task<ActionResult<OrderResponse>> GetById(int id)
            => base.GetById(id);

        [HttpPut("{id}")]
        public override Task<ActionResult<OrderResponse>> Update(int id, [FromBody] UpdateOrderRequest dto)
            => base.Update(id, dto);

        // Create and Delete endpoints are intentionally not exposed
        // Orders are user-generated, not admin-created
    }
}
