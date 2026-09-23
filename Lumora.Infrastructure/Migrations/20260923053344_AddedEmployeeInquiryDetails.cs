using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddedEmployeeInquiryDetails : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InquiryEmployees",
            columns: table => new
            {
                InquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
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
                table.PrimaryKey("PK_InquiryEmployees", x => new { x.InquiryId, x.EmployeeId });
                table.ForeignKey(
                    name: "FK_InquiryEmployees_Employees_EmployeeId",
                    column: x => x.EmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_InquiryEmployees_Inquiries_InquiryId",
                    column: x => x.InquiryId,
                    principalTable: "Inquiries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InquiryEmployees_EmployeeId",
            table: "InquiryEmployees",
            column: "EmployeeId");

        migrationBuilder.CreateIndex(
            name: "IX_InquiryEmployees_InquiryId",
            table: "InquiryEmployees",
            column: "InquiryId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InquiryEmployees");
    }
}
