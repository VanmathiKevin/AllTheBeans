
using AllTheBeans.Domain.Models;

namespace AllTheBeans.Application.Interfaces
{
    public interface IBeanOfTheDayRepository
    {
        Task<BeanOfTheDay?> GetBeanByDateAsync(DateTime date);
        Task<BeanOfTheDay?> GetPreviousDayBeanAsync();
        Task AddBeanOfTheDayAsync(BeanOfTheDay beanOfTheDay);
    }
}
