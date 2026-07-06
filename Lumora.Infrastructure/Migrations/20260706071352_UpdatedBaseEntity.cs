using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdatedBaseEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Tags",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Tags",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "StudioProfiles",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "StudioProfiles",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Reviews",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Reviews",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Payments",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Payments",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Inquiries",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Inquiries",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Galleries",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Galleries",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Events",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Events",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Employees",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "Employees",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "ConsumerProfiles",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ModifiedBy",
            table: "ConsumerProfiles",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "StudioProfiles");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "StudioProfiles");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Reviews");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Reviews");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Payments");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Payments");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Inquiries");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Inquiries");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Galleries");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Galleries");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Events");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Events");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Employees");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "Employees");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "ConsumerProfiles");

        migrationBuilder.DropColumn(
            name: "ModifiedBy",
            table: "ConsumerProfiles");
    }
}
