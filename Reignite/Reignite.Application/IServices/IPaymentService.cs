using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponse> CreatePaymentIntentAsync(List<CreateOrderItemRequest> items);
        Task<bool> VerifyPaymentAsync(string paymentIntentId);
    }
}
