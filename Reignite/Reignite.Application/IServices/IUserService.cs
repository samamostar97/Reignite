using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IUserService : IService<User, UserResponse, CreateUserRequest, UpdateUserRequest, UserQueryFilter, int>
    {
    }
}
