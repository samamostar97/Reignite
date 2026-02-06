using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IHobbyService
    {
        Task<List<HobbyResponse>> GetAllAsync();
    }
}
