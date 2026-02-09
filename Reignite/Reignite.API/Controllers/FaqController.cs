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
    [Route("api/faqs")]
    public class FaqController : BaseController<Faq, FaqResponse, CreateFaqRequest, UpdateFaqRequest, FaqQueryFilter, int>
    {
        public FaqController(IFaqService service) : base(service)
        {
        }

        // Public can read FAQs
        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<FaqResponse>>> GetAllPagedAsync([FromQuery] FaqQueryFilter filter, CancellationToken cancellationToken = default)
            => base.GetAllPagedAsync(filter, cancellationToken);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<FaqResponse>> GetById(int id, CancellationToken cancellationToken = default)
            => base.GetById(id, cancellationToken);

        // Admin only for create, update, delete
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public override Task<ActionResult<FaqResponse>> Create([FromBody] CreateFaqRequest dto, CancellationToken cancellationToken = default)
            => base.Create(dto, cancellationToken);

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public override Task<ActionResult<FaqResponse>> Update(int id, [FromBody] UpdateFaqRequest dto, CancellationToken cancellationToken = default)
            => base.Update(id, dto, cancellationToken);

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public override Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
            => base.Delete(id, cancellationToken);
    }
}
