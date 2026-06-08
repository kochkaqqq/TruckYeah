using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    phone_country_code = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    phone_number = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    companyId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.companyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "dbo",
                columns: table => new
                {
                    token_hash = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    user_id1 = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    device_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    user_agent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    ip_address = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.token_hash);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.user_id1,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UX_companies_name",
                table: "companies",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                schema: "dbo",
                table: "RefreshTokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_IpAddress",
                schema: "dbo",
                table: "RefreshTokens",
                column: "ip_address");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_user_id1",
                schema: "dbo",
                table: "RefreshTokens",
                column: "user_id1");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_ExpiresAt",
                schema: "dbo",
                table: "RefreshTokens",
                columns: new[] { "user_id", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_users_companyId",
                table: "users",
                column: "companyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
