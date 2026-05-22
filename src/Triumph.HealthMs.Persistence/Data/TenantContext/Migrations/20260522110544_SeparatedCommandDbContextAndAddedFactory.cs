using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triumph.HealthMs.Persistence.Data.TenantContext.Migrations
{
    /// <inheritdoc />
    public partial class SeparatedCommandDbContextAndAddedFactory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Deleted", "DeletedAt", "DeletedBy", "Description", "DisplayName", "PermissionType", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("423e4568-e89b-12d3-a456-426655440000"), new DateTimeOffset(new DateTime(2026, 5, 9, 20, 22, 30, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", false, null, null, "Add to Health related issues like drugs, health diagnosis etc. Mostly for health professionals like Doctors.", "Manage Health Internals", 4, new DateTimeOffset(new DateTime(2026, 5, 9, 20, 22, 30, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("423e4568-e89b-12d3-a456-426655440000"));
        }
    }
}
