using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingService.Infrastructure.Migrations;

[DbContext(typeof(ListingServiceDbContext))]
[Migration("20260618151000_AddListingModeration")]
public partial class AddListingModeration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        AddColumns(migrationBuilder, "Cargos");
        AddColumns(migrationBuilder, "Trucks");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        DropColumns(migrationBuilder, "Cargos");
        DropColumns(migrationBuilder, "Trucks");
    }

    private static void AddColumns(MigrationBuilder migrationBuilder, string table)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "ModeratedAt",
            table: table,
            type: "timestamp with time zone",
            nullable: true);
        migrationBuilder.AddColumn<Guid>(
            name: "ModeratedBy",
            table: table,
            type: "uuid",
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "RejectionReason",
            table: table,
            type: "character varying(1000)",
            maxLength: 1000,
            nullable: true);
    }

    private static void DropColumns(MigrationBuilder migrationBuilder, string table)
    {
        migrationBuilder.DropColumn(name: "ModeratedAt", table: table);
        migrationBuilder.DropColumn(name: "ModeratedBy", table: table);
        migrationBuilder.DropColumn(name: "RejectionReason", table: table);
    }
}
