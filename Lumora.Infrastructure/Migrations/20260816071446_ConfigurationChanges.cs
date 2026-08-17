using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ConfigurationChanges : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PhotographyStyle",
            table: "Events");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PhotographyStyle",
            table: "Events",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);
    }
}
