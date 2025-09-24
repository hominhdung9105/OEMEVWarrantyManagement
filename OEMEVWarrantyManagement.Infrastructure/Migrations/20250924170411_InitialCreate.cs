using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OEMEVWarrantyManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recall",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EVMStaffId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartReplacementId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfCars = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recall", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeAppointments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeAppointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyPolicies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PeriodInMonths = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Coverage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Conditions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkPlaces",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPlaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartTypeModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PartTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTypeModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartTypeModels_PartTypes_PartTypeId",
                        column: x => x.PartTypeId,
                        principalTable: "PartTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkPlacesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_WorkPlaces_WorkPlacesId",
                        column: x => x.WorkPlacesId,
                        principalTable: "WorkPlaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecallPartsReplacements",
                columns: table => new
                {
                    RecallId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartTypeModelId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecallPartsReplacements", x => new { x.RecallId, x.PartTypeModelId });
                    table.ForeignKey(
                        name: "FK_RecallPartsReplacements_PartTypeModels_PartTypeModelId",
                        column: x => x.PartTypeModelId,
                        principalTable: "PartTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecallPartsReplacements_Recall_RecallId",
                        column: x => x.RecallId,
                        principalTable: "Recall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkPlacePartTypeModels(PartInStock)",
                columns: table => new
                {
                    WorkPlacesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartTypeModelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPlacePartTypeModels(PartInStock)", x => new { x.WorkPlacesId, x.PartTypeModelId });
                    table.ForeignKey(
                        name: "FK_WorkPlacePartTypeModels(PartInStock)_PartTypeModels_PartTypeModelId",
                        column: x => x.PartTypeModelId,
                        principalTable: "PartTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkPlacePartTypeModels(PartInStock)_WorkPlaces_WorkPlacesId",
                        column: x => x.WorkPlacesId,
                        principalTable: "WorkPlaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SCStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SCTechID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignment_Employee_SCStaffId",
                        column: x => x.SCStaffId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignment_Employee_SCTechID",
                        column: x => x.SCTechID,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarConditionCurrents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarConditionCurrents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarConditionCurrents_Employee_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffSend = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffReceive = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateSend = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReceive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkPlaceId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryParts_Employee_StaffReceive",
                        column: x => x.StaffReceive,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryParts_Employee_StaffSend",
                        column: x => x.StaffSend,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryParts_WorkPlaces_WorkPlaceId",
                        column: x => x.WorkPlaceId,
                        principalTable: "WorkPlaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SCStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EVMStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestParts_Employee_EVMStaffId",
                        column: x => x.EVMStaffId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestParts_Employee_SCStaffId",
                        column: x => x.SCStaffId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    FilePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CarConditionCurrentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => new { x.CarConditionCurrentId, x.FilePath });
                    table.ForeignKey(
                        name: "FK_Images_CarConditionCurrents_CarConditionCurrentId",
                        column: x => x.CarConditionCurrentId,
                        principalTable: "CarConditionCurrents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarInfo",
                columns: table => new
                {
                    VIN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarInfo", x => x.VIN);
                    table.ForeignKey(
                        name: "FK_CarInfo_CarModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "CarModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarInfo_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartTypeModelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestPartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliveryPartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parts_DeliveryParts_DeliveryPartId",
                        column: x => x.DeliveryPartId,
                        principalTable: "DeliveryParts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parts_PartTypeModels_PartTypeModelId",
                        column: x => x.PartTypeModelId,
                        principalTable: "PartTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parts_RequestParts_RequestPartsId",
                        column: x => x.RequestPartsId,
                        principalTable: "RequestParts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeAppointmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(17)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_CarInfo_VIN",
                        column: x => x.VIN,
                        principalTable: "CarInfo",
                        principalColumn: "VIN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_TypeAppointments_TypeAppointmentId",
                        column: x => x.TypeAppointmentId,
                        principalTable: "TypeAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecallHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(17)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecallId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmpoloyeeSCStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecallHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecallHistories_CarInfo_VIN",
                        column: x => x.VIN,
                        principalTable: "CarInfo",
                        principalColumn: "VIN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecallHistories_Employee_EmpoloyeeSCStaffId",
                        column: x => x.EmpoloyeeSCStaffId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecallHistories_Recall_RecallId",
                        column: x => x.RecallId,
                        principalTable: "Recall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(17)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarrantyPolicyId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarrantyRecord_CarInfo_VIN",
                        column: x => x.VIN,
                        principalTable: "CarInfo",
                        principalColumn: "VIN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarrantyRecord_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyRecord_WarrantyPolicies_WarrantyPolicyId",
                        column: x => x.WarrantyPolicyId,
                        principalTable: "WarrantyPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(17)", nullable: false),
                    SCStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EVMStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CarConditionCurrentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarrantyRequests_CarConditionCurrents_CarConditionCurrentId",
                        column: x => x.CarConditionCurrentId,
                        principalTable: "CarConditionCurrents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarrantyRequests_CarInfo_VIN",
                        column: x => x.VIN,
                        principalTable: "CarInfo",
                        principalColumn: "VIN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyRequests_Employee_EVMStaffId",
                        column: x => x.EVMStaffId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyRequests_Employee_SCStaffId",
                        column: x => x.SCStaffId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecallHistoryEmployees",
                columns: table => new
                {
                    RecallHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecallHistoryEmployees", x => new { x.RecallHistoryId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_RecallHistoryEmployees_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecallHistoryEmployees_RecallHistories_RecallHistoryId",
                        column: x => x.RecallHistoryId,
                        principalTable: "RecallHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Warranties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestWarrantyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarrantyRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeSCStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warranties_Employee_EmployeeSCStaffId",
                        column: x => x.EmployeeSCStaffId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warranties_WarrantyRecord_WarrantyRecordId",
                        column: x => x.WarrantyRecordId,
                        principalTable: "WarrantyRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Warranties_WarrantyRequests_RequestWarrantyId",
                        column: x => x.RequestWarrantyId,
                        principalTable: "WarrantyRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartsReplacement",
                columns: table => new
                {
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PartTypeModelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WarrantyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecallHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsReplacement", x => x.SerialNumber);
                    table.ForeignKey(
                        name: "FK_PartsReplacement_PartTypeModels_PartTypeModelId",
                        column: x => x.PartTypeModelId,
                        principalTable: "PartTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartsReplacement_RecallHistories_RecallHistoryId",
                        column: x => x.RecallHistoryId,
                        principalTable: "RecallHistories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartsReplacement_Warranties_WarrantyId",
                        column: x => x.WarrantyId,
                        principalTable: "Warranties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarrantyEmployees",
                columns: table => new
                {
                    WarrantyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyEmployees", x => new { x.WarrantyId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_WarrantyEmployees_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyEmployees_Warranties_WarrantyId",
                        column: x => x.WarrantyId,
                        principalTable: "Warranties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyPartsReplacement",
                columns: table => new
                {
                    WarrantyId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: false),
                    PartsReplacementId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyPartsReplacement", x => new { x.WarrantyId, x.PartsReplacementId });
                    table.ForeignKey(
                        name: "FK_WarrantyPartsReplacement_PartsReplacement_PartsReplacementId",
                        column: x => x.PartsReplacementId,
                        principalTable: "PartsReplacement",
                        principalColumn: "SerialNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarrantyPartsReplacement_Warranties_WarrantyId",
                        column: x => x.WarrantyId,
                        principalTable: "Warranties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerId",
                table: "Appointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TypeAppointmentId",
                table: "Appointments",
                column: "TypeAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VIN",
                table: "Appointments",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_SCStaffId",
                table: "Assignment",
                column: "SCStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_SCTechID",
                table: "Assignment",
                column: "SCTechID");

            migrationBuilder.CreateIndex(
                name: "IX_CarConditionCurrents_TechnicianId",
                table: "CarConditionCurrents",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_CarInfo_CustomerId",
                table: "CarInfo",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarInfo_ModelId",
                table: "CarInfo",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmployeeId",
                table: "Customers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryParts_StaffReceive",
                table: "DeliveryParts",
                column: "StaffReceive");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryParts_StaffSend",
                table: "DeliveryParts",
                column: "StaffSend");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryParts_WorkPlaceId",
                table: "DeliveryParts",
                column: "WorkPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_RoleId",
                table: "Employee",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_WorkPlacesId",
                table: "Employee",
                column: "WorkPlacesId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_DeliveryPartId",
                table: "Parts",
                column: "DeliveryPartId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartTypeModelId",
                table: "Parts",
                column: "PartTypeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_RequestPartsId",
                table: "Parts",
                column: "RequestPartsId");

            migrationBuilder.CreateIndex(
                name: "IX_PartsReplacement_PartTypeModelId",
                table: "PartsReplacement",
                column: "PartTypeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PartsReplacement_RecallHistoryId",
                table: "PartsReplacement",
                column: "RecallHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PartsReplacement_WarrantyId",
                table: "PartsReplacement",
                column: "WarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartTypeModels_PartTypeId",
                table: "PartTypeModels",
                column: "PartTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecallHistories_EmpoloyeeSCStaffId",
                table: "RecallHistories",
                column: "EmpoloyeeSCStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_RecallHistories_RecallId",
                table: "RecallHistories",
                column: "RecallId");

            migrationBuilder.CreateIndex(
                name: "IX_RecallHistories_VIN",
                table: "RecallHistories",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_RecallHistoryEmployees_EmployeeId",
                table: "RecallHistoryEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecallPartsReplacements_PartTypeModelId",
                table: "RecallPartsReplacements",
                column: "PartTypeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestParts_EVMStaffId",
                table: "RequestParts",
                column: "EVMStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestParts_SCStaffId",
                table: "RequestParts",
                column: "SCStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_EmployeeSCStaffId",
                table: "Warranties",
                column: "EmployeeSCStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_RequestWarrantyId",
                table: "Warranties",
                column: "RequestWarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_WarrantyRecordId",
                table: "Warranties",
                column: "WarrantyRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyEmployees_EmployeeId",
                table: "WarrantyEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyPartsReplacement_PartsReplacementId",
                table: "WarrantyPartsReplacement",
                column: "PartsReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyRecord_CustomerId",
                table: "WarrantyRecord",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyRecord_VIN",
                table: "WarrantyRecord",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyRecord_WarrantyPolicyId",
                table: "WarrantyRecord",
                column: "WarrantyPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyRequests_CarConditionCurrentId",
                table: "WarrantyRequests",
                column: "CarConditionCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyRequests_EVMStaffId",
                table: "WarrantyRequests",
                column: "EVMStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyRequests_SCStaffId",
                table: "WarrantyRequests",
                column: "SCStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyRequests_VIN",
                table: "WarrantyRequests",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlacePartTypeModels(PartInStock)_PartTypeModelId",
                table: "WorkPlacePartTypeModels(PartInStock)",
                column: "PartTypeModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Assignment");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "RecallHistoryEmployees");

            migrationBuilder.DropTable(
                name: "RecallPartsReplacements");

            migrationBuilder.DropTable(
                name: "WarrantyEmployees");

            migrationBuilder.DropTable(
                name: "WarrantyPartsReplacement");

            migrationBuilder.DropTable(
                name: "WorkPlacePartTypeModels(PartInStock)");

            migrationBuilder.DropTable(
                name: "TypeAppointments");

            migrationBuilder.DropTable(
                name: "DeliveryParts");

            migrationBuilder.DropTable(
                name: "RequestParts");

            migrationBuilder.DropTable(
                name: "PartsReplacement");

            migrationBuilder.DropTable(
                name: "PartTypeModels");

            migrationBuilder.DropTable(
                name: "RecallHistories");

            migrationBuilder.DropTable(
                name: "Warranties");

            migrationBuilder.DropTable(
                name: "PartTypes");

            migrationBuilder.DropTable(
                name: "Recall");

            migrationBuilder.DropTable(
                name: "WarrantyRecord");

            migrationBuilder.DropTable(
                name: "WarrantyRequests");

            migrationBuilder.DropTable(
                name: "WarrantyPolicies");

            migrationBuilder.DropTable(
                name: "CarConditionCurrents");

            migrationBuilder.DropTable(
                name: "CarInfo");

            migrationBuilder.DropTable(
                name: "CarModels");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "WorkPlaces");
        }
    }
}
