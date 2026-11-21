using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class updateAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Builtins",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "Furnished",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "Garden",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "Geyser",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "Parking",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "PetFriendly",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "Pool",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "Security",
                table: "amenities");

            migrationBuilder.DropColumn(
                name: "Stove",
                table: "amenities");

            migrationBuilder.RenameColumn(
                name: "Unfurnished",
                table: "amenities",
                newName: "AmenityName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AmenityName",
                table: "amenities",
                newName: "Unfurnished");

            migrationBuilder.AddColumn<string>(
                name: "Builtins",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Furnished",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Garden",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Geyser",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Parking",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PetFriendly",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pool",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Security",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Stove",
                table: "amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
