namespace AllTheBeans.Domain.Models
{
    public class CoffeeBean
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Colour { get; set; }
        public string Country { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public ICollection<BeanOfTheDay> BeanOfTheDayHistory { get; set; }
    }
}
