
namespace AllTheBeans.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
