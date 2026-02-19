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
        private readonly ICouponService _couponService;

        public ProfileController(IUserService userService, IOrderService orderService, IWishlistService wishlistService, IPaymentService paymentService, ICouponService couponService)
        {
            _userService = userService;
            _orderService = orderService;
            _wishlistService = wishlistService;
            _paymentService = paymentService;
            _couponService = couponService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Korisnik nije autentificiran."));

        // GET api/profile
        [HttpGet]
        public async Task<ActionResult<UserResponse>> GetProfile(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            return Ok(user);
        }

        // PUT api/profile
        [HttpPut]
        public async Task<ActionResult<UserResponse>> UpdateProfile([FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            request.ProfileImageUrl = null; // Image is handled by separate endpoint
            var user = await _userService.UpdateAsync(userId, request, cancellationToken);
            return Ok(user);
        }

        // POST api/profile/image
        [HttpPost("image")]
        public async Task<ActionResult<UserResponse>> UploadImage(IFormFile image, CancellationToken cancellationToken = default)
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

            var result = await _userService.UploadImageAsync(userId, fileRequest, cancellationToken);
            return Ok(result);
        }

        // DELETE api/profile/image
        [HttpDelete("image")]
        public async Task<ActionResult> DeleteImage(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _userService.DeleteImageAsync(userId, cancellationToken);
            return NoContent();
        }

        // PUT api/profile/password
        [HttpPut("password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var isValid = await _userService.VerifyPasswordAsync(userId, request.CurrentPassword, cancellationToken);
            if (!isValid)
                return BadRequest("Trenutna lozinka nije ispravna.");

            await _userService.ChangePasswordAsync(userId, request.NewPassword, cancellationToken);
            return Ok(new { message = "Lozinka uspješno promijenjena." });
        }

        // GET api/profile/address
        [HttpGet("address")]
        public async Task<ActionResult<UserAddressResponse>> GetAddress(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var address = await _userService.GetUserAddressAsync(userId, cancellationToken);
            if (address == null)
                return NotFound("Nemate dodatu adresu.");

            return Ok(address);
        }

        // POST api/profile/address
        [HttpPost("address")]
        public async Task<ActionResult<UserAddressResponse>> CreateAddress([FromBody] CreateUserAddressRequest request, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var address = await _userService.CreateUserAddressAsync(userId, request, cancellationToken);
            return Created($"/api/profile/address", address);
        }

        // PUT api/profile/address
        [HttpPut("address")]
        public async Task<ActionResult<UserAddressResponse>> UpdateAddress([FromBody] UpdateUserAddressRequest request, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var address = await _userService.UpdateUserAddressAsync(userId, request, cancellationToken);
            return Ok(address);
        }

        // DELETE api/profile/address
        [HttpDelete("address")]
        public async Task<ActionResult> DeleteAddress(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _userService.DeleteUserAddressAsync(userId, cancellationToken);
            return NoContent();
        }

        // GET api/profile/orders
        [HttpGet("orders")]
        public async Task<ActionResult<PagedResult<OrderResponse>>> GetOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var filter = new OrderQueryFilter
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                UserId = userId
            };
            var orders = await _orderService.GetPagedAsync(filter, cancellationToken);
            return Ok(orders);
        }

        // GET api/profile/hobbies
        [HttpGet("hobbies")]
        public async Task<ActionResult<List<UserHobbyResponse>>> GetHobbies(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var hobbies = await _userService.GetUserHobbiesAsync(userId, cancellationToken);
            return Ok(hobbies);
        }

        // POST api/profile/hobbies
        [HttpPost("hobbies")]
        public async Task<ActionResult<UserHobbyResponse>> AddHobby([FromBody] AddUserHobbyRequest request, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var hobby = await _userService.AddUserHobbyAsync(userId, request, cancellationToken);
            return Created($"/api/profile/hobbies", hobby);
        }

        // DELETE api/profile/hobbies/{hobbyId}
        [HttpDelete("hobbies/{hobbyId}")]
        public async Task<ActionResult> DeleteHobby(int hobbyId, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _userService.DeleteUserHobbyAsync(userId, hobbyId, cancellationToken);
            return NoContent();
        }

        // GET api/profile/wishlist
        [HttpGet("wishlist")]
        public async Task<ActionResult<WishlistResponse>> GetWishlist(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId, cancellationToken);
            return Ok(wishlist ?? new WishlistResponse { UserId = userId, Items = new List<WishlistItemResponse>() });
        }

        // POST api/profile/wishlist/{productId}
        [HttpPost("wishlist/{productId}")]
        public async Task<ActionResult<WishlistItemResponse>> AddToWishlist(int productId, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var item = await _wishlistService.AddItemAsync(userId, productId, cancellationToken);
            return Created($"/api/profile/wishlist", item);
        }

        // DELETE api/profile/wishlist/{productId}
        [HttpDelete("wishlist/{productId}")]
        public async Task<ActionResult> RemoveFromWishlist(int productId, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            await _wishlistService.RemoveItemAsync(userId, productId, cancellationToken);
            return NoContent();
        }

        // POST api/profile/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderResponse>> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            // Verify Stripe payment before creating order
            var paymentVerified = await _paymentService.VerifyPaymentAsync(request.StripePaymentIntentId, cancellationToken);
            if (!paymentVerified)
                return BadRequest("Plaćanje nije uspjelo. Molimo pokušajte ponovo.");

            // Calculate discount if coupon is provided
            decimal discountAmount = 0;
            if (!string.IsNullOrEmpty(request.CouponCode))
            {
                try
                {
                    var coupon = await _couponService.ValidateCouponAsync(request.CouponCode, 0, cancellationToken);

                    // Calculate subtotal from items to determine discount
                    var subtotal = await _paymentService.CalculateSubtotalAsync(request.Items, cancellationToken);

                    if (coupon.DiscountType == "Percentage")
                        discountAmount = subtotal * coupon.DiscountValue / 100;
                    else
                        discountAmount = coupon.DiscountValue;

                    if (discountAmount > subtotal) discountAmount = subtotal;

                    // Increment coupon usage
                    await _couponService.IncrementUsageAsync(request.CouponCode, cancellationToken);
                }
                catch
                {
                    // If coupon validation fails at checkout, proceed without discount
                    discountAmount = 0;
                    request.CouponCode = null;
                }
            }

            var createOrderRequest = new CreateOrderRequest
            {
                UserId = userId,
                Items = request.Items,
                StripePaymentIntentId = request.StripePaymentIntentId,
                CouponCode = request.CouponCode,
                DiscountAmount = discountAmount
            };
            var order = await _orderService.CreateAsync(createOrderRequest, cancellationToken);
            return Created($"/api/profile/orders", order);
        }
    }
}
