using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsTwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorSecret = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TwoFactorBackupCodes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuspensionInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhotoUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumerProfiles_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumerProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudioProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudioName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Website = table.Column<string>(type: "text", nullable: true),
                    StartingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric", nullable: true),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Location = table.Column<string>(type: "jsonb", nullable: false),
                    ServiceRadius = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudioProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudioProfiles_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudioProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmployeeRoleType = table.Column<string>(name: "Employee Role Type", type: "text", nullable: false),
                    EmployeeRoleValue = table.Column<string>(name: "Employee Role Value", type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_StudioProfiles_StudioId",
                        column: x => x.StudioId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SelectedStudioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    event_category_type = table.Column<string>(type: "text", nullable: false),
                    event_category_value = table.Column<string>(type: "text", nullable: false),
                    Budget = table.Column<decimal>(type: "numeric", nullable: false),
                    Duration = table.Column<decimal>(type: "numeric", nullable: false),
                    PhotographyStyle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SpecialRequirements = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ConsumerProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Location = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_ConsumerProfiles_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "ConsumerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_ConsumerProfiles_ConsumerProfileId",
                        column: x => x.ConsumerProfileId,
                        principalTable: "ConsumerProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_StudioProfiles_SelectedStudioId",
                        column: x => x.SelectedStudioId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateTable(
                name: "Inquiries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RejectionStatus = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ConsumerProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    StudioProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inquiries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inquiries_ConsumerProfiles_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "ConsumerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inquiries_ConsumerProfiles_ConsumerProfileId",
                        column: x => x.ConsumerProfileId,
                        principalTable: "ConsumerProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inquiries_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inquiries_StudioProfiles_StudioId",
                        column: x => x.StudioId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inquiries_StudioProfiles_StudioProfileId",
                        column: x => x.StudioProfileId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                    GalleryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GalleryStatus = table.Column<string>(type: "text", nullable: false, defaultValue: "Draft"),
                    ExternalProvider = table.Column<int>(type: "integer", nullable: false),
                    FolderLink = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Galleries_Inquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "Inquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "INR"),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RazorPayOrderId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RazorePaySignature = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    StudioProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Inquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "Inquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_StudioProfiles_StudioId",
                        column: x => x.StudioId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_StudioProfiles_StudioProfileId",
                        column: x => x.StudioProfileId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                    InquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ConsumerProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    StudioProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_ConsumerProfiles_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "ConsumerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_ConsumerProfiles_ConsumerProfileId",
                        column: x => x.ConsumerProfileId,
                        principalTable: "ConsumerProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Inquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "Inquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_StudioProfiles_StudioId",
                        column: x => x.StudioId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_StudioProfiles_StudioProfileId",
                        column: x => x.StudioProfileId,
                        principalTable: "StudioProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerProfiles_UserId",
                table: "ConsumerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email_Phone",
                table: "Employees",
                columns: new[] { "Email", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StudioId",
                table: "Employees",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ConsumerId",
                table: "Events",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ConsumerProfileId",
                table: "Events",
                column: "ConsumerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SelectedStudioId",
                table: "Events",
                column: "SelectedStudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Status",
                table: "Events",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_ApprovedAt",
                table: "Galleries",
                column: "ApprovedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_ExternalProvider",
                table: "Galleries",
                column: "ExternalProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_GalleryStatus",
                table: "Galleries",
                column: "GalleryStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_InquiryId",
                table: "Galleries",
                column: "InquiryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_ConsumerId",
                table: "Inquiries",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_ConsumerProfileId",
                table: "Inquiries",
                column: "ConsumerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_EventId",
                table: "Inquiries",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_Status",
                table: "Inquiries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_StudioId",
                table: "Inquiries",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_StudioId_EventId_ConsumerId",
                table: "Inquiries",
                columns: new[] { "StudioId", "EventId", "ConsumerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_StudioProfileId",
                table: "Inquiries",
                column: "StudioProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_EventId",
                table: "Payments",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InquiryId",
                table: "Payments",
                column: "InquiryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RazorPayOrderId",
                table: "Payments",
                column: "RazorPayOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudioId",
                table: "Payments",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudioProfileId",
                table: "Payments",
                column: "StudioProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ConsumerId",
                table: "Reviews",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ConsumerProfileId",
                table: "Reviews",
                column: "ConsumerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EventId",
                table: "Reviews",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_InquiryId",
                table: "Reviews",
                column: "InquiryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Rating",
                table: "Reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StudioId",
                table: "Reviews",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StudioProfileId",
                table: "Reviews",
                column: "StudioProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudioProfiles_StudioName",
                table: "StudioProfiles",
                column: "StudioName");

            migrationBuilder.CreateIndex(
                name: "IX_StudioProfiles_UserId",
                table: "StudioProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudioTags_TagsId",
                table: "StudioTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "StudioTags");

            migrationBuilder.DropTable(
                name: "Inquiries");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "ConsumerProfiles");

            migrationBuilder.DropTable(
                name: "StudioProfiles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
