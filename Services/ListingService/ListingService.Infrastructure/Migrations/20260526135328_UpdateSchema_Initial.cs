using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CargoName = table.Column<string>(type: "text", nullable: false),
                    RouteFrom = table.Column<string>(type: "text", nullable: false),
                    RouteTo = table.Column<string>(type: "text", nullable: false),
                    LoadDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnloadDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WeightTons = table.Column<double>(type: "double precision", nullable: false),
                    VolumeM3 = table.Column<double>(type: "double precision", nullable: false),
                    BodyTypeRequired = table.Column<string>(type: "text", nullable: false),
                    LoadingType = table.Column<int>(type: "integer", nullable: false),
                    LengthCm = table.Column<double>(type: "double precision", nullable: true),
                    WidthCm = table.Column<double>(type: "double precision", nullable: true),
                    HeightCm = table.Column<double>(type: "double precision", nullable: true),
                    PalletsCount = table.Column<int>(type: "integer", nullable: true),
                    PackagingType = table.Column<string>(type: "text", nullable: true),
                    RequiresCMR = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresTIR = table.Column<bool>(type: "boolean", nullable: false),
                    IsADR = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    AllowBargaining = table.Column<bool>(type: "boolean", nullable: false),
                    PrepaymentPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BoostToTop = table.Column<bool>(type: "boolean", nullable: false),
                    BoostedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: true),
                    SourceListingId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RouteFrom = table.Column<string>(type: "text", nullable: false),
                    RouteTo = table.Column<string>(type: "text", nullable: false),
                    CapacityTons = table.Column<double>(type: "double precision", nullable: false),
                    VolumeM3 = table.Column<double>(type: "double precision", nullable: false),
                    BodyType = table.Column<string>(type: "text", nullable: false),
                    LoadingType = table.Column<int>(type: "integer", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    AllowBargaining = table.Column<bool>(type: "boolean", nullable: false),
                    PrepaymentPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SourceListingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoutePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CargoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CargoEntityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutePoints_Cargos_CargoEntityId",
                        column: x => x.CargoEntityId,
                        principalTable: "Cargos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_CargoEntityId",
                table: "RoutePoints",
                column: "CargoEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutePoints");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "Cargos");
        }
    }
}
