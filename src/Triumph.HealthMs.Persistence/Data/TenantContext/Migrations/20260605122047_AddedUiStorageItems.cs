using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triumph.HealthMs.Persistence.Data.TenantContext.Migrations
{
    /// <inheritdoc />
    public partial class AddedUiStorageItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacilityAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityAnnouncements_OrganizationalFacilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "OrganizationalFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantAnnouncements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UiStorageItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UiStorageItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAnnouncements_FacilityId",
                table: "FacilityAnnouncements",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAnnouncements_TenantId_FacilityId_Deleted_Type_Crea~",
                table: "FacilityAnnouncements",
                columns: new[] { "TenantId", "FacilityId", "Deleted", "Type", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAnnouncements_ValidUntil",
                table: "FacilityAnnouncements",
                column: "ValidUntil");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAnnouncements_TenantId_Deleted_Type_CreatedAt",
                table: "TenantAnnouncements",
                columns: new[] { "TenantId", "Deleted", "Type", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantAnnouncements_ValidUntil",
                table: "TenantAnnouncements",
                column: "ValidUntil");

            migrationBuilder.CreateIndex(
                name: "IX_UiStorageItems_Key_CreatedBy_Deleted",
                table: "UiStorageItems",
                columns: new[] { "Key", "CreatedBy", "Deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityAnnouncements");

            migrationBuilder.DropTable(
                name: "TenantAnnouncements");

            migrationBuilder.DropTable(
                name: "UiStorageItems");
        }
    }
}
