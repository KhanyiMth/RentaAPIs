using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_properties_landlords_LandlordID",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "properties");

            migrationBuilder.RenameColumn(
                name: "LandlordID",
                table: "properties",
                newName: "LandlordId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "properties",
                newName: "Suburb");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "properties",
                newName: "StreetNumber");

            migrationBuilder.RenameIndex(
                name: "IX_properties_LandlordID",
                table: "properties",
                newName: "IX_properties_LandlordId");

            migrationBuilder.AddColumn<decimal>(
                name: "Deposit",
                table: "properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ViewingAvailabilities",
                columns: table => new
                {
                    ViewingAvailabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SlotDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewingAvailabilities", x => x.ViewingAvailabilityId);
                    table.ForeignKey(
                        name: "FK_ViewingAvailabilities_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewingBookings",
                columns: table => new
                {
                    ViewingBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewingAvailabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewingBookings", x => x.ViewingBookingId);
                    table.ForeignKey(
                        name: "FK_ViewingBookings_ViewingAvailabilities_ViewingAvailabilityId",
                        column: x => x.ViewingAvailabilityId,
                        principalTable: "ViewingAvailabilities",
                        principalColumn: "ViewingAvailabilityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViewingBookings_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViewingAvailabilities_PropertyId",
                table: "ViewingAvailabilities",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingBookings_PropertyId",
                table: "ViewingBookings",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingBookings_ViewingAvailabilityId",
                table: "ViewingBookings",
                column: "ViewingAvailabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_properties_landlords_LandlordId",
                table: "properties",
                column: "LandlordId",
                principalTable: "landlords",
                principalColumn: "LandlordID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_properties_landlords_LandlordId",
                table: "properties");

            migrationBuilder.DropTable(
                name: "ViewingBookings");

            migrationBuilder.DropTable(
                name: "ViewingAvailabilities");

            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "properties");

            migrationBuilder.RenameColumn(
                name: "LandlordId",
                table: "properties",
                newName: "LandlordID");

            migrationBuilder.RenameColumn(
                name: "Suburb",
                table: "properties",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "StreetNumber",
                table: "properties",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_properties_LandlordId",
                table: "properties",
                newName: "IX_properties_LandlordID");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "properties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_properties_landlords_LandlordID",
                table: "properties",
                column: "LandlordID",
                principalTable: "landlords",
                principalColumn: "LandlordID");
        }
    }
}
