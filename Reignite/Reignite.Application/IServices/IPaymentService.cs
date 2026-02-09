using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponse> CreatePaymentIntentAsync(List<CreateOrderItemRequest> items, CancellationToken cancellationToken = default);
        Task<bool> VerifyPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    }
}
