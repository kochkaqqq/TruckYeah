using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCargoAutomaticCalculation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseAutomaticCalculation",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "WeightPerPackageKg",
                table: "Cargos",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseAutomaticCalculation",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "WeightPerPackageKg",
                table: "Cargos");
        }
    }
}
