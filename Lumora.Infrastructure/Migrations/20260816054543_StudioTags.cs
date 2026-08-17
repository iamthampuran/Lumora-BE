using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class StudioTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "StudioTags");

        migrationBuilder.CreateTable(
            name: "StudioTag",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StudioProfile = table.Column<Guid>(type: "uuid", nullable: false),
                TagId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StudioTag", x => x.Id);
                table.ForeignKey(
                    name: "FK_StudioTag_StudioProfiles_StudioProfile",
                    column: x => x.StudioProfile,
                    principalTable: "StudioProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_StudioTag_Tags_TagId",
                    column: x => x.TagId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_StudioTag_StudioProfile",
            table: "StudioTag",
            column: "StudioProfile");

        migrationBuilder.CreateIndex(
            name: "IX_StudioTag_TagId",
            table: "StudioTag",
            column: "TagId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "StudioTag");

        migrationBuilder.CreateTable(
            name: "StudioTags",
            columns: table => new
            {
                StudiosId = table.Column<Guid>(type: "uuid", nullable: false),
                TagsId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StudioTags", x => new { x.StudiosId, x.TagsId });
                table.ForeignKey(
                    name: "FK_StudioTags_StudioProfiles_StudiosId",
                    column: x => x.StudiosId,
                    principalTable: "StudioProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_StudioTags_Tags_TagsId",
                    column: x => x.TagsId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_StudioTags_TagsId",
            table: "StudioTags",
            column: "TagsId");
    }
}
