using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTenantApplication1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenantapplications",
                columns: table => new
                {
                    TenantApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvertisementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AppScore = table.Column<int>(type: "int", nullable: false),
                    AcceptanceScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PredictionLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenantapplications", x => x.TenantApplicationId);
                    table.ForeignKey(
                        name: "FK_tenantapplications_advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "advertisements",
                        principalColumn: "AdvertisementId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenantapplications_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenantapplications_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenantapplications_units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "units",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenantapplications_AdvertisementId",
                table: "tenantapplications",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_tenantapplications_PropertyId",
                table: "tenantapplications",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_tenantapplications_TenantId",
                table: "tenantapplications",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenantapplications_UnitId",
                table: "tenantapplications",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenantapplications");
        }
    }
}
