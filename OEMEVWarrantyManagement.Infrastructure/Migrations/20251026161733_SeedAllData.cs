using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OEMEVWarrantyManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    OrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.OrgId);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationOrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.CampaignId);
                    table.ForeignKey(
                        name: "FK_Campaigns_Organizations_OrganizationOrgId",
                        column: x => x.OrganizationOrgId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationOrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Organizations_OrganizationOrgId",
                        column: x => x.OrganizationOrgId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Employee_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    PartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    OrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.PartId);
                    table.ForeignKey(
                        name: "FK_Parts_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyPolicies",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoveragePeriodMonths = table.Column<int>(type: "int", nullable: false),
                    Conditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationOrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyPolicies", x => x.PolicyId);
                    table.ForeignKey(
                        name: "FK_WarrantyPolicies_Organizations_OrganizationOrgId",
                        column: x => x.OrganizationOrgId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargets",
                columns: table => new
                {
                    CampaignTargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetRefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YearFrom = table.Column<int>(type: "int", nullable: true),
                    YearTo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargets", x => x.CampaignTargetId);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Vin);
                    table.ForeignKey(
                        name: "FK_Vehicles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartOrders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShippedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpectedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PartDelivery = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_PartOrders_Employee_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartOrders_Organizations_ServiceCenterId",
                        column: x => x.ServiceCenterId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.WorkOrderId);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Employee_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignVehicles",
                columns: table => new
                {
                    CampaignVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HandledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignVehicles", x => x.CampaignVehicleId);
                    table.ForeignKey(
                        name: "FK_CampaignVehicles_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignVehicles_Vehicles_Vin",
                        column: x => x.Vin,
                        principalTable: "Vehicles",
                        principalColumn: "Vin");
                });

            migrationBuilder.CreateTable(
                name: "VehicleParts",
                columns: table => new
                {
                    VehiclePartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstalledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UninstalledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleParts", x => x.VehiclePartId);
                    table.ForeignKey(
                        name: "FK_VehicleParts_Vehicles_Vin",
                        column: x => x.Vin,
                        principalTable: "Vehicles",
                        principalColumn: "Vin",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleWarrantyPolicies",
                columns: table => new
                {
                    VehicleWarrantyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleWarrantyPolicies", x => x.VehicleWarrantyId);
                    table.ForeignKey(
                        name: "FK_VehicleWarrantyPolicies_Vehicles_Vin",
                        column: x => x.Vin,
                        principalTable: "Vehicles",
                        principalColumn: "Vin",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleWarrantyPolicies_WarrantyPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "WarrantyPolicies",
                        principalColumn: "PolicyId");
                });

            migrationBuilder.CreateTable(
                name: "PartOrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_PartOrderItems_PartOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "PartOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyClaims",
                columns: table => new
                {
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConfirmDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VehicleWarrantyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    failureDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarrantyPolicyPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyClaims", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_WarrantyClaims_Employee_ConfirmBy",
                        column: x => x.ConfirmBy,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyClaims_Employee_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyClaims_Organizations_ServiceCenterId",
                        column: x => x.ServiceCenterId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyClaims_VehicleWarrantyPolicies_VehicleWarrantyId",
                        column: x => x.VehicleWarrantyId,
                        principalTable: "VehicleWarrantyPolicies",
                        principalColumn: "VehicleWarrantyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyClaims_Vehicles_Vin",
                        column: x => x.Vin,
                        principalTable: "Vehicles",
                        principalColumn: "Vin",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyClaims_WarrantyPolicies_WarrantyPolicyPolicyId",
                        column: x => x.WarrantyPolicyPolicyId,
                        principalTable: "WarrantyPolicies",
                        principalColumn: "PolicyId");
                });

            migrationBuilder.CreateTable(
                name: "BackWarrantyClaim",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    WarrantyClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedByEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackWarrantyClaim", x => new { x.WarrantyClaimId, x.CreatedDate });
                    table.ForeignKey(
                        name: "FK_BackWarrantyClaim_Employee_CreatedByEmployeeId",
                        column: x => x.CreatedByEmployeeId,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackWarrantyClaim_WarrantyClaims_WarrantyClaimId",
                        column: x => x.WarrantyClaimId,
                        principalTable: "WarrantyClaims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimAttachments",
                columns: table => new
                {
                    AttachmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimAttachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_ClaimAttachments_Employee_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimAttachments_WarrantyClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "WarrantyClaims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimParts",
                columns: table => new
                {
                    ClaimPartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumberOld = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumberNew = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimParts", x => x.ClaimPartId);
                    table.ForeignKey(
                        name: "FK_ClaimParts_WarrantyClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "WarrantyClaims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackWarrantyClaim_CreatedByEmployeeId",
                table: "BackWarrantyClaim",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_OrganizationOrgId",
                table: "Campaigns",
                column: "OrganizationOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_CampaignId",
                table: "CampaignTargets",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignVehicles_CampaignId",
                table: "CampaignVehicles",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignVehicles_Vin",
                table: "CampaignVehicles",
                column: "Vin");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimAttachments_ClaimId",
                table: "ClaimAttachments",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimAttachments_UploadedBy",
                table: "ClaimAttachments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimParts_ClaimId",
                table: "ClaimParts",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_OrganizationOrgId",
                table: "Customers",
                column: "OrganizationOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_OrgId",
                table: "Employee",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderItems_OrderId",
                table: "PartOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrders_CreatedBy",
                table: "PartOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrders_ServiceCenterId",
                table: "PartOrders",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_OrgId_Model",
                table: "Parts",
                columns: new[] { "OrgId", "Model" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleParts_Vin",
                table: "VehicleParts",
                column: "Vin");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerId",
                table: "Vehicles",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleWarrantyPolicies_PolicyId",
                table: "VehicleWarrantyPolicies",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleWarrantyPolicies_Vin",
                table: "VehicleWarrantyPolicies",
                column: "Vin");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_ConfirmBy",
                table: "WarrantyClaims",
                column: "ConfirmBy");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_CreatedBy",
                table: "WarrantyClaims",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_ServiceCenterId",
                table: "WarrantyClaims",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_VehicleWarrantyId",
                table: "WarrantyClaims",
                column: "VehicleWarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_Vin",
                table: "WarrantyClaims",
                column: "Vin");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_WarrantyPolicyPolicyId",
                table: "WarrantyClaims",
                column: "WarrantyPolicyPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyPolicies_OrganizationOrgId",
                table: "WarrantyPolicies",
                column: "OrganizationOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_AssignedTo",
                table: "WorkOrders",
                column: "AssignedTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackWarrantyClaim");

            migrationBuilder.DropTable(
                name: "CampaignTargets");

            migrationBuilder.DropTable(
                name: "CampaignVehicles");

            migrationBuilder.DropTable(
                name: "ClaimAttachments");

            migrationBuilder.DropTable(
                name: "ClaimParts");

            migrationBuilder.DropTable(
                name: "PartOrderItems");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "VehicleParts");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "WarrantyClaims");

            migrationBuilder.DropTable(
                name: "PartOrders");

            migrationBuilder.DropTable(
                name: "VehicleWarrantyPolicies");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "WarrantyPolicies");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
