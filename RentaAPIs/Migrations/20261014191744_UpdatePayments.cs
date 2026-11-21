using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payments_LeaseID",
                table: "payments");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DaysLate",
                table: "payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentalPeriod",
                table: "payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_LeaseID_RentalPeriod",
                table: "payments",
                columns: new[] { "LeaseID", "RentalPeriod" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payments_LeaseID_RentalPeriod",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "DaysLate",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "RentalPeriod",
                table: "payments");

            migrationBuilder.CreateIndex(
                name: "IX_payments_LeaseID",
                table: "payments",
                column: "LeaseID");
        }
    }
}
