using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class UserService : BaseService<User, UserResponse, CreateUserRequest, UpdateUserRequest, UserQueryFilter, int>, IUserService
    {
        public UserService(IRepository<User, int> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public override async Task<UserResponse> CreateAsync(CreateUserRequest dto)
        {
            // Check if email already exists
            var existingUser = await _repository.AsQueryable()
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new InvalidOperationException("Korisnik sa ovim emailom već postoji.");

            // Check if username already exists
            existingUser = await _repository.AsQueryable()
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (existingUser != null)
                throw new InvalidOperationException("Korisničko ime je već zauzeto.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repository.AddAsync(user);

            return await GetByIdWithCountsAsync(user.Id);
        }

        public override async Task<UserResponse> GetByIdAsync(int id)
        {
            return await GetByIdWithCountsAsync(id);
        }

        protected override IQueryable<User> ApplyFilter(IQueryable<User> query, UserQueryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower) ||
                    u.Username.ToLower().Contains(searchLower));
            }

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "name" => query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                    "namedesc" => query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName),
                    "email" => query.OrderBy(u => u.Email),
                    "createdat" => query.OrderBy(u => u.CreatedAt),
                    "createdatdesc" => query.OrderByDescending(u => u.CreatedAt),
                    _ => query.OrderByDescending(u => u.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(u => u.CreatedAt);
            }

            return query;
        }

        private async Task<UserResponse> GetByIdWithCountsAsync(int id)
        {
            var user = await _repository.AsQueryable()
                .Include(u => u.Orders)
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            var response = _mapper.Map<UserResponse>(user);
            response.OrderCount = user.Orders.Count;
            response.ProjectCount = user.Projects.Count;

            return response;
        }
    }
}
