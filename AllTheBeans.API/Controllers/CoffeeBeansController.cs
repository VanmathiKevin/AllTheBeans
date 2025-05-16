using AllTheBeans.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllTheBeans.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeeBeansController : ControllerBase
    {
        private readonly ICoffeeBeanService _coffeeBeanService;

        public CoffeeBeansController(ICoffeeBeanService coffeeBeanService)
        {
            _coffeeBeanService = coffeeBeanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _coffeeBeanService.GetAllBeansAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bean = await _coffeeBeanService.GetByIdAsync(id);
            return bean == null ? NotFound() : Ok(bean);
        }
    }
}
