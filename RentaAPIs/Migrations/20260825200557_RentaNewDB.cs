using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class RentaNewDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_UserID",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_properties_users_UserId",
                table: "properties");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_users_UserId",
                table: "tenants");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_properties_UserId",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "IX_notifications_UserID",
                table: "notifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "tenants",
                newName: "LandlordUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "tenants",
                newName: "CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_UserId",
                table: "tenants",
                newName: "IX_tenants_LandlordUserId");

            migrationBuilder.AddColumn<string>(
                name: "Cellphone",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IDNumber",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LandlordUserId",
                table: "properties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LandlordUserId",
                table: "notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LandlordUserId",
                table: "leases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "landlords",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_landlords", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_properties_LandlordUserId",
                table: "properties",
                column: "LandlordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_LandlordUserId",
                table: "notifications",
                column: "LandlordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_leases_LandlordUserId",
                table: "leases",
                column: "LandlordUserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "landlords");

            migrationBuilder.DropIndex(
                name: "IX_properties_LandlordUserId",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "IX_notifications_LandlordUserId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_leases_LandlordUserId",
                table: "leases");

            migrationBuilder.DropColumn(
                name: "Cellphone",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "IDNumber",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "LandlordUserId",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "LandlordUserId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "LandlordUserId",
                table: "leases");

            migrationBuilder.RenameColumn(
                name: "LandlordUserId",
                table: "tenants",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "tenants",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_LandlordUserId",
                table: "tenants",
                newName: "IX_tenants_UserId");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_properties_UserId",
                table: "properties",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserID",
                table: "notifications",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_UserID",
                table: "notifications",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_properties_users_UserId",
                table: "properties",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_users_UserId",
                table: "tenants",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId");
        }
    }
}
