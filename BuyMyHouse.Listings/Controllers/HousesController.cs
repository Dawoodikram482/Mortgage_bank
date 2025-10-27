using BuyMyHouse.Core.Interfaces;
using BuyMyHouse.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace BuyMyHouse.Listings.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HousesController : ControllerBase
{
    private readonly IHouseRepository _houseRepository;
    private readonly ILogger<HousesController> _logger;

    public HousesController(IHouseRepository houseRepository, ILogger<HousesController> logger)
    {
        _houseRepository = houseRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all available houses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<House>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<House>>> GetAllHouses()
    {
        _logger.LogInformation("Fetching all houses");
        var houses = await _houseRepository.GetAllAsync();
        return Ok(houses);
    }

    /// <summary>
    /// Get house by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(House), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<House>> GetHouseById(int id)
    {
        _logger.LogInformation("Fetching house with ID: {HouseId}", id);
        var house = await _houseRepository.GetByIdAsync(id);
        
        if (house == null)
        {
            _logger.LogWarning("House with ID {HouseId} not found", id);
            return NotFound(new { message = $"House with ID {id} not found" });
        }

        return Ok(house);
    }

    /// <summary>
    /// Search houses by price range
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<House>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<House>>> SearchByPriceRange(
        [FromQuery] decimal minPrice = 0, 
        [FromQuery] decimal maxPrice = decimal.MaxValue)
    {
        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            return BadRequest(new { message = "Invalid price range" });
        }

        _logger.LogInformation("Searching houses in price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
        var houses = await _houseRepository.GetByPriceRangeAsync(minPrice, maxPrice);
        return Ok(houses);
    }
}
