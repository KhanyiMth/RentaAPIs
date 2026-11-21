using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLease06Sep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "leases",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "leases",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextPaymentDate",
                table: "leases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_leases_PropertyID",
                table: "leases",
                column: "PropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_leases_properties_PropertyID",
                table: "leases",
                column: "PropertyID",
                principalTable: "properties",
                principalColumn: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leases_properties_PropertyID",
                table: "leases");

            migrationBuilder.DropIndex(
                name: "IX_leases_PropertyID",
                table: "leases");

            migrationBuilder.DropColumn(
                name: "NextPaymentDate",
                table: "leases");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "leases",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "leases",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
