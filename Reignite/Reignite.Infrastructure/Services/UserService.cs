using System.Security.Cryptography.X509Certificates;
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
        public UserService(
            IRepository<User, int> repository,
            IMapper mapper,
            IFileStorageService fileStorageService) : base(repository, mapper)
        {
            _fileStorageService = fileStorageService;
        }
        protected override async Task BeforeCreateAsync(User entity, CreateUserRequest dto)
        {
            var emailExists= await _repository.AsQueryable().AnyAsync(x=>x.Email.ToLower()==dto.Email.ToLower());
            if(emailExists) throw new ConflictException("Korisnik sa ovim emailom vec postoji");
            var usernameExists= await _repository.AsQueryable().AnyAsync(x=>x.Username.ToLower()==dto.Username.ToLower());
            if(usernameExists) throw new ConflictException("Korisnik sa ovim usernameom vec postoji");
            var phoneNumberExists= await _repository.AsQueryable().AnyAsync(x=>x.PhoneNumber.ToLower()==dto.PhoneNumber.ToLower());
            if(phoneNumberExists) throw new ConflictException("Korisnik sa ovim brojem telefona vec postoji");
        }
        protected override async Task BeforeUpdateAsync(User entity, UpdateUserRequest dto)
        {
            var emailExists= await _repository.AsQueryable().AnyAsync(x=>x.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase) && x.Id!=entity.Id);
            if(emailExists) throw new ConflictException("Korisnik sa ovim emailom vec postoji");
            var usernameExists= await _repository.AsQueryable().AnyAsync(x=>x.Username.Equals(dto.Username, StringComparison.CurrentCultureIgnoreCase) && x.Id!=entity.Id);
            if(usernameExists) throw new ConflictException("Korisnik sa ovim usernameom vec postoji");
            var phoneNumberExists= await _repository.AsQueryable().AnyAsync(x=>x.PhoneNumber.Equals(dto.PhoneNumber, StringComparison.CurrentCultureIgnoreCase) && x.Id!=entity.Id);
            if(phoneNumberExists) throw new ConflictException("Korisnik sa ovim brojem telefona vec postoji");
        }
        public override async Task<UserResponse> CreateAsync(CreateUserRequest dto)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repository.AddAsync(user);

            return await GetByIdWithCountsAsync(user.Id);
        }

        public async Task<UserResponse> CreateWithImageAsync(CreateUserRequest dto, FileUploadRequest? imageRequest)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repository.AddAsync(user);

            if (imageRequest != null && imageRequest.FileStream != null && imageRequest.FileSize > 0)
            {
                var uploadResult = await _fileStorageService.UploadAsync(imageRequest, "users", user.Id.ToString());

                if (uploadResult.Success)
                {
                    user.ProfileImageUrl = uploadResult.FileUrl;
                    await _repository.UpdateAsync(user);
                }
            }

            return await GetByIdWithCountsAsync(user.Id);
        }

        public override async Task<UserResponse> GetByIdAsync(int id)
        {
            return await GetByIdWithCountsAsync(id);
        }

        protected override IQueryable<User> ApplyFilter(IQueryable<User> query, UserQueryFilter filter)
        {
           query=query.Include(x=>x.Orders).ThenInclude(x=>x.OrderItems).Include(x=>x.Projects);
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

        public async Task<UserResponse> UploadImageAsync(int userId, FileUploadRequest fileRequest)
        {
            var user = await _repository.AsQueryable()
                .Include(u => u.Orders)
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                await _fileStorageService.DeleteAsync(user.ProfileImageUrl);
            }

            var uploadResult = await _fileStorageService.UploadAsync(fileRequest, "users", userId.ToString());

            if (!uploadResult.Success)
                throw new InvalidOperationException(uploadResult.ErrorMessage);

            user.ProfileImageUrl = uploadResult.FileUrl;
            await _repository.UpdateAsync(user);

            var response = _mapper.Map<UserResponse>(user);
            response.OrderCount = user.Orders.Count;
            response.ProjectCount = user.Projects.Count;

            return response;
        }

        public async Task<bool> DeleteImageAsync(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (string.IsNullOrEmpty(user.ProfileImageUrl))
                return false;

            var deleted = await _fileStorageService.DeleteAsync(user.ProfileImageUrl);

            user.ProfileImageUrl = null;
            await _repository.UpdateAsync(user);

            return deleted;
        }
    }
}
