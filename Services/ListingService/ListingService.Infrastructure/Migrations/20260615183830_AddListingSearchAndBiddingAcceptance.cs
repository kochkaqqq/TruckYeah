using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingSearchAndBiddingAcceptance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalEquipment",
                table: "Trucks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AcceptedBidId",
                table: "Cargos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BiddingClosedAt",
                table: "Cargos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "CargoBids",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CargoBids",
                type: "text",
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_AcceptedBidId",
                table: "Cargos",
                column: "AcceptedBidId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_Status",
                table: "CargoBids",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cargos_AcceptedBidId",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_CargoBids_Status",
                table: "CargoBids");

            migrationBuilder.DropColumn(
                name: "AdditionalEquipment",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "AcceptedBidId",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "BiddingClosedAt",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "CargoBids");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CargoBids");
        }
    }
}
