using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingRoutesAndGeometry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoutePoints_CargoId",
                table: "RoutePoints");

            migrationBuilder.AddColumn<DateTime>(
                name: "RouteCalculatedAt",
                table: "Trucks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RouteDistanceKm",
                table: "Trucks",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteDurationMinutes",
                table: "Trucks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RouteFuelLiters",
                table: "Trucks",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RouteGeometryGeoJson",
                table: "Trucks",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CargoId",
                table: "RoutePoints",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "RoutePoints",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lon",
                table: "RoutePoints",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "TruckId",
                table: "RoutePoints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RouteCalculatedAt",
                table: "Cargos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RouteDistanceKm",
                table: "Cargos",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteDurationMinutes",
                table: "Cargos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RouteFuelLiters",
                table: "Cargos",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RouteGeometryGeoJson",
                table: "Cargos",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_CargoId_Order",
                table: "RoutePoints",
                columns: new[] { "CargoId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_TruckId_Order",
                table: "RoutePoints",
                columns: new[] { "TruckId", "Order" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoutePoints_Trucks_TruckId",
                table: "RoutePoints",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutePoints_Trucks_TruckId",
                table: "RoutePoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutePoints_CargoId_Order",
                table: "RoutePoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutePoints_TruckId_Order",
                table: "RoutePoints");

            migrationBuilder.DropColumn(
                name: "RouteCalculatedAt",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RouteDistanceKm",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RouteDurationMinutes",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RouteFuelLiters",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RouteGeometryGeoJson",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "RoutePoints");

            migrationBuilder.DropColumn(
                name: "Lon",
                table: "RoutePoints");

            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "RoutePoints");

            migrationBuilder.DropColumn(
                name: "RouteCalculatedAt",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "RouteDistanceKm",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "RouteDurationMinutes",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "RouteFuelLiters",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "RouteGeometryGeoJson",
                table: "Cargos");

            migrationBuilder.AlterColumn<Guid>(
                name: "CargoId",
                table: "RoutePoints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_CargoId",
                table: "RoutePoints",
                column: "CargoId");
        }
    }
}
