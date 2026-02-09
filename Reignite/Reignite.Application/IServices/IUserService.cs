using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IUserService : IService<User, UserResponse, CreateUserRequest, UpdateUserRequest, UserQueryFilter, int>
    {
        Task<UserResponse> CreateWithImageAsync(CreateUserRequest dto, FileUploadRequest? imageRequest, CancellationToken cancellationToken = default);
        Task<UserResponse> UploadImageAsync(int userId, FileUploadRequest fileRequest, CancellationToken cancellationToken = default);
        Task<bool> DeleteImageAsync(int userId, CancellationToken cancellationToken = default);

        // User Address management
        Task<UserAddressResponse?> GetUserAddressAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserAddressResponse> CreateUserAddressAsync(int userId, CreateUserAddressRequest request, CancellationToken cancellationToken = default);
        Task<UserAddressResponse> UpdateUserAddressAsync(int userId, UpdateUserAddressRequest request, CancellationToken cancellationToken = default);
        Task DeleteUserAddressAsync(int userId, CancellationToken cancellationToken = default);

        // User Hobby management
        Task<List<UserHobbyResponse>> GetUserHobbiesAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserHobbyResponse> AddUserHobbyAsync(int userId, AddUserHobbyRequest request, CancellationToken cancellationToken = default);
        Task DeleteUserHobbyAsync(int userId, int hobbyId, CancellationToken cancellationToken = default);

        // Password management
        Task<bool> VerifyPasswordAsync(int userId, string password, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default);
    }
}
