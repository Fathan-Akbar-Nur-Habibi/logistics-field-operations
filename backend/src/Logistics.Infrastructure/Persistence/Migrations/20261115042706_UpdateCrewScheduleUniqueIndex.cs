using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCrewScheduleUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_crew_schedules_EmployeeId_ShiftId_WorkDate",
                table: "crew_schedules");

            migrationBuilder.CreateIndex(
                name: "IX_crew_schedules_EmployeeId_WorkDate",
                table: "crew_schedules",
                columns: new[] { "EmployeeId", "WorkDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_crew_schedules_EmployeeId_WorkDate",
                table: "crew_schedules");

            migrationBuilder.CreateIndex(
                name: "IX_crew_schedules_EmployeeId_ShiftId_WorkDate",
                table: "crew_schedules",
                columns: new[] { "EmployeeId", "ShiftId", "WorkDate" },
                unique: true);
        }
    }
}
