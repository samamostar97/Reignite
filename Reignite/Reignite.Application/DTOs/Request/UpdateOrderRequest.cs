using Reignite.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class UpdateOrderRequest
    {
        [Required(ErrorMessage = "Status je obavezan.")]
        public OrderStatus Status { get; set; }
    }
}
