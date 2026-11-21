using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class AddRentaVisionToMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UnitID",
                table: "maintenanceRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedAt",
                table: "maintenanceRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "AICategoryConfirmed",
                table: "maintenanceRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "AIConfidence",
                table: "maintenanceRequests",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIModelVersion",
                table: "maintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIPredictedCategory",
                table: "maintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceImagePath",
                table: "maintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AICategoryConfirmed",
                table: "maintenanceRequests");

            migrationBuilder.DropColumn(
                name: "AIConfidence",
                table: "maintenanceRequests");

            migrationBuilder.DropColumn(
                name: "AIModelVersion",
                table: "maintenanceRequests");

            migrationBuilder.DropColumn(
                name: "AIPredictedCategory",
                table: "maintenanceRequests");

            migrationBuilder.DropColumn(
                name: "MaintenanceImagePath",
                table: "maintenanceRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitID",
                table: "maintenanceRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedAt",
                table: "maintenanceRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
