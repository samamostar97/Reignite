using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class HobbyService : IHobbyService
    {
        private readonly IRepository<Hobby, int> _hobbyRepository;
        private readonly IMapper _mapper;

        public HobbyService(IRepository<Hobby, int> hobbyRepository, IMapper mapper)
        {
            _hobbyRepository = hobbyRepository;
            _mapper = mapper;
        }

        public async Task<List<HobbyResponse>> GetAllAsync()
        {
            var hobbies = await _hobbyRepository.AsQueryable()
                .OrderBy(h => h.Name)
                .ToListAsync();

            return _mapper.Map<List<HobbyResponse>>(hobbies);
        }
    }
}
