using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class Update25Aug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leases_landlords_LandlordUserId",
                table: "leases");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_landlords_LandlordUserId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_properties_landlords_LandlordUserId",
                table: "properties");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_landlords_LandlordUserId",
                table: "tenants");

            migrationBuilder.RenameColumn(
                name: "LandlordUserId",
                table: "tenants",
                newName: "LandlordID");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_LandlordUserId",
                table: "tenants",
                newName: "IX_tenants_LandlordID");

            migrationBuilder.RenameColumn(
                name: "LandlordUserId",
                table: "properties",
                newName: "LandlordID");

            migrationBuilder.RenameIndex(
                name: "IX_properties_LandlordUserId",
                table: "properties",
                newName: "IX_properties_LandlordID");

            migrationBuilder.RenameColumn(
                name: "LandlordUserId",
                table: "notifications",
                newName: "LandlordID");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_LandlordUserId",
                table: "notifications",
                newName: "IX_notifications_LandlordID");

            migrationBuilder.RenameColumn(
                name: "LandlordUserId",
                table: "leases",
                newName: "LandlordID");

            migrationBuilder.RenameIndex(
                name: "IX_leases_LandlordUserId",
                table: "leases",
                newName: "IX_leases_LandlordID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "landlords",
                newName: "LandlordID");

            migrationBuilder.AddForeignKey(
                name: "FK_leases_landlords_LandlordID",
                table: "leases",
                column: "LandlordID",
                principalTable: "landlords",
                principalColumn: "LandlordID");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_landlords_LandlordID",
                table: "notifications",
                column: "LandlordID",
                principalTable: "landlords",
                principalColumn: "LandlordID");

            migrationBuilder.AddForeignKey(
                name: "FK_properties_landlords_LandlordID",
                table: "properties",
                column: "LandlordID",
                principalTable: "landlords",
                principalColumn: "LandlordID");

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_landlords_LandlordID",
                table: "tenants",
                column: "LandlordID",
                principalTable: "landlords",
                principalColumn: "LandlordID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leases_landlords_LandlordID",
                table: "leases");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_landlords_LandlordID",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_properties_landlords_LandlordID",
                table: "properties");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_landlords_LandlordID",
                table: "tenants");

            migrationBuilder.RenameColumn(
                name: "LandlordID",
                table: "tenants",
                newName: "LandlordUserId");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_LandlordID",
                table: "tenants",
                newName: "IX_tenants_LandlordUserId");

            migrationBuilder.RenameColumn(
                name: "LandlordID",
                table: "properties",
                newName: "LandlordUserId");

            migrationBuilder.RenameIndex(
                name: "IX_properties_LandlordID",
                table: "properties",
                newName: "IX_properties_LandlordUserId");

            migrationBuilder.RenameColumn(
                name: "LandlordID",
                table: "notifications",
                newName: "LandlordUserId");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_LandlordID",
                table: "notifications",
                newName: "IX_notifications_LandlordUserId");

            migrationBuilder.RenameColumn(
                name: "LandlordID",
                table: "leases",
                newName: "LandlordUserId");

            migrationBuilder.RenameIndex(
                name: "IX_leases_LandlordID",
                table: "leases",
                newName: "IX_leases_LandlordUserId");

            migrationBuilder.RenameColumn(
                name: "LandlordID",
                table: "landlords",
                newName: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_leases_landlords_LandlordUserId",
                table: "leases",
                column: "LandlordUserId",
                principalTable: "landlords",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_landlords_LandlordUserId",
                table: "notifications",
                column: "LandlordUserId",
                principalTable: "landlords",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_properties_landlords_LandlordUserId",
                table: "properties",
                column: "LandlordUserId",
                principalTable: "landlords",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_landlords_LandlordUserId",
                table: "tenants",
                column: "LandlordUserId",
                principalTable: "landlords",
                principalColumn: "UserId");
        }
    }
}
