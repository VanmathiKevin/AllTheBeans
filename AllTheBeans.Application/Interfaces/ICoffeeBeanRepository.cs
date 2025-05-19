
using AllTheBeans.Domain.Models;

namespace AllTheBeans.Application.Interfaces
{
    public interface ICoffeeBeanRepository
    {
        Task<IEnumerable<CoffeeBean>> GetAllBeansAsync();
        Task<CoffeeBean?> GetBeanByIdAsync(int id);
        Task AddBeanAsync(CoffeeBean bean);
        Task UpdateBeanAsync(CoffeeBean bean);
        Task DeleteBeanAsync(CoffeeBean bean);
        Task<IEnumerable<CoffeeBean>> SearchBeansAsync(string keyword);
    }
}
