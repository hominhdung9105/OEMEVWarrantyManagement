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
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
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
                    OrganizationOrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyPolicies", x => x.PolicyId);
                    table.ForeignKey(
                        name: "FK_WarrantyPolicies_Organizations_OrganizationOrgId",
                        column: x => x.OrganizationOrgId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId");
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
                name: "Campaigns",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacementPartModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAffectedVehicles = table.Column<int>(type: "int", nullable: false),
                    PendingVehicles = table.Column<int>(type: "int", nullable: false),
                    InProgressVehicles = table.Column<int>(type: "int", nullable: false),
                    CompletedVehicles = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.CampaignId);
                    table.ForeignKey(
                        name: "FK_Campaigns_Employee_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
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
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Slot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_Organizations_ServiceCenterId",
                        column: x => x.ServiceCenterId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Vehicles_Vin",
                        column: x => x.Vin,
                        principalTable: "Vehicles",
                        principalColumn: "Vin",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehiclePartHistories",
                columns: table => new
                {
                    VehiclePartHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InstalledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UninstalledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarrantyPeriodMonths = table.Column<int>(type: "int", nullable: false),
                    WarrantyEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclePartHistories", x => x.VehiclePartHistoryId);
                    table.ForeignKey(
                        name: "FK_VehiclePartHistories_Organizations_ServiceCenterId",
                        column: x => x.ServiceCenterId,
                        principalTable: "Organizations",
                        principalColumn: "OrgId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehiclePartHistories_Vehicles_Vin",
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
                name: "CampaignNotifications",
                columns: table => new
                {
                    CampaignNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailSentCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastEmailSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstEmailSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignNotifications", x => x.CampaignNotificationId);
                    table.ForeignKey(
                        name: "FK_CampaignNotifications_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignNotifications_Vehicles_Vin",
                        column: x => x.Vin,
                        principalTable: "Vehicles",
                        principalColumn: "Vin",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignVehicles",
                columns: table => new
                {
                    CampaignVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NewSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "PartOrderDiscrepancyResolutions",
                columns: table => new
                {
                    ResolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResolvedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverallNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrderDiscrepancyResolutions", x => x.ResolutionId);
                    table.ForeignKey(
                        name: "FK_PartOrderDiscrepancyResolutions_Employee_ResolvedBy",
                        column: x => x.ResolvedBy,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartOrderDiscrepancyResolutions_PartOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "PartOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartOrderIssues",
                columns: table => new
                {
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReasonDetail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrderIssues", x => x.IssueId);
                    table.ForeignKey(
                        name: "FK_PartOrderIssues_Employee_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Employee",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartOrderIssues_PartOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "PartOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "PartOrderReceipts",
                columns: table => new
                {
                    ReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrderReceipts", x => x.ReceiptId);
                    table.ForeignKey(
                        name: "FK_PartOrderReceipts_PartOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "PartOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartOrderShipments",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShippedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrderShipments", x => x.ShipmentId);
                    table.ForeignKey(
                        name: "FK_PartOrderShipments_PartOrders_OrderId",
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
                    DenialReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DenialReasonDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "CampaignVehicleReplacements",
                columns: table => new
                {
                    CampaignVehicleReplacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReplacedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignVehicleReplacements", x => x.CampaignVehicleReplacementId);
                    table.ForeignKey(
                        name: "FK_CampaignVehicleReplacements_CampaignVehicles_CampaignVehicleId",
                        column: x => x.CampaignVehicleId,
                        principalTable: "CampaignVehicles",
                        principalColumn: "CampaignVehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartOrderDiscrepancyDetails",
                columns: table => new
                {
                    DetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiscrepancyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResponsibleParty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOrderDiscrepancyDetails", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_PartOrderDiscrepancyDetails_PartOrderDiscrepancyResolutions_ResolutionId",
                        column: x => x.ResolutionId,
                        principalTable: "PartOrderDiscrepancyResolutions",
                        principalColumn: "ResolutionId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Appointments_ServiceCenterId",
                table: "Appointments",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Vin",
                table: "Appointments",
                column: "Vin");

            migrationBuilder.CreateIndex(
                name: "IX_BackWarrantyClaim_CreatedByEmployeeId",
                table: "BackWarrantyClaim",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignNotifications_CampaignId_Vin",
                table: "CampaignNotifications",
                columns: new[] { "CampaignId", "Vin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignNotifications_IsCompleted",
                table: "CampaignNotifications",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignNotifications_LastEmailSentAt",
                table: "CampaignNotifications",
                column: "LastEmailSentAt");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignNotifications_Vin",
                table: "CampaignNotifications",
                column: "Vin");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CreatedBy",
                table: "Campaigns",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignVehicleReplacements_CampaignVehicleId",
                table: "CampaignVehicleReplacements",
                column: "CampaignVehicleId");

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
                name: "IX_PartOrderDiscrepancyDetails_ResolutionId",
                table: "PartOrderDiscrepancyDetails",
                column: "ResolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderDiscrepancyDetails_SerialNumber",
                table: "PartOrderDiscrepancyDetails",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderDiscrepancyResolutions_OrderId",
                table: "PartOrderDiscrepancyResolutions",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderDiscrepancyResolutions_ResolvedBy",
                table: "PartOrderDiscrepancyResolutions",
                column: "ResolvedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderDiscrepancyResolutions_Status",
                table: "PartOrderDiscrepancyResolutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderIssues_CreatedBy",
                table: "PartOrderIssues",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderIssues_OrderId",
                table: "PartOrderIssues",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderItems_OrderId",
                table: "PartOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderReceipts_OrderId_SerialNumber",
                table: "PartOrderReceipts",
                columns: new[] { "OrderId", "SerialNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_PartOrders_CreatedBy",
                table: "PartOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrders_ServiceCenterId",
                table: "PartOrders",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PartOrderShipments_OrderId_SerialNumber",
                table: "PartOrderShipments",
                columns: new[] { "OrderId", "SerialNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Parts_OrgId_Model",
                table: "Parts",
                columns: new[] { "OrgId", "Model" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePartHistories_ServiceCenterId",
                table: "VehiclePartHistories",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePartHistories_Vin_SerialNumber",
                table: "VehiclePartHistories",
                columns: new[] { "Vin", "SerialNumber" });

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
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "BackWarrantyClaim");

            migrationBuilder.DropTable(
                name: "CampaignNotifications");

            migrationBuilder.DropTable(
                name: "CampaignVehicleReplacements");

            migrationBuilder.DropTable(
                name: "ClaimAttachments");

            migrationBuilder.DropTable(
                name: "ClaimParts");

            migrationBuilder.DropTable(
                name: "PartOrderDiscrepancyDetails");

            migrationBuilder.DropTable(
                name: "PartOrderIssues");

            migrationBuilder.DropTable(
                name: "PartOrderItems");

            migrationBuilder.DropTable(
                name: "PartOrderReceipts");

            migrationBuilder.DropTable(
                name: "PartOrderShipments");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "VehiclePartHistories");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "CampaignVehicles");

            migrationBuilder.DropTable(
                name: "WarrantyClaims");

            migrationBuilder.DropTable(
                name: "PartOrderDiscrepancyResolutions");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "VehicleWarrantyPolicies");

            migrationBuilder.DropTable(
                name: "PartOrders");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "WarrantyPolicies");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
