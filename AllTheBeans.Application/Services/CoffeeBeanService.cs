
using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Exceptions;
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
        private readonly IBeanSelectionStrategy _beanSelectionStrategy;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CoffeeBeanService> _logger;

        public CoffeeBeanService(
            ICoffeeBeanRepository coffeeBeanRepo,
            IBeanOfTheDayRepository beanOfTheDayRepo,
            IBeanSelectionStrategy beanSelectionStrategy,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CoffeeBeanService> logger)
        {
            _coffeeBeanRepo = coffeeBeanRepo;
            _beanOfTheDayRepo = beanOfTheDayRepo;
            _beanSelectionStrategy = beanSelectionStrategy;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<IEnumerable<CoffeeBeanDto>> GetAllBeansAsync()
        {
            _logger.LogInformation("[Service] Fetching all coffee beans.");
            var beans = await _coffeeBeanRepo.GetAllBeansAsync();
            return _mapper.Map<IEnumerable<CoffeeBeanDto>>(beans);
        }

        public async Task<CoffeeBeanDto?> GetBeanByIdAsync(int id)
        {
            _logger.LogInformation("[Service] Fetching coffee bean with ID {Id}", id);
            var bean = await _coffeeBeanRepo.GetBeanByIdAsync(id);
            if(bean == null)
            {
                _logger.LogWarning("[Service] Coffee bean with ID {Id} not found", id);
                throw new NotFoundException($"Coffee bean with ID {id} not found.");
            }

            return _mapper.Map<CoffeeBeanDto>(bean);
        }
        public async Task<CoffeeBeanDto> CreateBeanAsync(CreateCoffeeBeanDto dto)
        {
            _logger.LogInformation("[Service] Creating a new coffee bean: {@Dto}", dto);
            var bean = _mapper.Map<CoffeeBean>(dto);

            try
            {
                await _coffeeBeanRepo.AddBeanAsync(bean);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("[Service] Coffee bean created successfully with ID {Id}", bean.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] Failed to create coffee bean: {@Dto}", dto);
                throw;
            }

            return _mapper.Map<CoffeeBeanDto>(bean);
        }
        public async Task<bool> UpdateBeanAsync(int id, CreateCoffeeBeanDto dto)
        {
            _logger.LogInformation("[Service] Updating coffee bean with ID {Id}", id);
            var bean = await _coffeeBeanRepo.GetBeanByIdAsync(id);

            if (bean == null)
            {
                _logger.LogWarning("Cannot update: coffee bean with ID {Id} not found", id);
                throw new NotFoundException($"Coffee bean with ID {id} not found.");
            }

            try
            {
                _mapper.Map(dto, bean);
                await _coffeeBeanRepo.UpdateBeanAsync(bean);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("[Service] Coffee bean with ID {Id} updated successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " [Service] Failed to update coffee bean with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteBeanAsync(int id)
        {
            _logger.LogInformation("[Service] Deleting coffee bean with ID {Id}", id);
            var bean = await _coffeeBeanRepo.GetBeanByIdAsync(id);

            if (bean == null)
            {
                _logger.LogWarning("[Service] Cannot delete: coffee bean with ID {Id} not found", id);
                throw new NotFoundException($"Coffee bean with ID {id} not found.");
            }

            try
            {
                await _coffeeBeanRepo.DeleteBeanAsync(bean);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("[Service] Coffee bean with ID {Id} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] Failed to delete coffee bean with ID {Id}", id);
                throw;
            }
        }
        public async Task<IEnumerable<CoffeeBeanDto>> SearchBeansAsync(string query)
        {
            _logger.LogInformation("[Service] Searching coffee beans with query: {Query}", query);
            var results = await _coffeeBeanRepo.SearchBeansAsync(query);
            return _mapper.Map<IEnumerable<CoffeeBeanDto>>(results);
        }

        public async Task<CoffeeBeanDto> GetBeanOfTheDayAsync()
        {
            _logger.LogInformation("[Service] Fetching Bean of the Day from repository");
            var today = DateTime.UtcNow.Date;

            try
            {
                var existing = await _beanOfTheDayRepo.GetBeanByDateAsync(today);
                if (existing != null)
                {
                    _logger.LogInformation("[Service] Bean of the Day already selected: {BeanId}", existing.CoffeeBeanId);
                    return _mapper.Map<CoffeeBeanDto>(existing.CoffeeBean);
                }


                var allBeans = await _coffeeBeanRepo.GetAllBeansAsync();
                var previousBean = await _beanOfTheDayRepo.GetPreviousDayBeanAsync();

                var selectedBean = await _beanSelectionStrategy.SelectBeanAsync(allBeans, previousBean?.CoffeeBean);

                var newBeanOfTheDay = new BeanOfTheDay
                {
                    CoffeeBeanId = selectedBean.Id,
                    Date = today
                };

                await _beanOfTheDayRepo.AddBeanOfTheDayAsync(newBeanOfTheDay);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("[Service] New Bean of the Day selected: {BeanId}", selectedBean.Id);
                return _mapper.Map<CoffeeBeanDto>(selectedBean);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] Error occurred while selecting Bean of the Day");
                throw;
            }
        }
    }
}
