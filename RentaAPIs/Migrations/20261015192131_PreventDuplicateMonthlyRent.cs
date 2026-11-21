using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class PreventDuplicateMonthlyRent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payments_LeaseID_RentalPeriod",
                table: "payments");

            migrationBuilder.CreateIndex(
                name: "IX_payments_LeaseID_RentalPeriod",
                table: "payments",
                columns: new[] { "LeaseID", "RentalPeriod" },
                unique: true,
                filter: "[RentalPeriod] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payments_LeaseID_RentalPeriod",
                table: "payments");

            migrationBuilder.CreateIndex(
                name: "IX_payments_LeaseID_RentalPeriod",
                table: "payments",
                columns: new[] { "LeaseID", "RentalPeriod" });
        }
    }
}
