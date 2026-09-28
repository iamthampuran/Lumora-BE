using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdatedPaymentDetails : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PayoutUpiId",
            table: "StudioProfiles",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "PlatformFee",
            table: "Payments",
            type: "numeric",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<string>(
            name: "RazorPayQrCodeId",
            table: "Payments",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<decimal>(
            name: "ServiceFee",
            table: "Payments",
            type: "numeric",
            nullable: false,
            defaultValue: 0m);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PayoutUpiId",
            table: "StudioProfiles");

        migrationBuilder.DropColumn(
            name: "PlatformFee",
            table: "Payments");

        migrationBuilder.DropColumn(
            name: "RazorPayQrCodeId",
            table: "Payments");

        migrationBuilder.DropColumn(
            name: "ServiceFee",
            table: "Payments");
    }
}
