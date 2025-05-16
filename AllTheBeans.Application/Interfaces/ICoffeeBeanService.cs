
using AllTheBeans.Application.DTOs;

namespace AllTheBeans.Application.Interfaces
{
    public interface ICoffeeBeanService
    {
        Task<IEnumerable<CoffeeBeanDto>> GetAllBeansAsync();
        Task<CoffeeBeanDto?> GetBeanByIdAsync(int id);
        Task<CoffeeBeanDto> CreateBeanAsync(CreateCoffeeBeanDto beanDto);
        Task<bool> UpdateBeanAsync(int id, CreateCoffeeBeanDto beanDto);
        Task<bool> DeleteBeanAsync(int id);
        Task<IEnumerable<CoffeeBeanDto>> SearchBeansAsync(string query);
        Task<CoffeeBeanDto> GetBeanOfTheDayAsync();
    }
}
