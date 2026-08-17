using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class SeedTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Tags",
            columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "ModifiedAt", "ModifiedBy", "Name" },
            values: new object[,]
            {
                { new Guid("51a4aebc-2cbe-4ce1-b95f-05a88ff163ab"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "corporate" },
                { new Guid("68f3bb15-c4bb-41a8-912c-49563b2386d2"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "traditional" },
                { new Guid("981479f3-3a6a-4b59-ac3e-3199edfd6a93"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "wedding" },
                { new Guid("a13145e5-bd2d-4433-b1f4-cd17e4b3d093"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "film" },
                { new Guid("accda9f2-4ca7-4102-8cc1-40348ea42a52"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "documentary" },
                { new Guid("c15e9002-b7eb-48a3-b197-5c7bfb22c806"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "drone" },
                { new Guid("d4702155-2ae0-445a-9fb3-cf6b04966bba"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "candid" },
                { new Guid("d7bc3b61-4fb9-4802-a83f-f7c5c91c521f"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "editorial" },
                { new Guid("df699af6-7d5e-4c0e-84df-70ba1c2c736b"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "cinematic" },
                { new Guid("f1d92970-e307-4997-9b7a-27527afd2173"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "moody" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("51a4aebc-2cbe-4ce1-b95f-05a88ff163ab"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("68f3bb15-c4bb-41a8-912c-49563b2386d2"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("981479f3-3a6a-4b59-ac3e-3199edfd6a93"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("a13145e5-bd2d-4433-b1f4-cd17e4b3d093"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("accda9f2-4ca7-4102-8cc1-40348ea42a52"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("c15e9002-b7eb-48a3-b197-5c7bfb22c806"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("d4702155-2ae0-445a-9fb3-cf6b04966bba"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("d7bc3b61-4fb9-4802-a83f-f7c5c91c521f"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("df699af6-7d5e-4c0e-84df-70ba1c2c736b"));

        migrationBuilder.DeleteData(
            table: "Tags",
            keyColumn: "Id",
            keyValue: new Guid("f1d92970-e307-4997-9b7a-27527afd2173"));
    }
}
