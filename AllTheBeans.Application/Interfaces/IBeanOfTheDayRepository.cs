
using AllTheBeans.Domain.Models;

namespace AllTheBeans.Application.Interfaces
{
    public interface IBeanOfTheDayRepository
    {
        Task<BeanOfTheDay?> GetByDateAsync(DateTime date);
        Task<BeanOfTheDay?> GetPreviousDayAsync();
        Task AddAsync(BeanOfTheDay beanOfTheDay);
    }
}
