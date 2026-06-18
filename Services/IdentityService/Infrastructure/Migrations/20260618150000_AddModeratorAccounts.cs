using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Infrastructure.Migrations;

[DbContext(typeof(ApplicationContext))]
[Migration("20260618150000_AddModeratorAccounts")]
public partial class AddModeratorAccounts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "created_at",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<string>(
            name: "role",
            table: "users",
            type: "varchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "User");

        migrationBuilder.AddColumn<string>(
            name: "status",
            table: "users",
            type: "varchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "Active");

        migrationBuilder.CreateIndex(name: "IX_users_role", table: "users", column: "role");
        migrationBuilder.CreateIndex(name: "IX_users_status", table: "users", column: "status");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_users_role", table: "users");
        migrationBuilder.DropIndex(name: "IX_users_status", table: "users");
        migrationBuilder.DropColumn(name: "created_at", table: "users");
        migrationBuilder.DropColumn(name: "role", table: "users");
        migrationBuilder.DropColumn(name: "status", table: "users");
    }
}
