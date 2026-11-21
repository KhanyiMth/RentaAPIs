using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class updateTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyExpenses",
                table: "tenants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "amenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pool = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Garden = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Geyser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stove = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Builtins = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unfurnished = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Furnished = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parking = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetFriendly = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Security = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyAmenities",
                columns: table => new
                {
                    AmenitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertiesPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAmenities", x => new { x.AmenitiesId, x.PropertiesPropertyId });
                    table.ForeignKey(
                        name: "FK_PropertyAmenities_amenities_AmenitiesId",
                        column: x => x.AmenitiesId,
                        principalTable: "amenities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyAmenities_properties_PropertiesPropertyId",
                        column: x => x.PropertiesPropertyId,
                        principalTable: "properties",
                        principalColumn: "PropertyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAmenities_PropertiesPropertyId",
                table: "PropertyAmenities",
                column: "PropertiesPropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyAmenities");

            migrationBuilder.DropTable(
                name: "amenities");

            migrationBuilder.DropColumn(
                name: "MonthlyExpenses",
                table: "tenants");
        }
    }
}
