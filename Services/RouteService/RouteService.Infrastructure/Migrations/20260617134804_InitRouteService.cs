using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitRouteService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RouteCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    RequestHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DistanceKm = table.Column<double>(type: "double precision", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    FuelConsumptionLiters = table.Column<double>(type: "double precision", nullable: false),
                    TollRoadsStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    Geometry = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteCalculations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResolvedRoutePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteCalculationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Lon = table.Column<double>(type: "double precision", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResolvedRoutePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResolvedRoutePoints_RouteCalculations_RouteCalculationId",
                        column: x => x.RouteCalculationId,
                        principalTable: "RouteCalculations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResolvedRoutePoints_RouteCalculationId_Order",
                table: "ResolvedRoutePoints",
                columns: new[] { "RouteCalculationId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_RouteCalculations_ExpiresAt",
                table: "RouteCalculations",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RouteCalculations_RequestHash",
                table: "RouteCalculations",
                column: "RequestHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResolvedRoutePoints");

            migrationBuilder.DropTable(
                name: "RouteCalculations");
        }
    }
}
