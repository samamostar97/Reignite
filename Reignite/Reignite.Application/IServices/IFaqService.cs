using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IFaqService : IService<Faq, FaqResponse, CreateFaqRequest, UpdateFaqRequest, FaqQueryFilter, int>
    {
    }
}
