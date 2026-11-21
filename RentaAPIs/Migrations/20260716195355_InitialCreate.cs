using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ActionUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_notifications_users_UserID",
                        column: x => x.UserID,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bedrooms = table.Column<int>(type: "int", nullable: false),
                    Bathrooms = table.Column<int>(type: "int", nullable: false),
                    RentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_properties_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_tenants_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitNumber = table.Column<int>(type: "int", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    Bedrooms = table.Column<int>(type: "int", nullable: false),
                    Bathrooms = table.Column<int>(type: "int", nullable: false),
                    RentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isOccupied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_units_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "PropertyId");
                });

            migrationBuilder.CreateTable(
                name: "leases",
                columns: table => new
                {
                    LeaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentFrequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Terms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSignedByLandlord = table.Column<bool>(type: "bit", nullable: false),
                    IsSignedByTenant = table.Column<bool>(type: "bit", nullable: false),
                    TenantSignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LandlordSignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaseDocumentURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leases", x => x.LeaseId);
                    table.ForeignKey(
                        name: "FK_leases_tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "tenants",
                        principalColumn: "TenantId");
                    table.ForeignKey(
                        name: "FK_leases_units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "maintenanceRequests",
                columns: table => new
                {
                    MaintenanceReqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenanceRequests", x => x.MaintenanceReqId);
                    table.ForeignKey(
                        name: "FK_maintenanceRequests_properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "properties",
                        principalColumn: "PropertyId");
                    table.ForeignKey(
                        name: "FK_maintenanceRequests_tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "tenants",
                        principalColumn: "TenantId");
                    table.ForeignKey(
                        name: "FK_maintenanceRequests_units_UnitID",
                        column: x => x.UnitID,
                        principalTable: "units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_payments_leases_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "leases",
                        principalColumn: "LeaseId");
                    table.ForeignKey(
                        name: "FK_payments_tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    DocumentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_documents_leases_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "leases",
                        principalColumn: "LeaseId");
                    table.ForeignKey(
                        name: "FK_documents_maintenanceRequests_MaintenanceRequestID",
                        column: x => x.MaintenanceRequestID,
                        principalTable: "maintenanceRequests",
                        principalColumn: "MaintenanceReqId");
                    table.ForeignKey(
                        name: "FK_documents_properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "properties",
                        principalColumn: "PropertyId");
                    table.ForeignKey(
                        name: "FK_documents_tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_documents_LeaseID",
                table: "documents",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_documents_MaintenanceRequestID",
                table: "documents",
                column: "MaintenanceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_documents_PropertyID",
                table: "documents",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_documents_TenantID",
                table: "documents",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_leases_TenantID",
                table: "leases",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_leases_UnitId",
                table: "leases",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_maintenanceRequests_PropertyID",
                table: "maintenanceRequests",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_maintenanceRequests_TenantID",
                table: "maintenanceRequests",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_maintenanceRequests_UnitID",
                table: "maintenanceRequests",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserID",
                table: "notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_payments_LeaseID",
                table: "payments",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_payments_TenantID",
                table: "payments",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_properties_UserId",
                table: "properties",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_UserId",
                table: "tenants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_units_PropertyId",
                table: "units",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "maintenanceRequests");

            migrationBuilder.DropTable(
                name: "leases");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "properties");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
