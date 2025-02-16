using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarPlantAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntallationDate",
                table: "SolarPowerPlants",
                newName: "InstallationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstallationDate",
                table: "SolarPowerPlants",
                newName: "IntallationDate");
        }
    }
}
