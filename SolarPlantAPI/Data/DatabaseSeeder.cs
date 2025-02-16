using Microsoft.EntityFrameworkCore;
using SolarPlantAPI.Models;
using System;

namespace SolarPlantAPI.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedData(DatabaseContext context)
        {
            if (!context.SolarPowerPlants.Any())
            {
                Console.WriteLine("No solar power plants found. Seeding initial data...");

                var plants = new List<SolarPowerPlant>
                {
                    new SolarPowerPlant { Name = "Solar Plant A", InstalledPower = 5000, InstallationDate = DateTime.UtcNow.AddYears(-3), Latitude = 45.123, Longitude = 15.456 },
                    new SolarPowerPlant { Name = "Solar Plant B", InstalledPower = 3000, InstallationDate = DateTime.UtcNow.AddYears(-2), Latitude = 46.789, Longitude = 16.789 }
                };

                context.SolarPowerPlants.AddRange(plants);
                await context.SaveChangesAsync();
                Console.WriteLine("Seeded solar power plants.");
            }

            if (!context.ProductionRecords.Any())
            {
                Console.WriteLine("No production records found. Generating data...");

                var plants = context.SolarPowerPlants.ToList();
                var records = new List<ProductionRecord>();

                Random random = new Random();
                DateTime startDate = DateTime.UtcNow.AddDays(-365);
                int recordCount = 0;

                foreach (var plant in plants)
                {
                    DateTime timestamp = startDate;
                    while (timestamp < DateTime.UtcNow && recordCount < 500)
                    {
                        double variationFactor = (random.NextDouble() * 0.2) + 0.9;
                        double realProduction = (plant.InstalledPower * variationFactor) * 0.00025;
                        double forecastedProduction = realProduction * ((random.NextDouble() * 0.1) + 0.95);

                        records.Add(new ProductionRecord
                        {
                            SolarPowerPlantId = plant.Id,
                            Timestamp = timestamp,
                            RealProduction = realProduction,
                            ForecastedProduction = forecastedProduction
                        });

                        timestamp = timestamp.AddMinutes(15);
                        recordCount++;
                    }
                }

                context.ProductionRecords.AddRange(records);
                await context.SaveChangesAsync();
                Console.WriteLine($"Seeded {records.Count} production records.");
            }
        }
    }
}