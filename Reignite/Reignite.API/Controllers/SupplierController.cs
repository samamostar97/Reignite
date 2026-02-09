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
    [Route("api/suppliers")]
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
        public override Task<ActionResult<PagedResult<SupplierResponse>>> GetAllPagedAsync([FromQuery] SupplierQueryFilter filter, CancellationToken cancellationToken = default) => base.GetAllPagedAsync(filter, cancellationToken);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<SupplierResponse>> GetById(int id, CancellationToken cancellationToken = default) => base.GetById(id, cancellationToken);

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<List<SupplierResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var suppliers = await _supplierService.GetAllAsync(cancellationToken);
            return Ok(suppliers);
        }
    }
}
