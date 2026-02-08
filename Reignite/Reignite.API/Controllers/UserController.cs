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
        private readonly IWishlistService _wishlistService;

        public UserController(IUserService service, IWishlistService wishlistService) : base(service)
        {
            _userService = service;
            _wishlistService = wishlistService;
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

        // User Wishlist (Read-only)
        [HttpGet("{userId}/wishlist")]
        public async Task<ActionResult<WishlistResponse>> GetUserWishlist(int userId)
        {
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            if (wishlist == null)
                return Ok(new WishlistResponse { UserId = userId, Items = new List<WishlistItemResponse>() });

            return Ok(wishlist);
        }

        // User Hobby Management
        [HttpGet("{userId}/hobbies")]
        public async Task<ActionResult<List<UserHobbyResponse>>> GetUserHobbies(int userId)
        {
            var hobbies = await _userService.GetUserHobbiesAsync(userId);
            return Ok(hobbies);
        }

        [HttpPost("{userId}/hobbies")]
        public async Task<ActionResult<UserHobbyResponse>> AddUserHobby(int userId, [FromBody] AddUserHobbyRequest request)
        {
            var hobby = await _userService.AddUserHobbyAsync(userId, request);
            return CreatedAtAction(nameof(GetUserHobbies), new { userId }, hobby);
        }

        [HttpDelete("{userId}/hobbies/{hobbyId}")]
        public async Task<ActionResult> DeleteUserHobby(int userId, int hobbyId)
        {
            await _userService.DeleteUserHobbyAsync(userId, hobbyId);
            return NoContent();
        }
    }
}
