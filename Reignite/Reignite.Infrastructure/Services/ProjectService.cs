using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Infrastructure.Services
{
    public class ProjectService : BaseService<Project, ProjectResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryFilter, int>, IProjectService
    {
        public ProjectService(IRepository<Project, int> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override IQueryable<Project> ApplyFilter(IQueryable<Project> query, ProjectQueryFilter filter)
        {
            query = query.Include(x => x.User).Include(x => x.Hobby).Include(x => x.Product);

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.Title.ToLower().Contains(filter.Search.ToLower()));

            if (filter.UserId.HasValue)
                query = query.Where(x => x.UserId == filter.UserId.Value);

            if (filter.HobbyId.HasValue)
                query = query.Where(x => x.HobbyId == filter.HobbyId.Value);

            if (filter.ProductId.HasValue)
                query = query.Where(x => x.ProductId == filter.ProductId.Value);

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "title" => query.OrderBy(x => x.Title),
                    "createdatdesc" => query.OrderByDescending(x => x.CreatedAt),
                    _ => query.OrderByDescending(x => x.CreatedAt)
                };
                return query;
            }

            query = query.OrderByDescending(x => x.CreatedAt);
            return query;
        }
    }
}
