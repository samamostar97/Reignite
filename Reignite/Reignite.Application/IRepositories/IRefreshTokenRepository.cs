using Reignite.Core.Entities;

namespace Reignite.Application.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task RevokeAsync(RefreshToken token);
    }
}
