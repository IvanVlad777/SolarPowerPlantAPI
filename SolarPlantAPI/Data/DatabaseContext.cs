using Microsoft.EntityFrameworkCore;
using SolarPlantAPI.Models;

namespace SolarPlantAPI.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SolarPowerPlant> SolarPowerPlants { get; set; }
        public DbSet<ProductionRecord> ProductionRecords { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
