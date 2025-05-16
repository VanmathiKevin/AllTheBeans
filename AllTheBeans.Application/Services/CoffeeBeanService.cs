
using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AutoMapper;

namespace AllTheBeans.Application.Services
{
    public class CoffeeBeanService : ICoffeeBeanService
    {
        private readonly ICoffeeBeanRepository _coffeeBeanRepo;
        private readonly IBeanOfTheDayRepository _beanOfTheDayRepo;
        private readonly IBeanSelectorService _beanSelectorService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
       
        public CoffeeBeanService(
            ICoffeeBeanRepository coffeeBeanRepo,
            IBeanOfTheDayRepository beanOfTheDayRepo,
            IBeanSelectorService beanSelectorService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _coffeeBeanRepo = coffeeBeanRepo;
            _beanOfTheDayRepo = beanOfTheDayRepo;
            _beanSelectorService = beanSelectorService;
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
        public async Task<bool> UpdateAsync(int id, CreateCoffeeBeanDto dto)
        {
            var bean = await _coffeeBeanRepo.GetByIdAsync(id);
            if (bean is null) return false;

            _mapper.Map(dto, bean);
            await _coffeeBeanRepo.UpdateAsync(bean);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bean = await _coffeeBeanRepo.GetByIdAsync(id);
            if (bean is null) return false;

            await _coffeeBeanRepo.DeleteAsync(bean);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<CoffeeBeanDto>> SearchAsync(string query)
        {
            var results = await _coffeeBeanRepo.SearchAsync(query);
            return _mapper.Map<IEnumerable<CoffeeBeanDto>>(results);
        }

        public async Task<CoffeeBeanDto> GetBeanOfTheDayAsync()
        {
            var today = DateTime.UtcNow.Date;

            var existing = await _beanOfTheDayRepo.GetByDateAsync(today);
            if (existing != null)
                return _mapper.Map<CoffeeBeanDto>(existing.CoffeeBean);

            var allBeans = await _coffeeBeanRepo.GetAllAsync();
            var previousBean = await _beanOfTheDayRepo.GetPreviousDayAsync();

            var selectedBean = await _beanSelectorService.SelectBeanAsync(allBeans, previousBean?.CoffeeBean);

            var newBeanOfTheDay = new BeanOfTheDay
            {
                CoffeeBeanId = selectedBean.Id,
                Date = today
            };

            await _beanOfTheDayRepo.AddAsync(newBeanOfTheDay);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CoffeeBeanDto>(selectedBean);
        }
    }
}
