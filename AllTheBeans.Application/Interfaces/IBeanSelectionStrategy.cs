
using AllTheBeans.Domain.Models;

namespace AllTheBeans.Application.Interfaces
{
    public interface IBeanSelectionStrategy
    {
        Task<CoffeeBean> SelectBeanAsync(IEnumerable<CoffeeBean> availableBeans, CoffeeBean? previousBean);
    }
}
