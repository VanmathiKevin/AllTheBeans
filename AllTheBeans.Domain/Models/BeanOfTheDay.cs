
namespace AllTheBeans.Domain.Models
{
    public class BeanOfTheDay
    {
        public int Id { get; set; }
        public int CoffeeBeanId { get; set; }
        public DateTime Date { get; set; }
        public CoffeeBean? CoffeeBean { get; set; }
    }
}
