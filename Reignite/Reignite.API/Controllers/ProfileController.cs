using System.Security.Claims;
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
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IWishlistService _wishlistService;
        private readonly IPaymentService _paymentService;

        public ProfileController(IUserService userService, IOrderService orderService, IWishlistService wishlistService, IPaymentService paymentService)
        {
            _userService = userService;
            _orderService = orderService;
            _wishlistService = wishlistService;
            _paymentService = paymentService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Korisnik nije autentificiran."));

        // GET api/profile
        [HttpGet]
        public async Task<ActionResult<UserResponse>> GetProfile()
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId);
            return Ok(user);
        }

        // PUT api/profile
        [HttpPut]
        public async Task<ActionResult<UserResponse>> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            var userId = GetCurrentUserId();
            request.ProfileImageUrl = null; // Image is handled by separate endpoint
            var user = await _userService.UpdateAsync(userId, request);
            return Ok(user);
        }

        // POST api/profile/image
        [HttpPost("image")]
        public async Task<ActionResult<UserResponse>> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Slika nije proslijeđena.");

            var userId = GetCurrentUserId();

            using var stream = image.OpenReadStream();
            var fileRequest = new FileUploadRequest
            {
                FileStream = stream,
                FileName = image.FileName,
                ContentType = image.ContentType,
                FileSize = image.Length
            };

            var result = await _userService.UploadImageAsync(userId, fileRequest);
            return Ok(result);
        }

        // DELETE api/profile/image
        [HttpDelete("image")]
        public async Task<ActionResult> DeleteImage()
        {
            var userId = GetCurrentUserId();
            await _userService.DeleteImageAsync(userId);
            return NoContent();
        }

        // PUT api/profile/password
        [HttpPut("password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();

            var isValid = await _userService.VerifyPasswordAsync(userId, request.CurrentPassword);
            if (!isValid)
                return BadRequest("Trenutna lozinka nije ispravna.");

            await _userService.ChangePasswordAsync(userId, request.NewPassword);
            return Ok(new { message = "Lozinka uspješno promijenjena." });
        }

        // GET api/profile/address
        [HttpGet("address")]
        public async Task<ActionResult<UserAddressResponse>> GetAddress()
        {
            var userId = GetCurrentUserId();
            var address = await _userService.GetUserAddressAsync(userId);
            if (address == null)
                return NotFound("Nemate dodatu adresu.");

            return Ok(address);
        }

        // POST api/profile/address
        [HttpPost("address")]
        public async Task<ActionResult<UserAddressResponse>> CreateAddress([FromBody] CreateUserAddressRequest request)
        {
            var userId = GetCurrentUserId();
            var address = await _userService.CreateUserAddressAsync(userId, request);
            return Created($"/api/profile/address", address);
        }

        // PUT api/profile/address
        [HttpPut("address")]
        public async Task<ActionResult<UserAddressResponse>> UpdateAddress([FromBody] UpdateUserAddressRequest request)
        {
            var userId = GetCurrentUserId();
            var address = await _userService.UpdateUserAddressAsync(userId, request);
            return Ok(address);
        }

        // DELETE api/profile/address
        [HttpDelete("address")]
        public async Task<ActionResult> DeleteAddress()
        {
            var userId = GetCurrentUserId();
            await _userService.DeleteUserAddressAsync(userId);
            return NoContent();
        }

        // GET api/profile/orders
        [HttpGet("orders")]
        public async Task<ActionResult<PagedResult<OrderResponse>>> GetOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var filter = new OrderQueryFilter
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                UserId = userId
            };
            var orders = await _orderService.GetPagedAsync(filter);
            return Ok(orders);
        }

        // GET api/profile/hobbies
        [HttpGet("hobbies")]
        public async Task<ActionResult<List<UserHobbyResponse>>> GetHobbies()
        {
            var userId = GetCurrentUserId();
            var hobbies = await _userService.GetUserHobbiesAsync(userId);
            return Ok(hobbies);
        }

        // POST api/profile/hobbies
        [HttpPost("hobbies")]
        public async Task<ActionResult<UserHobbyResponse>> AddHobby([FromBody] AddUserHobbyRequest request)
        {
            var userId = GetCurrentUserId();
            var hobby = await _userService.AddUserHobbyAsync(userId, request);
            return Created($"/api/profile/hobbies", hobby);
        }

        // DELETE api/profile/hobbies/{hobbyId}
        [HttpDelete("hobbies/{hobbyId}")]
        public async Task<ActionResult> DeleteHobby(int hobbyId)
        {
            var userId = GetCurrentUserId();
            await _userService.DeleteUserHobbyAsync(userId, hobbyId);
            return NoContent();
        }

        // GET api/profile/wishlist
        [HttpGet("wishlist")]
        public async Task<ActionResult<WishlistResponse>> GetWishlist()
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist ?? new WishlistResponse { UserId = userId, Items = new List<WishlistItemResponse>() });
        }

        // POST api/profile/wishlist/{productId}
        [HttpPost("wishlist/{productId}")]
        public async Task<ActionResult<WishlistItemResponse>> AddToWishlist(int productId)
        {
            var userId = GetCurrentUserId();
            var item = await _wishlistService.AddItemAsync(userId, productId);
            return Created($"/api/profile/wishlist", item);
        }

        // DELETE api/profile/wishlist/{productId}
        [HttpDelete("wishlist/{productId}")]
        public async Task<ActionResult> RemoveFromWishlist(int productId)
        {
            var userId = GetCurrentUserId();
            await _wishlistService.RemoveItemAsync(userId, productId);
            return NoContent();
        }

        // POST api/profile/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderResponse>> Checkout([FromBody] CheckoutRequest request)
        {
            var userId = GetCurrentUserId();

            // Verify Stripe payment before creating order
            var paymentVerified = await _paymentService.VerifyPaymentAsync(request.StripePaymentIntentId);
            if (!paymentVerified)
                return BadRequest("Plaćanje nije uspjelo. Molimo pokušajte ponovo.");

            var createOrderRequest = new CreateOrderRequest
            {
                UserId = userId,
                Items = request.Items,
                StripePaymentIntentId = request.StripePaymentIntentId
            };
            var order = await _orderService.CreateAsync(createOrderRequest);
            return Created($"/api/profile/orders", order);
        }
    }
}
