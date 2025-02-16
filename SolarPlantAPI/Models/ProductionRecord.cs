using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolarPlantAPI.Models
{
    public class ProductionRecord
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey("SolarPowerPlant")]
        public int SolarPowerPlantId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public double RealProduction { get; set; }
        [Required]
        public double ForecastedProduction { get; set; }
    }
}
