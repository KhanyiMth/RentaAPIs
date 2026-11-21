using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class AddEligibilityPrediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IDNumber",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AppScore",
                table: "tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDueDate",
                table: "payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "propertyApplications",
                columns: table => new
                {
                    ApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propertyApplications", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK_propertyApplications_properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "properties",
                        principalColumn: "PropertyId");
                    table.ForeignKey(
                        name: "FK_propertyApplications_tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "propertyEligibilityPrediction",
                columns: table => new
                {
                    PredicitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Probability = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EligibilityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModelVersion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propertyEligibilityPrediction", x => x.PredicitionID);
                    table.ForeignKey(
                        name: "FK_propertyEligibilityPrediction_properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "properties",
                        principalColumn: "PropertyId");
                    table.ForeignKey(
                        name: "FK_propertyEligibilityPrediction_tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_propertyApplications_PropertyID",
                table: "propertyApplications",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_propertyApplications_TenantID",
                table: "propertyApplications",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_propertyEligibilityPrediction_PropertyID",
                table: "propertyEligibilityPrediction",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_propertyEligibilityPrediction_TenantID",
                table: "propertyEligibilityPrediction",
                column: "TenantID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "propertyApplications");

            migrationBuilder.DropTable(
                name: "propertyEligibilityPrediction");

            migrationBuilder.DropColumn(
                name: "IDNumber",
                table: "users");

            migrationBuilder.DropColumn(
                name: "AppScore",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "PaymentDueDate",
                table: "payments");
        }
    }
}
