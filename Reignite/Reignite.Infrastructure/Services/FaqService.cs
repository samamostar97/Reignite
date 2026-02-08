using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class FaqService : BaseService<Faq, FaqResponse, CreateFaqRequest, UpdateFaqRequest, FaqQueryFilter, int>, IFaqService
    {
        public FaqService(IRepository<Faq, int> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override IQueryable<Faq> ApplyFilter(IQueryable<Faq> query, FaqQueryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                query = query.Where(f =>
                    f.Question.ToLower().Contains(searchLower) ||
                    f.Answer.ToLower().Contains(searchLower));
            }

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "question" => query.OrderBy(f => f.Question),
                    "questiondesc" => query.OrderByDescending(f => f.Question),
                    _ => query.OrderByDescending(f => f.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(f => f.CreatedAt);
            }

            return query;
        }
    }
}
