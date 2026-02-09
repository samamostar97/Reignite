using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Exceptions;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class UserService : BaseService<User, UserResponse, CreateUserRequest, UpdateUserRequest, UserQueryFilter, int>, IUserService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IRepository<UserAddress, int> _userAddressRepository;
        private readonly IRepository<UserHobby, int> _userHobbyRepository;

        public UserService(
            IRepository<User, int> repository,
            IMapper mapper,
            IFileStorageService fileStorageService,
            IRepository<UserAddress, int> userAddressRepository,
            IRepository<UserHobby, int> userHobbyRepository) : base(repository, mapper)
        {
            _fileStorageService = fileStorageService;
            _userAddressRepository = userAddressRepository;
            _userHobbyRepository = userHobbyRepository;
        }
        protected override async Task BeforeCreateAsync(User entity, CreateUserRequest dto, CancellationToken cancellationToken = default)
        {
            await ValidateUniqueFieldsAsync(dto.Email, dto.Username, dto.PhoneNumber, excludeUserId: null, cancellationToken);
        }

        protected override async Task BeforeUpdateAsync(User entity, UpdateUserRequest dto, CancellationToken cancellationToken = default)
        {
            await ValidateUniqueFieldsAsync(dto.Email, dto.Username, dto.PhoneNumber, excludeUserId: entity.Id, cancellationToken);
        }

        private async Task ValidateUniqueFieldsAsync(string email, string username, string phoneNumber, int? excludeUserId, CancellationToken cancellationToken = default)
        {
            var emailLower = email.ToLower();
            var usernameLower = username.ToLower();
            var phoneLower = phoneNumber.ToLower();

            var query = _repository.AsQueryable().AsNoTracking();

            if (excludeUserId.HasValue)
                query = query.Where(x => x.Id != excludeUserId.Value);

            var emailExists = await query.AnyAsync(x => x.Email.ToLower() == emailLower, cancellationToken);
            if (emailExists) throw new ConflictException("Korisnik sa ovim emailom vec postoji");

            var usernameExists = await query.AnyAsync(x => x.Username.ToLower() == usernameLower, cancellationToken);
            if (usernameExists) throw new ConflictException("Korisnik sa ovim usernameom vec postoji");

            var phoneNumberExists = await query.AnyAsync(x => x.PhoneNumber.ToLower() == phoneLower, cancellationToken);
            if (phoneNumberExists) throw new ConflictException("Korisnik sa ovim brojem telefona vec postoji");
        }

        protected override async Task BeforeDeleteAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(entity.ProfileImageUrl))
                await _fileStorageService.DeleteAsync(entity.ProfileImageUrl, cancellationToken);
        }
        public override async Task<UserResponse> CreateAsync(CreateUserRequest dto, CancellationToken cancellationToken = default)
        {
            var user = _mapper.Map<User>(dto);
            await BeforeCreateAsync(user, dto, cancellationToken);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repository.AddAsync(user, cancellationToken);

            return await GetByIdWithCountsAsync(user.Id, cancellationToken);
        }

        public async Task<UserResponse> CreateWithImageAsync(CreateUserRequest dto, FileUploadRequest? imageRequest, CancellationToken cancellationToken = default)
        {
            var user = _mapper.Map<User>(dto);
            await BeforeCreateAsync(user, dto, cancellationToken);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repository.AddAsync(user, cancellationToken);

            if (imageRequest != null && imageRequest.FileStream != null && imageRequest.FileSize > 0)
            {
                var uploadResult = await _fileStorageService.UploadAsync(imageRequest, "users", user.Id.ToString(), cancellationToken);

                if (uploadResult.Success)
                {
                    user.ProfileImageUrl = uploadResult.FileUrl;
                    await _repository.UpdateAsync(user, cancellationToken);
                }
            }

            return await GetByIdWithCountsAsync(user.Id, cancellationToken);
        }

        public override async Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetByIdWithCountsAsync(id, cancellationToken);
        }

        protected override IQueryable<User> ApplyFilter(IQueryable<User> query, UserQueryFilter filter)
        {
            // Exclude admins from the list
            query = query.Where(u => u.Role != Core.Enums.UserRole.Admin);

            // Include related data for counts (but not OrderItems - not needed for count)
            query = query.Include(x => x.Orders).Include(x => x.Projects);

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

        private async Task<UserResponse> GetByIdWithCountsAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _repository.AsQueryable()
                .Include(u => u.Orders)
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            var response = _mapper.Map<UserResponse>(user);
            response.OrderCount = user.Orders.Count;
            response.ProjectCount = user.Projects.Count;

            return response;
        }

        public async Task<UserResponse> UploadImageAsync(int userId, FileUploadRequest fileRequest, CancellationToken cancellationToken = default)
        {
            var user = await _repository.AsQueryable()
                .Include(u => u.Orders)
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                await _fileStorageService.DeleteAsync(user.ProfileImageUrl, cancellationToken);
            }

            var uploadResult = await _fileStorageService.UploadAsync(fileRequest, "users", userId.ToString(), cancellationToken);

            if (!uploadResult.Success)
                throw new InvalidOperationException(uploadResult.ErrorMessage);

            user.ProfileImageUrl = uploadResult.FileUrl;
            await _repository.UpdateAsync(user, cancellationToken);

            var response = _mapper.Map<UserResponse>(user);
            response.OrderCount = user.Orders.Count;
            response.ProjectCount = user.Projects.Count;

            return response;
        }

        public async Task<bool> DeleteImageAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (string.IsNullOrEmpty(user.ProfileImageUrl))
                return false;

            var deleted = await _fileStorageService.DeleteAsync(user.ProfileImageUrl, cancellationToken);

            user.ProfileImageUrl = null;
            await _repository.UpdateAsync(user, cancellationToken);

            return deleted;
        }

        // User Address Management
        public async Task<UserAddressResponse?> GetUserAddressAsync(int userId, CancellationToken cancellationToken = default)
        {
            var address = await _userAddressRepository.AsQueryable()
                .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            return address != null ? _mapper.Map<UserAddressResponse>(address) : null;
        }

        public async Task<UserAddressResponse> CreateUserAddressAsync(int userId, CreateUserAddressRequest request, CancellationToken cancellationToken = default)
        {
            // Check if user exists
            var user = await _repository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            // Check if user already has an address
            var existingAddress = await _userAddressRepository.AsQueryable()
                .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            if (existingAddress != null)
                throw new InvalidOperationException("Korisnik već ima adresu. Koristite ažuriranje.");

            var address = _mapper.Map<UserAddress>(request);
            address.UserId = userId;

            await _userAddressRepository.AddAsync(address, cancellationToken);

            return _mapper.Map<UserAddressResponse>(address);
        }

        public async Task<UserAddressResponse> UpdateUserAddressAsync(int userId, UpdateUserAddressRequest request, CancellationToken cancellationToken = default)
        {
            var address = await _userAddressRepository.AsQueryable()
                .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            if (address == null)
                throw new KeyNotFoundException("Adresa nije pronađena.");

            _mapper.Map(request, address);
            await _userAddressRepository.UpdateAsync(address, cancellationToken);

            return _mapper.Map<UserAddressResponse>(address);
        }

        public async Task DeleteUserAddressAsync(int userId, CancellationToken cancellationToken = default)
        {
            var address = await _userAddressRepository.AsQueryable()
                .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            if (address == null)
                throw new KeyNotFoundException("Adresa nije pronađena.");

            await _userAddressRepository.DeleteAsync(address, cancellationToken);
        }

        // User Hobby Management
        public async Task<List<UserHobbyResponse>> GetUserHobbiesAsync(int userId, CancellationToken cancellationToken = default)
        {
            var userHobbies = await _userHobbyRepository.AsQueryable()
                .Include(uh => uh.Hobby)
                .Where(uh => uh.UserId == userId)
                .ToListAsync(cancellationToken);

            return userHobbies.Select(uh => new UserHobbyResponse
            {
                Id = uh.Id,
                UserId = uh.UserId,
                HobbyId = uh.HobbyId,
                HobbyName = uh.Hobby.Name,
                SkillLevel = uh.SkillLevel,
                Bio = uh.Bio
            }).ToList();
        }

        public async Task<UserHobbyResponse> AddUserHobbyAsync(int userId, AddUserHobbyRequest request, CancellationToken cancellationToken = default)
        {
            // Check if user exists
            var userExists = await _repository.AsQueryable().AnyAsync(u => u.Id == userId, cancellationToken);
            if (!userExists)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            // Check if hobby already added
            var exists = await _userHobbyRepository.AsQueryable()
                .AnyAsync(uh => uh.UserId == userId && uh.HobbyId == request.HobbyId, cancellationToken);

            if (exists)
                throw new ConflictException("Korisnik već ima ovaj hobi.");

            var userHobby = new UserHobby
            {
                UserId = userId,
                HobbyId = request.HobbyId,
                SkillLevel = request.SkillLevel,
                Bio = request.Bio
            };

            await _userHobbyRepository.AddAsync(userHobby, cancellationToken);

            // Load hobby name
            var hobbyName = await _userHobbyRepository.AsQueryable()
                .Where(uh => uh.Id == userHobby.Id)
                .Select(uh => uh.Hobby.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            return new UserHobbyResponse
            {
                Id = userHobby.Id,
                UserId = userHobby.UserId,
                HobbyId = userHobby.HobbyId,
                HobbyName = hobbyName,
                SkillLevel = userHobby.SkillLevel,
                Bio = userHobby.Bio
            };
        }

        public async Task DeleteUserHobbyAsync(int userId, int hobbyId, CancellationToken cancellationToken = default)
        {
            var userHobby = await _userHobbyRepository.AsQueryable()
                .FirstOrDefaultAsync(uh => uh.UserId == userId && uh.HobbyId == hobbyId, cancellationToken);

            if (userHobby == null)
                throw new KeyNotFoundException("Korisnikov hobi nije pronađen.");

            await _userHobbyRepository.DeleteAsync(userHobby, cancellationToken);
        }

        // Password Management
        public async Task<bool> VerifyPasswordAsync(int userId, string password, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task ChangePasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _repository.UpdateAsync(user, cancellationToken);
        }
    }
}
