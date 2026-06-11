using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteListingAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutePoints_Cargos_CargoEntityId",
                table: "RoutePoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutePoints_CargoEntityId",
                table: "RoutePoints");

            migrationBuilder.DropColumn(
                name: "CargoEntityId",
                table: "RoutePoints");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Trucks",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Trucks",
                type: "text",
                nullable: false,
                defaultValue: "Draft",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "RouteTo",
                table: "Trucks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RouteFrom",
                table: "Trucks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Trucks",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrepaymentPercent",
                table: "Trucks",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "Trucks",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "LoadingType",
                table: "Trucks",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Trucks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Trucks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "BodyType",
                table: "Trucks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowBargaining",
                table: "Trucks",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "CrewDriversCount",
                table: "Trucks",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Trucks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                table: "Trucks",
                type: "text",
                nullable: false,
                defaultValue: "Exchange");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "RoutePoints",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Cargos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateName",
                table: "Cargos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Cargos",
                type: "text",
                nullable: false,
                defaultValue: "Draft",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "RouteTo",
                table: "Cargos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RouteFrom",
                table: "Cargos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresTIR",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresCMR",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrepaymentPercent",
                table: "Cargos",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "PackagingType",
                table: "Cargos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Cargos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoadingType",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTemplate",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsADR",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Cargos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "CargoName",
                table: "Cargos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "BoostToTop",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "BodyTypeRequired",
                table: "Cargos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowBargaining",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<bool>(
                name: "BiddingEnabled",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MinBidStep",
                table: "Cargos",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresTwoDrivers",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "StartingPrice",
                table: "Cargos",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Cargos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                table: "Cargos",
                type: "text",
                nullable: false,
                defaultValue: "Exchange");

            migrationBuilder.Sql("""
                UPDATE "Cargos" SET "Status" = CASE "Status"
                    WHEN '0' THEN 'Draft'
                    WHEN '1' THEN 'Published'
                    WHEN '2' THEN 'Archived'
                    WHEN '3' THEN 'Completed'
                    ELSE "Status"
                END;

                UPDATE "Cargos" SET "PaymentType" = CASE "PaymentType"
                    WHEN '0' THEN 'Cash'
                    WHEN '1' THEN 'WithVAT'
                    WHEN '2' THEN 'WithoutVAT'
                    ELSE "PaymentType"
                END;

                UPDATE "Cargos" SET "LoadingType" = CASE "LoadingType"
                    WHEN '0' THEN 'Rear'
                    WHEN '1' THEN 'Side'
                    WHEN '2' THEN 'Top'
                    WHEN '3' THEN 'FullAccess'
                    ELSE "LoadingType"
                END;

                UPDATE "Trucks" SET "Status" = CASE "Status"
                    WHEN '0' THEN 'Draft'
                    WHEN '1' THEN 'Published'
                    WHEN '2' THEN 'Archived'
                    WHEN '3' THEN 'Completed'
                    ELSE "Status"
                END;

                UPDATE "Trucks" SET "PaymentType" = CASE "PaymentType"
                    WHEN '0' THEN 'Cash'
                    WHEN '1' THEN 'WithVAT'
                    WHEN '2' THEN 'WithoutVAT'
                    ELSE "PaymentType"
                END;

                UPDATE "Trucks" SET "LoadingType" = CASE "LoadingType"
                    WHEN '0' THEN 'Rear'
                    WHEN '1' THEN 'Side'
                    WHEN '2' THEN 'Top'
                    WHEN '3' THEN 'FullAccess'
                    ELSE "LoadingType"
                END;
                """);

            migrationBuilder.CreateTable(
                name: "CargoBids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CargoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CarrierUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoBids_Cargos_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_AvailableFrom",
                table: "Trucks",
                column: "AvailableFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Status",
                table: "Trucks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_UserId",
                table: "Trucks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Visibility",
                table: "Trucks",
                column: "Visibility");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_CargoId",
                table: "RoutePoints",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_BiddingEnabled",
                table: "Cargos",
                column: "BiddingEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_LoadDateTime",
                table: "Cargos",
                column: "LoadDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Status",
                table: "Cargos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_UserId",
                table: "Cargos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Visibility",
                table: "Cargos",
                column: "Visibility");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_CargoId",
                table: "CargoBids",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_CargoId_CarrierUserId",
                table: "CargoBids",
                columns: new[] { "CargoId", "CarrierUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoutePoints_Cargos_CargoId",
                table: "RoutePoints",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutePoints_Cargos_CargoId",
                table: "RoutePoints");

            migrationBuilder.DropTable(
                name: "CargoBids");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_AvailableFrom",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_Status",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_UserId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_Visibility",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_RoutePoints_CargoId",
                table: "RoutePoints");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_BiddingEnabled",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_LoadDateTime",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_Status",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_UserId",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_Visibility",
                table: "Cargos");

            migrationBuilder.Sql("""
                UPDATE "Cargos" SET "Status" = CASE "Status"
                    WHEN 'Draft' THEN '0'
                    WHEN 'Published' THEN '1'
                    WHEN 'Archived' THEN '2'
                    WHEN 'Completed' THEN '3'
                    ELSE "Status"
                END;

                UPDATE "Cargos" SET "PaymentType" = CASE "PaymentType"
                    WHEN 'Cash' THEN '0'
                    WHEN 'WithVAT' THEN '1'
                    WHEN 'WithoutVAT' THEN '2'
                    ELSE "PaymentType"
                END;

                UPDATE "Cargos" SET "LoadingType" = CASE "LoadingType"
                    WHEN 'Rear' THEN '0'
                    WHEN 'Side' THEN '1'
                    WHEN 'Top' THEN '2'
                    WHEN 'FullAccess' THEN '3'
                    ELSE "LoadingType"
                END;

                UPDATE "Trucks" SET "Status" = CASE "Status"
                    WHEN 'Draft' THEN '0'
                    WHEN 'Published' THEN '1'
                    WHEN 'Archived' THEN '2'
                    WHEN 'Completed' THEN '3'
                    ELSE "Status"
                END;

                UPDATE "Trucks" SET "PaymentType" = CASE "PaymentType"
                    WHEN 'Cash' THEN '0'
                    WHEN 'WithVAT' THEN '1'
                    WHEN 'WithoutVAT' THEN '2'
                    ELSE "PaymentType"
                END;

                UPDATE "Trucks" SET "LoadingType" = CASE "LoadingType"
                    WHEN 'Rear' THEN '0'
                    WHEN 'Side' THEN '1'
                    WHEN 'Top' THEN '2'
                    WHEN 'FullAccess' THEN '3'
                    ELSE "LoadingType"
                END;
                """);

            migrationBuilder.DropColumn(
                name: "CrewDriversCount",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "BiddingEnabled",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "MinBidStep",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "RequiresTwoDrivers",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Cargos");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Trucks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Trucks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Draft");

            migrationBuilder.AlterColumn<string>(
                name: "RouteTo",
                table: "Trucks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "RouteFrom",
                table: "Trucks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Trucks",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrepaymentPercent",
                table: "Trucks",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentType",
                table: "Trucks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "LoadingType",
                table: "Trucks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Trucks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Trucks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "BodyType",
                table: "Trucks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "AllowBargaining",
                table: "Trucks",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "RoutePoints",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "CargoEntityId",
                table: "RoutePoints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "TemplateName",
                table: "Cargos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Cargos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Draft");

            migrationBuilder.AlterColumn<string>(
                name: "RouteTo",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "RouteFrom",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresTIR",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresCMR",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrepaymentPercent",
                table: "Cargos",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentType",
                table: "Cargos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PackagingType",
                table: "Cargos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Cargos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LoadingType",
                table: "Cargos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTemplate",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsADR",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Cargos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "CargoName",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "BoostToTop",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "BodyTypeRequired",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "AllowBargaining",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RoutePoints_CargoEntityId",
                table: "RoutePoints",
                column: "CargoEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutePoints_Cargos_CargoEntityId",
                table: "RoutePoints",
                column: "CargoEntityId",
                principalTable: "Cargos",
                principalColumn: "Id");
        }
    }
}
