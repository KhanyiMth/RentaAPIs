using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentaAPIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_documents_leases_LeaseID",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_maintenanceRequests_MaintenanceRequestID",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_properties_PropertyID",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_tenants_TenantID",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "BlobName",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "documents");

            migrationBuilder.RenameColumn(
                name: "TenantID",
                table: "documents",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "PropertyID",
                table: "documents",
                newName: "PropertyId");

            migrationBuilder.RenameColumn(
                name: "MaintenanceRequestID",
                table: "documents",
                newName: "MaintenanceRequestId");

            migrationBuilder.RenameColumn(
                name: "LeaseID",
                table: "documents",
                newName: "LeaseId");

            migrationBuilder.RenameColumn(
                name: "DocumentID",
                table: "documents",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_documents_TenantID",
                table: "documents",
                newName: "IX_documents_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_documents_PropertyID",
                table: "documents",
                newName: "IX_documents_PropertyId");

            migrationBuilder.RenameIndex(
                name: "IX_documents_MaintenanceRequestID",
                table: "documents",
                newName: "IX_documents_MaintenanceRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_documents_LeaseID",
                table: "documents",
                newName: "IX_documents_LeaseId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadedBy",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "PropertyId",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaintenanceRequestId",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LeaseId",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "documents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "documents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "documents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "documents",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<Guid>(
                name: "LeaseId1",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaintenanceRequestMaintenanceReqId",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId1",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId1",
                table: "documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_documents_LeaseId1",
                table: "documents",
                column: "LeaseId1");

            migrationBuilder.CreateIndex(
                name: "IX_documents_MaintenanceRequestMaintenanceReqId",
                table: "documents",
                column: "MaintenanceRequestMaintenanceReqId");

            migrationBuilder.CreateIndex(
                name: "IX_documents_PropertyId1",
                table: "documents",
                column: "PropertyId1");

            migrationBuilder.CreateIndex(
                name: "IX_documents_TenantId1",
                table: "documents",
                column: "TenantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_leases_LeaseId",
                table: "documents",
                column: "LeaseId",
                principalTable: "leases",
                principalColumn: "LeaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_leases_LeaseId1",
                table: "documents",
                column: "LeaseId1",
                principalTable: "leases",
                principalColumn: "LeaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_maintenanceRequests_MaintenanceRequestId",
                table: "documents",
                column: "MaintenanceRequestId",
                principalTable: "maintenanceRequests",
                principalColumn: "MaintenanceReqId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_maintenanceRequests_MaintenanceRequestMaintenanceReqId",
                table: "documents",
                column: "MaintenanceRequestMaintenanceReqId",
                principalTable: "maintenanceRequests",
                principalColumn: "MaintenanceReqId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_properties_PropertyId",
                table: "documents",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_properties_PropertyId1",
                table: "documents",
                column: "PropertyId1",
                principalTable: "properties",
                principalColumn: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_tenants_TenantId",
                table: "documents",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_tenants_TenantId1",
                table: "documents",
                column: "TenantId1",
                principalTable: "tenants",
                principalColumn: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_documents_leases_LeaseId",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_leases_LeaseId1",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_maintenanceRequests_MaintenanceRequestId",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_maintenanceRequests_MaintenanceRequestMaintenanceReqId",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_properties_PropertyId",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_properties_PropertyId1",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_tenants_TenantId",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_tenants_TenantId1",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "IX_documents_LeaseId1",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "IX_documents_MaintenanceRequestMaintenanceReqId",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "IX_documents_PropertyId1",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "IX_documents_TenantId1",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "FileData",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "LeaseId1",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "MaintenanceRequestMaintenanceReqId",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "PropertyId1",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "TenantId1",
                table: "documents");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "documents",
                newName: "TenantID");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "documents",
                newName: "PropertyID");

            migrationBuilder.RenameColumn(
                name: "MaintenanceRequestId",
                table: "documents",
                newName: "MaintenanceRequestID");

            migrationBuilder.RenameColumn(
                name: "LeaseId",
                table: "documents",
                newName: "LeaseID");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "documents",
                newName: "DocumentID");

            migrationBuilder.RenameIndex(
                name: "IX_documents_TenantId",
                table: "documents",
                newName: "IX_documents_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_documents_PropertyId",
                table: "documents",
                newName: "IX_documents_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_documents_MaintenanceRequestId",
                table: "documents",
                newName: "IX_documents_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_documents_LeaseId",
                table: "documents",
                newName: "IX_documents_LeaseID");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadedBy",
                table: "documents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantID",
                table: "documents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PropertyID",
                table: "documents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MaintenanceRequestID",
                table: "documents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LeaseID",
                table: "documents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "documents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "documents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "documents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                table: "documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_leases_LeaseID",
                table: "documents",
                column: "LeaseID",
                principalTable: "leases",
                principalColumn: "LeaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_maintenanceRequests_MaintenanceRequestID",
                table: "documents",
                column: "MaintenanceRequestID",
                principalTable: "maintenanceRequests",
                principalColumn: "MaintenanceReqId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_properties_PropertyID",
                table: "documents",
                column: "PropertyID",
                principalTable: "properties",
                principalColumn: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_tenants_TenantID",
                table: "documents",
                column: "TenantID",
                principalTable: "tenants",
                principalColumn: "TenantId");
        }
    }
}
