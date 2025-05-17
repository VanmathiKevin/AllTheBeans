
using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AllTheBeans.Application.Services
{
    public class CoffeeBeanService : ICoffeeBeanService
    {
        private readonly ICoffeeBeanRepository _coffeeBeanRepo;
        private readonly IBeanOfTheDayRepository _beanOfTheDayRepo;
        private readonly IBeanSelectorService _beanSelectorService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CoffeeBeanService> _logger;

        public CoffeeBeanService(
            ICoffeeBeanRepository coffeeBeanRepo,
            IBeanOfTheDayRepository beanOfTheDayRepo,
            IBeanSelectorService beanSelectorService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CoffeeBeanService> logger)
        {
            _coffeeBeanRepo = coffeeBeanRepo;
            _beanOfTheDayRepo = beanOfTheDayRepo;
            _beanSelectorService = beanSelectorService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<IEnumerable<CoffeeBeanDto>> GetAllBeansAsync()
        {
            _logger.LogInformation("Fetching all coffee beans from the repository.");
            var beans = await _coffeeBeanRepo.GetAllBeansAsync();
            return _mapper.Map<IEnumerable<CoffeeBeanDto>>(beans);
        }

        public async Task<CoffeeBeanDto?> GetBeanByIdAsync(int id)
        {
            _logger.LogInformation("Fetching coffee bean with ID {Id}", id);
            var bean = await _coffeeBeanRepo.GetBeanByIdAsync(id);
            if(bean == null)
            {
                _logger.LogWarning("Coffee bean with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<CoffeeBeanDto>(bean);
        }
        public async Task<CoffeeBeanDto> CreateBeanAsync(CreateCoffeeBeanDto dto)
        {
            _logger.LogInformation("Creating a new coffee bean: {@Dto}", dto);
            var bean = _mapper.Map<CoffeeBean>(dto);
            await _coffeeBeanRepo.AddBeanAsync(bean);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Coffee bean created successfully with ID {Id}", bean.Id);

            return _mapper.Map<CoffeeBeanDto>(bean);
        }
        public async Task<bool> UpdateBeanAsync(int id, CoffeeBeanDto dto)
        {
            _logger.LogInformation("Updating coffee bean with ID {Id}", id);
            var bean = await _coffeeBeanRepo.GetBeanByIdAsync(id);
            if (bean is null) return false;

            _mapper.Map(dto, bean);
            await _coffeeBeanRepo.UpdateBeanAsync(bean);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Coffee bean with ID {Id} updated successfully", id);

            return true;
        }

        public async Task<bool> DeleteBeanAsync(int id)
        {
            _logger.LogInformation("Deleting coffee bean with ID {Id}", id);
            var bean = await _coffeeBeanRepo.GetBeanByIdAsync(id);
            if (bean == null)
            {
                _logger.LogWarning("Cannot delete: coffee bean with ID {Id} not found", id);
                return false;
            }

            await _coffeeBeanRepo.DeleteBeanAsync(bean);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Coffee bean with ID {Id} deleted successfully", id);
            return true;
        }
        public async Task<IEnumerable<CoffeeBeanDto>> SearchBeansAsync(string query)
        {
            _logger.LogInformation("Searching coffee beans with query: {Query}", query);
            var results = await _coffeeBeanRepo.SearchBeansAsync(query);
            return _mapper.Map<IEnumerable<CoffeeBeanDto>>(results);
        }

        public async Task<CoffeeBeanDto> GetBeanOfTheDayAsync()
        {
            _logger.LogInformation("Fetching Bean of the Day");
            var today = DateTime.UtcNow.Date;

            var existing = await _beanOfTheDayRepo.GetBeanByDateAsync(today);
            if (existing != null)
            {
                _logger.LogInformation("Bean of the Day already selected: {BeanId}", existing.CoffeeBeanId);
                return _mapper.Map<CoffeeBeanDto>(existing.CoffeeBean);
            }
                

            var allBeans = await _coffeeBeanRepo.GetAllBeansAsync();
            var previousBean = await _beanOfTheDayRepo.GetPreviousDayBeanAsync();

            var selectedBean = await _beanSelectorService.SelectBeanAsync(allBeans, previousBean?.CoffeeBean);

            var newBeanOfTheDay = new BeanOfTheDay
            {
                CoffeeBeanId = selectedBean.Id,
                Date = today
            };

            await _beanOfTheDayRepo.AddBeanOfTheDayAsync(newBeanOfTheDay);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("New Bean of the Day selected: {BeanId}", selectedBean.Id);
            return _mapper.Map<CoffeeBeanDto>(selectedBean);
        }
    }
}
