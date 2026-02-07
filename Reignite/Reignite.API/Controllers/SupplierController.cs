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
    [Route("api/supplier")]
    [Authorize(Roles = "Admin")]
    public class SupplierController : BaseController<Supplier, SupplierResponse, CreateSupplierRequest, UpdateSupplierRequest, SupplierQueryFilter, int>
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService) : base(supplierService)
        {
            _supplierService = supplierService;
        }

        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<SupplierResponse>>> GetAllPagedAsync([FromQuery] SupplierQueryFilter filter) => base.GetAllPagedAsync(filter);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<SupplierResponse>> GetById(int id) => base.GetById(id);

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<List<SupplierResponse>>> GetAll()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(suppliers);
        }
    }
}
