using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triumph.HealthMs.Persistence.Data.TenantContext.Migrations
{
    /// <inheritdoc />
    public partial class Migrations_002_LinkInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InvitedEntityType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Linked = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkInvitations_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkInvitations_ApplicationUserId",
                table: "LinkInvitations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkInvitations_TenantId",
                table: "LinkInvitations",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkInvitations");
        }
    }
}
