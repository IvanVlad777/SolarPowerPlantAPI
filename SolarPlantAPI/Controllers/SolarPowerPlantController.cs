using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarPlantAPI.Data;
using SolarPlantAPI.Models;
using System.Drawing;
using Microsoft.AspNetCore.JsonPatch;

namespace SolarPlantAPI.Controllers
{
    [Route("api/solarplant")]
    [ApiController]
    [Authorize]
    public class SolarPowerPlantController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<SolarPowerPlantController> _logger;
        public SolarPowerPlantController(DatabaseContext databaseContext, ILogger<SolarPowerPlantController> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSolarPlants()
        {
            _logger.LogInformation("Fetching all solar power plants from the database.");
            try
            {
                var plants = await _databaseContext.SolarPowerPlants.ToListAsync();

                if (!plants.Any())
                {
                    _logger.LogWarning("No solar power plants found in the database.");
                    return NotFound(new { message = "No solar power plants available." });
                }

                _logger.LogInformation($"Returning {plants.Count} solar power plants.");
                return Ok(plants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching solar power plants.");
                return StatusCode(500, "An error occurred while retrieving data.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSolarPlantById(int id)
        {
            _logger.LogInformation("Fetching solar power plant. SPPlant ID: {id}.", id);

            try
            {
                var plant = await _databaseContext.SolarPowerPlants.FindAsync(id);
                if (plant == null)
                {
                    _logger.LogWarning("Solar power plant with ID {id} not found.", id);
                    return NotFound(new { message = "Solar power plant not found." });
                }

                _logger.LogInformation("Solar power plant with ID {id} retrieved successfully.", id);
                return Ok(plant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching solar power plant. SPPlant ID: {id}.", id);
                return StatusCode(500, "An error occurred while retrieving data.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSolarPlant([FromBody] SolarPowerPlant plant)
        {
            _logger.LogInformation("Attempting to create a new solar power plant.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid solar power plant data provided.");
                return BadRequest(ModelState);
            }

            try
            {
                _databaseContext.SolarPowerPlants.Add(plant);
                await _databaseContext.SaveChangesAsync();

                _logger.LogInformation("Successfully created solar power plant. SPPlant ID: {id}.", plant.Id);
                return CreatedAtAction(nameof(GetSolarPlantById), new { id = plant.Id }, plant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new solar power plant.");
                return StatusCode(500, "An error occurred while creating the solar power plant.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSolarPlant(int id, [FromBody] SolarPowerPlant updatedPlant)
        {
            _logger.LogInformation("Attempting to update solar power plant. PowerPlant ID: {id}.", id);

            try
            {
                var plant = await _databaseContext.SolarPowerPlants.FindAsync(id);
                if (plant == null)
                {
                    _logger.LogWarning("Solar power plant with ID {id} not found for update.", id);
                    return NotFound(new { message = "Solar power plant not found." });
                }

                plant.Name = updatedPlant.Name ?? plant.Name;
                plant.InstalledPower = updatedPlant.InstalledPower > 0 ? updatedPlant.InstalledPower : plant.InstalledPower;
                plant.InstallationDate = updatedPlant.InstallationDate != default ? updatedPlant.InstallationDate : plant.InstallationDate;
                plant.Latitude = updatedPlant.Latitude != default ? updatedPlant.Latitude : plant.Latitude;
                plant.Longitude = updatedPlant.Longitude != default ? updatedPlant.Longitude : plant.Longitude;

                await _databaseContext.SaveChangesAsync();

                _logger.LogInformation("Solar power plant with ID {id} successfully updated.", id);
                return Ok(new { message = "Solar power plant successfully updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating solar power plant. ID of plant: {id}.", id);
                return StatusCode(500, "An error occurred while updating the solar power plant.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSolarPlant(int id)
        {
            _logger.LogInformation("Attempting to delete solar power plant. Plant ID {id}.", id);

            try
            {
                var plant = await _databaseContext.SolarPowerPlants.FindAsync(id);
                if (plant == null)
                {
                    _logger.LogWarning("Solar power plant with ID {id} not found for delete.", id);
                    return NotFound(new { message = "Solar power plant not found." });
                }

                _databaseContext.SolarPowerPlants.Remove(plant);
                await _databaseContext.SaveChangesAsync();

                _logger.LogInformation("Solar power plant with ID {id} successfully deleted.", id);
                return Ok(new { message = "Solar power plant successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting solar power plant with ID: {id}.", id);
                return StatusCode(500, "An error occurred while deleting the solar power plant.");
            }
        }
    }
}
