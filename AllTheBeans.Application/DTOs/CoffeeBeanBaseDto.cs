using System.ComponentModel.DataAnnotations;

namespace AllTheBeans.Application.DTOs
{
    public class CoffeeBeanBaseDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Colour is required.")]
        [StringLength(50, ErrorMessage = "Colour can't exceed 50 characters.")]
        public string Colour { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50, ErrorMessage = "Country can't exceed 50 characters.")]
        public string Country { get; set; }

        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Please provide a valid image URL.")]
        public string ImageUrl { get; set; }
    }
}
