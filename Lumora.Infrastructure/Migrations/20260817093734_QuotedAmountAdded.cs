using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class QuotedAmountAdded : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_StudioTag",
            table: "StudioTag");

        migrationBuilder.AddColumn<decimal>(
            name: "QuotedAmount",
            table: "Inquiries",
            type: "numeric(8,2)",
            precision: 8,
            scale: 2,
            nullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_StudioTag",
            table: "StudioTag",
            columns: new[] { "StudioProfile", "TagId" });

        migrationBuilder.AddCheckConstraint(
            name: "CK_Inquiry_QuotedAmount",
            table: "Inquiries",
            sql: "\"QuotedAmount\" >= 0 AND \"QuotedAmount\" <= 100000");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_StudioTag",
            table: "StudioTag");

        migrationBuilder.DropCheckConstraint(
            name: "CK_Inquiry_QuotedAmount",
            table: "Inquiries");

        migrationBuilder.DropColumn(
            name: "QuotedAmount",
            table: "Inquiries");

        migrationBuilder.AddPrimaryKey(
            name: "PK_StudioTag",
            table: "StudioTag",
            column: "Id");
    }
}
