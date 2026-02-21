using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using Reignite.Core.Enums;
using Reignite.Infrastructure.Data;

namespace Reignite.Infrastructure.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ReigniteDbContext _context;
        private readonly IMapper _mapper;

        public CommunityService(ReigniteDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<CommunityUserResponse>> GetPublicUsersAsync(
            int pageNumber = 1, int pageSize = 12, int? hobbyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .Where(u => !u.IsDeleted && u.Role != UserRole.Admin)
                .Include(u => u.UserHobbies).ThenInclude(uh => uh.Hobby)
                .Include(u => u.Projects)
                .AsQueryable();

            if (hobbyId.HasValue)
                query = query.Where(u => u.UserHobbies.Any(uh => uh.HobbyId == hobbyId.Value && !uh.IsDeleted));

            query = query.OrderByDescending(u => u.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = users.Select(u => new CommunityUserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                ProfileImageUrl = u.ProfileImageUrl,
                HobbyNames = u.UserHobbies
                    .Where(uh => !uh.IsDeleted)
                    .Select(uh => uh.Hobby.Name)
                    .ToArray(),
                ProjectCount = u.Projects.Count(p => !p.IsDeleted)
            }).ToList();

            return new PagedResult<CommunityUserResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber
            };
        }

        public async Task<PublicUserProfileResponse> GetPublicProfileAsync(
            int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .Where(u => !u.IsDeleted && u.Role != UserRole.Admin)
                .Include(u => u.UserHobbies).ThenInclude(uh => uh.Hobby)
                .Include(u => u.Projects).ThenInclude(p => p.Hobby)
                .Include(u => u.Projects).ThenInclude(p => p.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            return new PublicUserProfileResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = user.CreatedAt,
                Hobbies = user.UserHobbies
                    .Where(uh => !uh.IsDeleted)
                    .Select(uh => _mapper.Map<UserHobbyResponse>(uh))
                    .ToList(),
                Projects = user.Projects
                    .Where(p => !p.IsDeleted)
                    .Select(p => _mapper.Map<ProjectResponse>(p))
                    .ToList()
            };
        }

        public async Task<PagedResult<ChatMessageResponse>> GetChatMessagesAsync(
            int hobbyId, int pageNumber = 1, int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ChatMessages
                .Where(m => !m.IsDeleted && m.HobbyId == hobbyId)
                .Include(m => m.User)
                .Include(m => m.Hobby);

            var totalCount = await query.CountAsync(cancellationToken);
            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Reverse so oldest messages come first for display
            messages.Reverse();

            return new PagedResult<ChatMessageResponse>
            {
                Items = messages.Select(m => _mapper.Map<ChatMessageResponse>(m)).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber
            };
        }

        public async Task<ChatMessageResponse> SaveMessageAsync(
            int userId, int hobbyId, string content,
            CancellationToken cancellationToken = default)
        {
            var message = new ChatMessage
            {
                UserId = userId,
                HobbyId = hobbyId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync(cancellationToken);

            // Keep only the latest 500 messages per room
            const int maxMessages = 500;
            var count = await _context.ChatMessages
                .CountAsync(m => !m.IsDeleted && m.HobbyId == hobbyId, cancellationToken);

            if (count > maxMessages)
            {
                var oldMessages = await _context.ChatMessages
                    .Where(m => !m.IsDeleted && m.HobbyId == hobbyId)
                    .OrderBy(m => m.CreatedAt)
                    .Take(count - maxMessages)
                    .ToListAsync(cancellationToken);

                _context.ChatMessages.RemoveRange(oldMessages);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Reload with navigation properties for mapping
            var saved = await _context.ChatMessages
                .Include(m => m.User)
                .Include(m => m.Hobby)
                .FirstAsync(m => m.Id == message.Id, cancellationToken);

            return _mapper.Map<ChatMessageResponse>(saved);
        }
    }
}
