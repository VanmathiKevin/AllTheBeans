
using AllTheBeans.Application.DTOs;

namespace AllTheBeans.Application.Interfaces
{
    public interface ICoffeeBeanService
    {
        Task<IEnumerable<CoffeeBeanDto>> GetAllBeansAsync();
        Task<CoffeeBeanDto?> GetByIdAsync(int id);
        Task<CoffeeBeanDto> CreateAsync(CreateCoffeeBeanDto beanDto);
        Task<bool> UpdateAsync(int id, CreateCoffeeBeanDto beanDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CoffeeBeanDto>> SearchAsync(string query);
        Task<CoffeeBeanDto> GetBeanOfTheDayAsync();
    }
}
