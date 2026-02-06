using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Exceptions;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Application.Options;
using Reignite.Core.Entities;
using Reignite.Infrastructure.Data;

namespace Reignite.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ReigniteDbContext _context;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            ReigniteDbContext context,
            IRefreshTokenRepository refreshTokenRepository,
            IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var emailExists = await _context.Users.AnyAsync(x => x.Email.ToLower() == request.Email.ToLower());
            if (emailExists) throw new ConflictException("Email je zauzet");
            var usernameExists = await _context.Users.AnyAsync(x => x.Username.ToLower() == request.Username.ToLower());
            if (usernameExists) throw new ConflictException("Username je zauzet");
            var phoneNumberExists = await _context.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (phoneNumberExists) throw new ConflictException("Broj telefona je zauzet");

            // Create new user
            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Username = request.Username,
                CreatedAt = DateTime.UtcNow,
                Role = Core.Enums.UserRole.AppUser
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate tokens
            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Neispravan email ili lozinka.");
            }

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshRequest request)
        {
            var refreshTokenHash = HashRefreshToken(request.RefreshToken);
            var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash);

            // Fallback for unhashed tokens (migration support)
            if (refreshToken == null)
            {
                refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(request.RefreshToken);
            }

            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("Neispravan refresh token.");
            }

            if (!refreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Refresh token je istekao ili je ponisten.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _refreshTokenRepository.RevokeAsync(refreshToken);
            var response = await GenerateAuthResponse(refreshToken.User);

            await transaction.CommitAsync();

            return response;
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var rawRefreshToken = GenerateRefreshTokenValue();

            await SaveRefreshTokenAsync(user.Id, rawRefreshToken);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenExpirationHours),
                User = new UserAuthResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role
                }
            };
        }

        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenExpirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshTokenValue()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string HashRefreshToken(string token)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }

        private async Task SaveRefreshTokenAsync(int userId, string rawToken)
        {
            var refreshToken = new RefreshToken
            {
                Token = HashRefreshToken(rawToken),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
        }
    }
}
