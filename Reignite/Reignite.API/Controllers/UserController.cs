using System.IO;
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
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController<User, UserResponse, CreateUserRequest, UpdateUserRequest, UserQueryFilter, int>
    {
        private readonly IUserService _userService;

        public UserController(IUserService service) : base(service)
        {
            _userService = service;
        }

        [HttpPost]
        public override async Task<ActionResult<UserResponse>> Create([FromForm] CreateUserRequest dto)
        {
            var image = Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null;
            FileUploadRequest? fileRequest = null;
            Stream? imageStream = null;

            try
            {
                if (image != null && image.Length > 0)
                {
                    imageStream = image.OpenReadStream();
                    fileRequest = new FileUploadRequest
                    {
                        FileStream = imageStream,
                        FileName = image.FileName,
                        ContentType = image.ContentType,
                        FileSize = image.Length
                    };
                }

                var result = await _userService.CreateWithImageAsync(dto, fileRequest);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            finally
            {
                imageStream?.Dispose();
            }
        }

        // User Address Management
        [HttpGet("{userId}/address")]
        public async Task<ActionResult<UserAddressResponse>> GetUserAddress(int userId)
        {
            var address = await _userService.GetUserAddressAsync(userId);
            if (address == null)
                return NotFound("Korisnik nema adresu.");

            return Ok(address);
        }

        [HttpPost("{userId}/address")]
        public async Task<ActionResult<UserAddressResponse>> CreateUserAddress(int userId, [FromBody] CreateUserAddressRequest request)
        {
            var address = await _userService.CreateUserAddressAsync(userId, request);
            return CreatedAtAction(nameof(GetUserAddress), new { userId }, address);
        }

        [HttpPut("{userId}/address")]
        public async Task<ActionResult<UserAddressResponse>> UpdateUserAddress(int userId, [FromBody] UpdateUserAddressRequest request)
        {
            var address = await _userService.UpdateUserAddressAsync(userId, request);
            return Ok(address);
        }

        [HttpDelete("{userId}/address")]
        public async Task<ActionResult> DeleteUserAddress(int userId)
        {
            await _userService.DeleteUserAddressAsync(userId);
            return NoContent();
        }
    }
}
