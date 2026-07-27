using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class FixedStudioMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ConsumerProfiles_Users_Id",
            table: "ConsumerProfiles");

        migrationBuilder.DropForeignKey(
            name: "FK_StudioProfiles_Users_Id",
            table: "StudioProfiles");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "FK_ConsumerProfiles_Users_Id",
            table: "ConsumerProfiles",
            column: "Id",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_StudioProfiles_Users_Id",
            table: "StudioProfiles",
            column: "Id",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
