using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshRequest request);
    }
}

