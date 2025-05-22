using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllTheBeans.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CoffeeBeansController : ControllerBase
    {
        private readonly ICoffeeBeanService _coffeeBeanService;
        private readonly ILogger<CoffeeBeansController> _logger;
        private readonly ICacheService _cacheService;

        public CoffeeBeansController(ICoffeeBeanService coffeeBeanService, ILogger<CoffeeBeansController> logger, ICacheService cacheService)
        {
            _coffeeBeanService = coffeeBeanService;
            _logger = logger;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get all coffee beans.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("[API] GET /api/v1/coffeeBeans requested");
            const string cacheKey = "AllCoffeeBeans";

            var cached = await _cacheService.GetAsync<IEnumerable<CoffeeBeanDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("[API] Returned all beans from cache.");
                return Ok(cached);
            }

            var beans = await _coffeeBeanService.GetAllBeansAsync();
            await _cacheService.SetAsync(cacheKey, beans, TimeSpan.FromMinutes(10));
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
            _logger.LogInformation("[API] GET /api/v1/coffeeBeans/{Id} requested", id);

            if (id <= 0)
            {
                _logger.LogWarning("[API] Invalid ID provided for GetById: {Id}", id);
                return BadRequest("Invalid ID. ID must be a positive number.");
            }

            var cacheKey = $"CoffeeBean:{id}";

            var cached = await _cacheService.GetAsync<CoffeeBeanDto>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("[API] Returned coffee bean with ID {Id} from cache", id);
                return Ok(cached);
            }

            var bean = await _coffeeBeanService.GetBeanByIdAsync(id);
            await _cacheService.SetAsync(cacheKey, bean, TimeSpan.FromMinutes(10));
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
            _logger.LogInformation("[API] POST /api/v1/coffeeBeans requested with payload {@CreateCoffeeBeanDto}", dto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _coffeeBeanService.CreateBeanAsync(dto);
            await _cacheService.RemoveAsync("AllCoffeeBeans");

            _logger.LogInformation("[API] Coffee bean created successfully: {Name} with ID {id}", dto.Name, result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing coffee bean.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCoffeeBeanDto dto)
        {
            _logger.LogInformation("[API] PUT /api/v1/coffeeBeans/{Id} requested with payload {@CoffeeBeanDto}", id, dto);

            if (id <= 0)
            {
                _logger.LogWarning("[API] Invalid ID provided for Update: {Id}", id);
                return BadRequest("Invalid ID. ID must be a positive number.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _coffeeBeanService.UpdateBeanAsync(id, dto);
            await _cacheService.RemoveAsync("AllCoffeeBeans");
            await _cacheService.RemoveAsync($"CoffeeBean:{id}");

            _logger.LogInformation("[API] Coffee bean with ID {Id} updated successfully", id);
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
            _logger.LogInformation("[API] DELETE /api/v1/coffeeBeans/{Id} requested", id);

            if (id <= 0)
            {
                _logger.LogWarning("[API] Invalid ID provided for Delete: {Id}", id);
                return BadRequest("Invalid ID. ID must be a positive number.");
            }

            await _coffeeBeanService.DeleteBeanAsync(id);
            await _cacheService.RemoveAsync("AllCoffeeBeans");
            await _cacheService.RemoveAsync($"CoffeeBean:{id}");

            _logger.LogInformation("[API] Coffee bean with ID {Id} deleted successfully", id);
            return NoContent();
        }

        /// <summary>
        /// Search coffee beans by name, country, or colour.
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            _logger.LogInformation("[API] GET /api/v1/coffeeBeans/search requested with query: {Query}", query);

            var cacheKey = $"Search:{query.ToLower()}";

            var cached = await _cacheService.GetAsync<IEnumerable<CoffeeBeanDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("[API] Returned search results for query '{Query}' from cache", query);
                return Ok(cached);
            }

            var result = await _coffeeBeanService.SearchBeansAsync(query);
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return Ok(result);
        }

        /// <summary>
        /// Get Bean of the Day.
        /// </summary>
        [HttpGet("bean-of-the-day")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBeanOfTheDay()
        {
            _logger.LogInformation("[API] GET /api/v1/coffeeBeans/bean-of-the-day requested");

            var cacheKey = $"BeanOfTheDay:{DateTime.UtcNow:yyyy-MM-dd}";

            var cached = await _cacheService.GetAsync<CoffeeBeanDto>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("[API] Returned Bean of the Day from cache: {BeanName}", cached.Name);
                return Ok(cached);
            }

            var bean = await _coffeeBeanService.GetBeanOfTheDayAsync();
            await _cacheService.SetAsync(cacheKey, bean, TimeSpan.FromDays(1));

            _logger.LogInformation("[API] Bean of the Day cached: {BeanName}", bean.Name);
            return Ok(bean);
            
        }
    }
}
