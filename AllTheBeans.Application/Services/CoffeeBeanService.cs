
using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AutoMapper;

namespace AllTheBeans.Application.Services
{
    public class CoffeeBeanService : ICoffeeBeanService
    {
        private readonly ICoffeeBeanRepository _coffeeBeanRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CoffeeBeanService(
            ICoffeeBeanRepository coffeeBeanRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _coffeeBeanRepo = coffeeBeanRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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
        public async Task<CoffeeBeanDto> CreateAsync(CreateCoffeeBeanDto dto)
        {
            var bean = _mapper.Map<CoffeeBean>(dto);
            await _coffeeBeanRepo.AddAsync(bean);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CoffeeBeanDto>(bean);
        }
    }
}
