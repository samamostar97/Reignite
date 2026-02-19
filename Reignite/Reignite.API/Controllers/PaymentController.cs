using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IServices;
using Reignite.Application.Options;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly StripeSettings _stripeSettings;

        public PaymentController(IPaymentService paymentService, IOptions<StripeSettings> stripeSettings)
        {
            _paymentService = paymentService;
            _stripeSettings = stripeSettings.Value;
        }

        // GET api/payment/config
        [HttpGet("config")]
        public ActionResult GetConfig()
        {
            return Ok(new { publishableKey = _stripeSettings.PublishableKey });
        }

        // POST api/payment/create-intent
        [HttpPost("create-intent")]
        public async Task<ActionResult<PaymentIntentResponse>> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _paymentService.CreatePaymentIntentAsync(request.Items, request.CouponCode, cancellationToken);
            return Ok(result);
        }
    }
}
