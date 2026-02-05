using Reignite.Application.Common;
using Reignite.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Reignite.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Reignite.API.Controllers
{
    [ApiController]
    public class BaseController<T, TDto, TCreateDto, TUpdateDto, TQueryFilter, TKey> : ControllerBase
        where T : class
    {
        protected readonly IService<T, TDto, TCreateDto, TUpdateDto, TQueryFilter, TKey> _service;
        public BaseController(IService<T, TDto, TCreateDto, TUpdateDto, TQueryFilter, TKey> service)
        {
            _service = service;
        }
        [HttpGet]
        public virtual async Task<ActionResult<PagedResult<TDto>>> GetAllPagedAsync([FromQuery] TQueryFilter filter)
        {
            var list = await _service.GetPagedAsync(filter);
            return Ok(list);
        }
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(TKey id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);

            var idProp = result!.GetType().GetProperty("Id");
            var id = idProp?.GetValue(result);

            return CreatedAtAction(nameof(GetById), new { id }, result);
        }
        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TDto>> Update(TKey id, [FromBody] TUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult> Delete(TKey id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
