using AllTheBeans.Application.DTOs;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCoffeeBeanDto dto)
        {
            var result = await _coffeeBeanService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCoffeeBeanDto dto)
        {
            var success = await _coffeeBeanService.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _coffeeBeanService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var results = await _coffeeBeanService.SearchAsync(q);
            return Ok(results);
        }
    }
}
