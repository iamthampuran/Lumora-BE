using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdatedEmployeeRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Employee Role Type",
            table: "Employees");

        migrationBuilder.DropColumn(
            name: "Employee Role Value",
            table: "Employees");

        migrationBuilder.AddColumn<string>(
            name: "EmployeeRole",
            table: "Employees",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EmployeeRole",
            table: "Employees");

        migrationBuilder.AddColumn<string>(
            name: "Employee Role Type",
            table: "Employees",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Employee Role Value",
            table: "Employees",
            type: "text",
            nullable: false,
            defaultValue: "");
    }
}
