using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarPlantAPI.Data;
using SolarPlantAPI.Models;
using SolarPlantAPI.Services;

namespace SolarPlantAPI.Controllers
{
    [Route("api/production")]
    [ApiController]
    [Authorize]
    public class ProductionRecordController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly WeatherService _weatherService;
        private readonly ILogger<ProductionRecordController> _logger;
        public ProductionRecordController(DatabaseContext databaseContext, WeatherService weatherService, ILogger<ProductionRecordController> logger)
        {
            _databaseContext = databaseContext;
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet("{solarPlantId}")]
        public async Task<IActionResult> GetProductionRecords(
            int solarPlantId,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime,
            [FromQuery] string granularity = "15min")
        {
            _logger.LogInformation("Fetching production records for SolarPlantId {solarPlantId} from {startTime} to {endTime}.", solarPlantId, startTime, endTime);

            try
            {
                var records = await _databaseContext.ProductionRecords
                    .Where(r => r.SolarPowerPlantId == solarPlantId && r.Timestamp >= startTime && r.Timestamp <= endTime)
                    .ToListAsync();

                if (!records.Any())
                {
                    _logger.LogWarning("No production records found for SolarPlantId {solarPlantId} in the specified time range.", solarPlantId);
                    return NotFound(new { message = "No production records available for this time range." });
                }

                if (granularity == "1hour")
                {
                    records = records
                        .GroupBy(r => new { r.Timestamp.Year, r.Timestamp.Month, r.Timestamp.Day, r.Timestamp.Hour })
                        .Select(g => new ProductionRecord
                        {
                            SolarPowerPlantId = solarPlantId,
                            Timestamp = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                            RealProduction = g.Sum(r => r.RealProduction),
                            ForecastedProduction = g.Sum(r => r.ForecastedProduction)
                        })
                        .ToList();
                }

                _logger.LogInformation("Returning {count} production records for SolarPlantId {solarPlantId}.", records.Count, solarPlantId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching production records for SolarPlantId {solarPlantId}.", solarPlantId);
                return StatusCode(500, "An error occurred while retrieving production records.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductionRecord([FromBody] ProductionRecord record)
        {
            _logger.LogInformation("Attempting to create a new production record for SolarPlantId {solarPlantId}.", record.SolarPowerPlantId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid production record data provided.");
                return BadRequest(ModelState);
            }

            try
            {
                var plant = await _databaseContext.SolarPowerPlants.FindAsync(record.SolarPowerPlantId);
                if (plant == null)
                {
                    _logger.LogWarning("Solar power plant with ID {solarPlantId} not found.", record.SolarPowerPlantId);
                    return NotFound(new { message = "Solar power plant not found." });
                }

                double cloudCover = await _weatherService.GetCloudCoverAsync(plant.Latitude, plant.Longitude);
                double weatherFactor = 1.0 - (cloudCover / 100.0);
                record.ForecastedProduction *= weatherFactor;

                _databaseContext.ProductionRecords.Add(record);
                await _databaseContext.SaveChangesAsync();

                _logger.LogInformation("Successfully created production record with ID {id} for SolarPlantId {solarPlantId}.", record.Id, record.SolarPowerPlantId);
                return CreatedAtAction(nameof(GetProductionRecords), new { solarPlantId = record.SolarPowerPlantId }, record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new production record.");
                return StatusCode(500, "An error occurred while creating the production record.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductionRecord(int id, [FromBody] ProductionRecord updatedRecord)
        {
            _logger.LogInformation("Attempting to update production record with ID {id}.", id);

            try
            {
                var record = await _databaseContext.ProductionRecords.FindAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("Production record with ID {id} not found for update.", id);
                    return NotFound(new { message = "Production record not found." });
                }

                record.Timestamp = updatedRecord.Timestamp != default ? updatedRecord.Timestamp : record.Timestamp;
                record.RealProduction = updatedRecord.RealProduction > 0 ? updatedRecord.RealProduction : record.RealProduction;
                record.ForecastedProduction = updatedRecord.ForecastedProduction > 0 ? updatedRecord.ForecastedProduction : record.ForecastedProduction;

                await _databaseContext.SaveChangesAsync();

                _logger.LogInformation("Production record with ID {id} successfully updated.", id);
                return Ok(new { message = "Production record updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating production record with ID {id}.", id);
                return StatusCode(500, "An error occurred while updating the production record.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductionRecord(int id)
        {
            _logger.LogInformation("Attempting to delete production record with ID {id}.", id);

            try
            {
                var record = await _databaseContext.ProductionRecords.FindAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("Production record with ID {id} not found for deletion.", id);
                    return NotFound(new { message = "Production record not found." });
                }

                _databaseContext.ProductionRecords.Remove(record);
                await _databaseContext.SaveChangesAsync();

                _logger.LogInformation("Production record with ID {id} successfully deleted.", id);
                return Ok(new { message = "Production record successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting production record with ID {id}.", id);
                return StatusCode(500, "An error occurred while deleting the production record.");
            }
        }
    }
}
