using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class PortfolioImagesAdded : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PortfolioImages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StudioId = table.Column<Guid>(type: "uuid", nullable: false),
                ImageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PortfolioImages", x => x.Id);
                table.ForeignKey(
                    name: "FK_PortfolioImages_StudioProfiles_StudioId",
                    column: x => x.StudioId,
                    principalTable: "StudioProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PortfolioImages_StudioId",
            table: "PortfolioImages",
            column: "StudioId");

        migrationBuilder.CreateIndex(
            name: "IX_PortfolioImages_StudioId_DisplayOrder",
            table: "PortfolioImages",
            columns: new[] { "StudioId", "DisplayOrder" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PortfolioImages");
    }
}
