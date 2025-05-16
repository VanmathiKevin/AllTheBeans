
using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Interfaces;
using AutoMapper;

namespace AllTheBeans.Application.Services
{
    public class CoffeeBeanService : ICoffeeBeanService
    {
        private readonly ICoffeeBeanRepository _coffeeBeanRepo;
        private readonly IMapper _mapper;

        public CoffeeBeanService(
            ICoffeeBeanRepository coffeeBeanRepo,
            IMapper mapper)
        {
            _coffeeBeanRepo = coffeeBeanRepo;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CoffeeBeanDto>> GetAllBeansAsync()
        {
            var beans = await _coffeeBeanRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<CoffeeBeanDto>>(beans);
        }

        public async Task<CoffeeBeanDto?> GetByIdAsync(int id)
        {
            var bean = await _coffeeBeanRepo.GetByIdAsync(id);
            return bean is null ? null : _mapper.Map<CoffeeBeanDto>(bean);
        }
    }
}
