using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lumora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewEventTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "EventTypes" ("Id","CreatedAt","CreatedBy","DeletedAt","IsActive","IsPredefined","ModifiedAt","ModifiedBy","Name")
                VALUES
                ('2d25cce8-8923-4c57-8f83-70de9b04c006','2026-08-18T10:00:00Z','System',NULL,TRUE,TRUE,'2026-08-18T10:00:00Z','System','Graduation'),
                ('57f0de02-5aa8-43bd-a0f8-04f4f49f2102','2026-08-18T10:00:00Z','System',NULL,TRUE,TRUE,'2026-08-18T10:00:00Z','System','Haldi'),
                ('79c8f6c9-f717-4d2e-9269-b66712a94805','2026-08-18T10:00:00Z','System',NULL,TRUE,TRUE,'2026-08-18T10:00:00Z','System','Housewarming'),
                ('9b4c75db-4fd8-4f64-8d26-7b3ae16bb004','2026-08-18T10:00:00Z','System',NULL,TRUE,TRUE,'2026-08-18T10:00:00Z','System','Maternity Shoot'),
                ('d6f58d6f-43f5-4be8-9a34-2e45f9a1a101','2026-08-18T10:00:00Z','System',NULL,TRUE,TRUE,'2026-08-18T10:00:00Z','System','Reception'),
                ('f9e17d4f-5f9b-4604-af70-65a3c30c6b03','2026-08-18T10:00:00Z','System',NULL,TRUE,TRUE,'2026-08-18T10:00:00Z','System','Baby Shower')
                ON CONFLICT ("Id") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("2d25cce8-8923-4c57-8f83-70de9b04c006"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("57f0de02-5aa8-43bd-a0f8-04f4f49f2102"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("79c8f6c9-f717-4d2e-9269-b66712a94805"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("9b4c75db-4fd8-4f64-8d26-7b3ae16bb004"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("d6f58d6f-43f5-4be8-9a34-2e45f9a1a101"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("f9e17d4f-5f9b-4604-af70-65a3c30c6b03"));
        }
    }
}
