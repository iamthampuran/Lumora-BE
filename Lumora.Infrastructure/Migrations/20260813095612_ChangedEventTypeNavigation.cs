using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangedEventTypeNavigation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "event_category_type",
            table: "Events");

        migrationBuilder.DropColumn(
            name: "event_category_value",
            table: "Events");

        migrationBuilder.AddColumn<Guid>(
            name: "EventTypeId",
            table: "Events",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateTable(
            name: "EventTypes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                IsPredefined = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventTypes", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "EventTypes",
            columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsPredefined", "ModifiedAt", "ModifiedBy", "Name" },
            values: new object[,]
            {
                { new Guid("208f807f-b0de-42e8-8518-2e1d103f4318"), new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(561), "System", null, true, true, new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(562), "System", "Pre-Wedding" },
                { new Guid("482fbf1c-b62f-4704-bfeb-8da35b8a6d42"), new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(552), "System", null, true, true, new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(553), "System", "Engagement" },
                { new Guid("4aab92d6-f1fa-4fa7-8c3b-23f05fb29015"), new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(559), "System", null, true, true, new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(560), "System", "Anniversary" },
                { new Guid("672fce47-95c8-42d1-b65d-ac33bfdf2b02"), new DateTime(2026, 8, 13, 9, 48, 24, 208, DateTimeKind.Utc).AddTicks(7728), "System", null, true, true, new DateTime(2026, 8, 13, 15, 18, 24, 208, DateTimeKind.Utc).AddTicks(8300), "System", "Wedding" },
                { new Guid("9e3adba4-24a2-440d-9d3e-902e92e390f9"), new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(529), "System", null, true, true, new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(537), "System", "Birthday" },
                { new Guid("f355707c-0563-4d8a-bfa2-79129e91ef4a"), new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(549), "System", null, true, true, new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(551), "System", "Corporate" }
            });

        migrationBuilder.CreateIndex(
            name: "IX_Events_EventTypeId",
            table: "Events",
            column: "EventTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_EventTypes_Name",
            table: "EventTypes",
            column: "Name",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Events_EventTypes_EventTypeId",
            table: "Events",
            column: "EventTypeId",
            principalTable: "EventTypes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Events_EventTypes_EventTypeId",
            table: "Events");

        migrationBuilder.DropTable(
            name: "EventTypes");

        migrationBuilder.DropIndex(
            name: "IX_Events_EventTypeId",
            table: "Events");

        migrationBuilder.DropColumn(
            name: "EventTypeId",
            table: "Events");

        migrationBuilder.AddColumn<string>(
            name: "event_category_type",
            table: "Events",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "event_category_value",
            table: "Events",
            type: "text",
            nullable: false,
            defaultValue: "");
    }
}
