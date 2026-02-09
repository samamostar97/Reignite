using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IHobbyService : IService<Hobby, HobbyResponse, CreateHobbyRequest, UpdateHobbyRequest, HobbyQueryFilter, int>
    {
        Task<List<HobbyResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
