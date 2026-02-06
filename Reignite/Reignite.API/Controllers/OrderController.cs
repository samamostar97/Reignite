using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IServices;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(Roles = "Admin")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<OrderResponse>>> GetAllPaged([FromQuery] OrderQueryFilter filter)
        {
            var result = await _orderService.GetPagedAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return Ok(order);
        }

        [HttpPost("test")]
        public async Task<ActionResult<OrderResponse>> CreateTestOrder([FromBody] CreateOrderRequest request)
        {
            var order = await _orderService.CreateTestOrderAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}
