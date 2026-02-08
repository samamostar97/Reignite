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
        public override Task<ActionResult<PagedResult<FaqResponse>>> GetAllPagedAsync([FromQuery] FaqQueryFilter filter)
            => base.GetAllPagedAsync(filter);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<FaqResponse>> GetById(int id)
            => base.GetById(id);

        // Admin only for create, update, delete
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public override Task<ActionResult<FaqResponse>> Create([FromBody] CreateFaqRequest dto)
            => base.Create(dto);

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public override Task<ActionResult<FaqResponse>> Update(int id, [FromBody] UpdateFaqRequest dto)
            => base.Update(id, dto);

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public override Task<ActionResult> Delete(int id)
            => base.Delete(id);
    }
}
