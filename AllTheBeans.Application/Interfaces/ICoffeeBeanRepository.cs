
using AllTheBeans.Domain.Models;

namespace AllTheBeans.Application.Interfaces
{
    public interface ICoffeeBeanRepository
    {
        Task<IEnumerable<CoffeeBean>> GetAllAsync();
        Task<CoffeeBean?> GetByIdAsync(int id);
        Task AddAsync(CoffeeBean bean);
        Task UpdateAsync(CoffeeBean bean);
        Task DeleteAsync(CoffeeBean bean);
        Task<IEnumerable<CoffeeBean>> SearchAsync(string keyword);
    }
}
