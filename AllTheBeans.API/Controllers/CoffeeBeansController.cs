using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllTheBeans.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoffeeBeansController : ControllerBase
    {
        private readonly ICoffeeBeanService _coffeeBeanService;
        private readonly ILogger<CoffeeBeansController> _logger;

        public CoffeeBeansController(ICoffeeBeanService coffeeBeanService, ILogger<CoffeeBeansController> logger)
        {
            _coffeeBeanService = coffeeBeanService;
            _logger = logger;
        }

        /// <summary>
        /// Get all coffee beans.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/coffeeBeans requested");
            var beans = await _coffeeBeanService.GetAllBeansAsync();
            return Ok(beans);
        }

        /// <summary>
        /// Get a coffee bean by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/coffeeBeans/{Id} requested", id);
            var bean = await _coffeeBeanService.GetBeanByIdAsync(id);
            if(bean == null)
            {
                _logger.LogWarning("Coffee bean with ID {Id} not found", id);
                return NotFound();
            }

            return Ok(bean);
        }

        /// <summary>
        /// Create a new coffee bean.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCoffeeBeanDto dto)
        {
            _logger.LogInformation("POST /api/coffeeBeans requested with payload {@CreateCoffeeBeanDto}", dto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _coffeeBeanService.CreateBeanAsync(dto);
            _logger.LogInformation("Coffee bean created successfully: {Name} with ID {id}", dto.Name, result.Id);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing coffee bean.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CoffeeBeanDto dto)
        {
            _logger.LogInformation("PUT /api/coffeeBeans/{Id} requested with payload {@CoffeeBeanDto}", id, dto);

            if (id != dto.Id)
            {
                ModelState.AddModelError("Id", "URL ID and payload ID must match.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = await _coffeeBeanService.UpdateBeanAsync(id, dto);
            if(!isUpdated)
            {
                _logger.LogWarning("Update failed: Coffee bean with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Coffee bean with ID {Id} updated successfully", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a coffee bean by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/coffeeBeans/{Id} requested", id);

            var isDeleted = await _coffeeBeanService.DeleteBeanAsync(id);
            if (!isDeleted)
            {
                _logger.LogWarning("Delete failed: Coffee bean with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Coffee bean with ID {Id} deleted successfully", id);
            return NoContent();
        }

        /// <summary>
        /// Search coffee beans by name, country, or colour.
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            _logger.LogInformation("GET /api/coffeeBeans/search requested with query: {Query}", query);
            var result = await _coffeeBeanService.SearchBeansAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get Bean of the Day.
        /// </summary>
        [HttpGet("bean-of-the-day")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBeanOfTheDay()
        {
            _logger.LogInformation("GET /api/coffeeBeans/bean-of-the-day requested");
            var bean = await _coffeeBeanService.GetBeanOfTheDayAsync();
            return Ok(bean);
        }
    }
}
