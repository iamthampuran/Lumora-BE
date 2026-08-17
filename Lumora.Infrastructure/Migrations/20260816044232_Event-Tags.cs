using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class EventTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventTags",
            columns: table => new
            {
                EventId = table.Column<Guid>(type: "uuid", nullable: false),
                TagId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventTags", x => new { x.EventId, x.TagId });
                table.ForeignKey(
                    name: "FK_EventTags_Events_EventId",
                    column: x => x.EventId,
                    principalTable: "Events",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_EventTags_Tags_TagId",
                    column: x => x.TagId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_EventTags_EventId",
            table: "EventTags",
            column: "EventId");

        migrationBuilder.CreateIndex(
            name: "IX_EventTags_TagId",
            table: "EventTags",
            column: "TagId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EventTags");
    }
}
