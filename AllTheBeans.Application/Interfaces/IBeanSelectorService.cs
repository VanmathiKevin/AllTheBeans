
using AllTheBeans.Domain.Models;

namespace AllTheBeans.Application.Interfaces
{
    public interface IBeanSelectorService
    {
        Task<CoffeeBean> SelectBeanAsync(IEnumerable<CoffeeBean> availableBeans, CoffeeBean? previousBean);
    }
}
