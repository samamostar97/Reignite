using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using Stripe;

namespace Reignite.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Core.Entities.Product, int> _productRepository;
        private readonly ICouponService _couponService;

        public PaymentService(IRepository<Core.Entities.Product, int> productRepository, ICouponService couponService)
        {
            _productRepository = productRepository;
            _couponService = couponService;
        }

        public async Task<decimal> CalculateSubtotalAsync(List<CreateOrderItemRequest> items, CancellationToken cancellationToken = default)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.AsQueryable()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Price, cancellationToken);

            var missingProducts = productIds.Where(id => !products.ContainsKey(id)).ToList();
            if (missingProducts.Any())
                throw new KeyNotFoundException($"Proizvodi nisu pronađeni: {string.Join(", ", missingProducts)}");

            return items.Sum(item => item.Quantity * products[item.ProductId]);
        }

        public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(List<CreateOrderItemRequest> items, string? couponCode = null, CancellationToken cancellationToken = default)
        {
            var subtotal = await CalculateSubtotalAsync(items, cancellationToken);
            var totalAmount = subtotal;

            // Apply coupon discount if provided
            if (!string.IsNullOrEmpty(couponCode))
            {
                try
                {
                    var coupon = await _couponService.ValidateCouponAsync(couponCode, subtotal, cancellationToken);

                    decimal discountAmount;
                    if (coupon.DiscountType == "Percentage")
                        discountAmount = subtotal * coupon.DiscountValue / 100;
                    else
                        discountAmount = coupon.DiscountValue;

                    if (discountAmount > subtotal) discountAmount = subtotal;
                    totalAmount = subtotal - discountAmount;
                }
                catch
                {
                    // If coupon validation fails, charge full amount
                }
            }

            if (totalAmount < 0.50m) totalAmount = 0.50m; // Stripe minimum

            // Stripe expects amount in smallest currency unit (cents for EUR)
            var amountInCents = (long)(totalAmount * 100);

            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "eur",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    { "product_ids", string.Join(",", productIds) },
                    { "item_count", items.Count.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options, null, cancellationToken);

            return new PaymentIntentResponse
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id,
                Amount = totalAmount
            };
        }

        public async Task<bool> VerifyPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId, null, null, cancellationToken);
            return paymentIntent.Status == "succeeded";
        }
    }
}
