
using AllTheBeans.Application.DTOs;

namespace AllTheBeans.Application.Interfaces
{
    public interface ICoffeeBeanService
    {
        Task<IEnumerable<CoffeeBeanDto>> GetAllBeansAsync();
        Task<CoffeeBeanDto?> GetByIdAsync(int id);
    }
}
