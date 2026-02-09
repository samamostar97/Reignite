using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> RefreshTokenAsync(RefreshRequest request, CancellationToken cancellationToken = default);
    }
}

