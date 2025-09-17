using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OEMEVWarrantyManagement.Database.Migrations
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodInMonths = table.Column<int>(type: "int", nullable: false),
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PartTypeId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WorkPlacesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_WorkPlaces_WorkPlacesId",
                        column: x => x.WorkPlacesId,
                        principalTable: "WorkPlaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PartTypeModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarConditions_PartTypeModels_PartTypeModelId",
                        column: x => x.PartTypeModelId,
                        principalTable: "PartTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartTypeModelId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parts_PartTypeModels_PartTypeModelId",
                        column: x => x.PartTypeModelId,
                        principalTable: "PartTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkPlacePartTypeModels(PartInStock)",
                columns: table => new
                {
                    WorkPlacesId = table.Column<int>(type: "int", nullable: false),
                    PartTypeModelId = table.Column<int>(type: "int", nullable: false),
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SCStaffId = table.Column<int>(type: "int", nullable: false),
                    SCTechID = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Assignment_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarConditionCurrents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffSend = table.Column<int>(type: "int", nullable: false),
                    StaffReceive = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationId = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    DateSend = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReceive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartsID = table.Column<int>(type: "int", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "PartReplacements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartTypeModelId = table.Column<int>(type: "int", nullable: false),
                    VIN = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartReplacements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartReplacements_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartReplacements_PartTypeModels_PartTypeModelId",
                        column: x => x.PartTypeModelId,
                        principalTable: "PartTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartTypeId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SCStaffId = table.Column<int>(type: "int", nullable: false),
                    EVMStaffId = table.Column<int>(type: "int", nullable: false)
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
                name: "RoleEmployee",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleEmployee", x => new { x.RoleId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_RoleEmployee_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleEmployee_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Techs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Techs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Techs_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarConditionCarConditionCurrent",
                columns: table => new
                {
                    CarConditionId = table.Column<int>(type: "int", nullable: false),
                    CarConditionCurrentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarConditionCarConditionCurrent", x => new { x.CarConditionId, x.CarConditionCurrentId });
                    table.ForeignKey(
                        name: "FK_CarConditionCarConditionCurrent_CarConditionCurrents_CarConditionCurrentId",
                        column: x => x.CarConditionCurrentId,
                        principalTable: "CarConditionCurrents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarConditionCarConditionCurrent_CarConditions_CarConditionId",
                        column: x => x.CarConditionId,
                        principalTable: "CarConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CarConditionCurrentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
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
                    VIN = table.Column<int>(type: "int", maxLength: 17, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ModelId = table.Column<int>(type: "int", maxLength: 50, nullable: false)
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
                name: "PartsDeliveryParts",
                columns: table => new
                {
                    DeliveryPartId = table.Column<int>(type: "int", nullable: false),
                    PartsId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsDeliveryParts", x => new { x.DeliveryPartId, x.PartsId });
                    table.ForeignKey(
                        name: "FK_PartsDeliveryParts_DeliveryParts_DeliveryPartId",
                        column: x => x.DeliveryPartId,
                        principalTable: "DeliveryParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartsDeliveryParts_Parts_PartsId",
                        column: x => x.PartsId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkPlaceDeliveryParts",
                columns: table => new
                {
                    WorkPlaceId = table.Column<int>(type: "int", nullable: false),
                    DeliveryPartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPlaceDeliveryParts", x => new { x.WorkPlaceId, x.DeliveryPartId });
                    table.ForeignKey(
                        name: "FK_WorkPlaceDeliveryParts_DeliveryParts_DeliveryPartId",
                        column: x => x.DeliveryPartId,
                        principalTable: "DeliveryParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkPlaceDeliveryParts_WorkPlaces_WorkPlaceId",
                        column: x => x.WorkPlaceId,
                        principalTable: "WorkPlaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartsRequestParts",
                columns: table => new
                {
                    RequestPartId = table.Column<int>(type: "int", nullable: false),
                    PartsId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsRequestParts", x => new { x.RequestPartId, x.PartsId });
                    table.ForeignKey(
                        name: "FK_PartsRequestParts_Parts_PartsId",
                        column: x => x.PartsId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartsRequestParts_RequestParts_RequestPartId",
                        column: x => x.RequestPartId,
                        principalTable: "RequestParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TypeAppointmentId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    VIN = table.Column<int>(type: "int", nullable: false)
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
                name: "WarrantyRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    VIN = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    WarrantyPolicyId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VIN = table.Column<int>(type: "int", nullable: false),
                    SCStaffId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EVMStaffId = table.Column<int>(type: "int", nullable: false),
                    CarConditionCurrentId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarrantyRequests_CarConditionCurrents_CarConditionCurrentId",
                        column: x => x.CarConditionCurrentId,
                        principalTable: "CarConditionCurrents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Warranties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeTechId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestWarrantyId = table.Column<int>(type: "int", nullable: false),
                    PartRereplacementId = table.Column<int>(type: "int", nullable: false),
                    WarrantyRecordId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeSCStaffId = table.Column<int>(type: "int", nullable: false)
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
                        name: "FK_Warranties_Techs_EmployeeTechId",
                        column: x => x.EmployeeTechId,
                        principalTable: "Techs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warranties_WarrantyRequests_RequestWarrantyId",
                        column: x => x.RequestWarrantyId,
                        principalTable: "WarrantyRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Assignment_TaskId",
                table: "Assignment",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CarConditionCarConditionCurrent_CarConditionCurrentId",
                table: "CarConditionCarConditionCurrent",
                column: "CarConditionCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarConditionCurrents_TechnicianId",
                table: "CarConditionCurrents",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_CarConditions_PartTypeModelId",
                table: "CarConditions",
                column: "PartTypeModelId");

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
                name: "IX_Employee_RoleId",
                table: "Employee",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_WorkPlacesId",
                table: "Employee",
                column: "WorkPlacesId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_CarConditionCurrentId",
                table: "Images",
                column: "CarConditionCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_PartReplacements_EmployeeId",
                table: "PartReplacements",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartReplacements_PartTypeModelId",
                table: "PartReplacements",
                column: "PartTypeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartTypeModelId",
                table: "Parts",
                column: "PartTypeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PartsDeliveryParts_PartsId",
                table: "PartsDeliveryParts",
                column: "PartsId");

            migrationBuilder.CreateIndex(
                name: "IX_PartsRequestParts_PartsId",
                table: "PartsRequestParts",
                column: "PartsId");

            migrationBuilder.CreateIndex(
                name: "IX_PartTypeModels_PartTypeId",
                table: "PartTypeModels",
                column: "PartTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestParts_EVMStaffId",
                table: "RequestParts",
                column: "EVMStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestParts_SCStaffId",
                table: "RequestParts",
                column: "SCStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleEmployee_EmployeeId",
                table: "RoleEmployee",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Techs_EmployeeId",
                table: "Techs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_EmployeeSCStaffId",
                table: "Warranties",
                column: "EmployeeSCStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_EmployeeTechId",
                table: "Warranties",
                column: "EmployeeTechId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_RequestWarrantyId",
                table: "Warranties",
                column: "RequestWarrantyId");

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
                name: "IX_WorkPlaceDeliveryParts_DeliveryPartId",
                table: "WorkPlaceDeliveryParts",
                column: "DeliveryPartId");

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
                name: "CarConditionCarConditionCurrent");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "PartReplacements");

            migrationBuilder.DropTable(
                name: "PartsDeliveryParts");

            migrationBuilder.DropTable(
                name: "PartsRequestParts");

            migrationBuilder.DropTable(
                name: "RoleEmployee");

            migrationBuilder.DropTable(
                name: "Warranties");

            migrationBuilder.DropTable(
                name: "WarrantyRecord");

            migrationBuilder.DropTable(
                name: "WorkPlaceDeliveryParts");

            migrationBuilder.DropTable(
                name: "WorkPlacePartTypeModels(PartInStock)");

            migrationBuilder.DropTable(
                name: "TypeAppointments");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "CarConditions");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "RequestParts");

            migrationBuilder.DropTable(
                name: "Techs");

            migrationBuilder.DropTable(
                name: "WarrantyRequests");

            migrationBuilder.DropTable(
                name: "WarrantyPolicies");

            migrationBuilder.DropTable(
                name: "DeliveryParts");

            migrationBuilder.DropTable(
                name: "PartTypeModels");

            migrationBuilder.DropTable(
                name: "CarConditionCurrents");

            migrationBuilder.DropTable(
                name: "CarInfo");

            migrationBuilder.DropTable(
                name: "PartTypes");

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
