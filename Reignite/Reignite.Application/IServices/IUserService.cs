using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IUserService : IService<User, UserResponse, CreateUserRequest, UpdateUserRequest, UserQueryFilter, int>
    {
        Task<UserResponse> CreateWithImageAsync(CreateUserRequest dto, FileUploadRequest? imageRequest);
        Task<UserResponse> UploadImageAsync(int userId, FileUploadRequest fileRequest);
        Task<bool> DeleteImageAsync(int userId);

        // User Address management
        Task<UserAddressResponse?> GetUserAddressAsync(int userId);
        Task<UserAddressResponse> CreateUserAddressAsync(int userId, CreateUserAddressRequest request);
        Task<UserAddressResponse> UpdateUserAddressAsync(int userId, UpdateUserAddressRequest request);
        Task DeleteUserAddressAsync(int userId);
    }
}
