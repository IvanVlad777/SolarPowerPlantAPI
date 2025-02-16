using System.ComponentModel.DataAnnotations;

namespace SolarPlantAPI.Models
{
    public class SolarPowerPlant
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Required(ErrorMessage = "Power is required")]
        public double InstalledPower { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime InstallationDate { get; set; }
        [Required(ErrorMessage = "Longitude is required")]
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Latitude is required")]
        public double Latitude { get; set; }
        public List<ProductionRecord> ProductionRecords { get; set; } = new();
    }
}
