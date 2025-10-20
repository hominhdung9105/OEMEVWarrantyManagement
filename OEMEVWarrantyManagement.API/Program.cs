using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.API.Policy.Role;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.Mapping;
using OEMEVWarrantyManagement.Application.Services;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Infrastructure.Repositories;
using OEMEVWarrantyManagement.Share.Configs;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Middlewares;
using OEMEVWarrantyManagement.Share.Models.Response;
using Scalar.AspNetCore;
using OEMEVWarrantyManagement.Infrastructure.Persistence; // <-- Thêm using này
using Microsoft.EntityFrameworkCore; // <-- Thêm using này

namespace OEMEVWarrantyManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Bind AppSettings from appsettings.json
            builder.Services.Configure<AppSettings>(
                builder.Configuration.GetSection("AppSettings"));

            // Add Controllers - ignore null value in response

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    //options.InvalidModelStateResponseFactory = context =>
                    //{
                    //    var errors = context.ModelState
                    //        .Where(x => x.Value?.Errors.Count > 0)
                    //        .Select(x => new
                    //        {
                    //            field = x.Key,
                    //            messages = x.Value!.Errors.Select(e => e.ErrorMessage)
                    //        });

                    //    return new BadRequestObjectResult(ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat));
                    //};
                })
                .AddJsonOptions(options =>
                {
                    options.AllowInputFormatterExceptionMessages = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            //Auto Mapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Authentication - JWT Bearer
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult(); // ngan asp .net xu ly mac dinh
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = ApiResponse<object>.Fail(ResponseError.AuthenticationFailed);

                        return context.Response.WriteAsJsonAsync(response);
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse(); // chan challenge mac dinh
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = ApiResponse<object>.Fail(ResponseError.AuthenticationFailed);

                        return context.Response.WriteAsJsonAsync(response);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse<object>.Fail(ResponseError.Forbidden);
                        return context.Response.WriteAsJsonAsync(response);
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!))
                };
            });

            // Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy =>
                    policy.RequireRole(RoleIdEnum.Admin.GetRoleId()));

                options.AddPolicy("RequireScStaff", policy =>
                    policy.RequireRole(RoleIdEnum.ScStaff.GetRoleId()));

                options.AddPolicy("RequireScTech", policy =>
                    policy.RequireRole(RoleIdEnum.Technician.GetRoleId()));

                options.AddPolicy("RequireEvmStaff", policy =>
                    policy.RequireRole(RoleIdEnum.EvmStaff.GetRoleId()));

                options.AddPolicy("RequireScTechOrScStaff", policy =>
                    policy.RequireRole(RoleIdEnum.Technician.GetRoleId(), RoleIdEnum.ScStaff.GetRoleId()));

                options.AddPolicy("RequireScStaffOrEvmStaff", policy =>
                    policy.RequireRole(RoleIdEnum.EvmStaff.GetRoleId(), RoleIdEnum.ScStaff.GetRoleId()));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddHttpContextAccessor();//Use for CurrentUserService 

            //Auth
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            //Warranty Claim
            builder.Services.AddScoped<IWarrantyClaimService, WarrantyClaimService>();
            builder.Services.AddScoped<IWarrantyClaimRepository, WarrantyClaimRepository>();
            //Vehicle
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IVehicleService, VehicleService>();
            //Employee
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            //WorkOrder
            builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
            builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
            //Warranty Policy
            builder.Services.AddScoped<IWarrantyPolicyRepository, WarrantyPolicyRepository>();
            builder.Services.AddScoped<IWarrantyPolicyService, WarrantyPolicyService>();
            //Part
            builder.Services.AddScoped<IPartService, PartService>();
            builder.Services.AddScoped<IPartRepository, PartRepository>();
            //Take User
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            //Claim Part
            builder.Services.AddScoped<IClaimPartService, ClaimPartService>();
            builder.Services.AddScoped<IClaimPartRepository, ClaimPartRepository>();
            //Part ORder
            builder.Services.AddScoped<IPartOrderRepository, PartOrderRepository>();
            builder.Services.AddScoped<IPartOrderService, PartOrderService>();
            //Part Order Item
            builder.Services.AddScoped<IPartOrderItemRepository, PartOrderItemRepository>();
            builder.Services.AddScoped<IPartOrderItemService, PartOrderItemService>();
            //Vehicle Warranty Policy
            builder.Services.AddScoped<IVehicleWarrantyPolicyRepository, VehicleWarrantyPolicyRepository>();
            builder.Services.AddScoped<IVehicleWarrantyPolicyService, VehicleWarrantyPolicyService>();
            //Customer
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            //builder.Services.AddScoped<ICustomerService, CustomerService>();
            //Back Warranty Claim
            builder.Services.AddScoped<IBackWarrantyClaimRepository, BackWarrantyClaimRepository>();
            builder.Services.AddScoped<IBackWarrantyClaimService, BackWarrantyClaimService>();
            //Vehicle Part
            builder.Services.AddScoped<IVehiclePartRepository, VehiclePartRepository>();
            builder.Services.AddScoped<IVehiclePartService, VehiclePartService>();
            // Attechment
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.AddScoped<IImageService, ImageService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });


            var app = builder.Build();

            //// =================================================================
            //// KHỐI MÃ SEEDING: Thêm khối này vào | chạy 1 lần nếu muốn lấy data mẫu
            //// =================================================================
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        var dbContext = services.GetRequiredService<AppDbContext>();

            //        // 1. Tự động chạy migration để tạo bảng
            //        dbContext.Database.Migrate();

            //        // 2. Gọi Seeder để thêm data (nó sẽ tự kiểm tra nếu DB trống)
            //        DataSeeder.SeedDatabase(dbContext);
            //    }
            //    catch (Exception ex)
            //    {
            //        // Ghi log lỗi nếu có
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "Đã xảy ra lỗi khi seeding database.");
            //    }
            //}
            //// =================================================================
            //// KẾT THÚC KHỐI MÃ SEEDING
            //// =================================================================

            // Dev


            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();

            }
            app.UseCors("AllowAll");
            // Thay tất cả exception middleware bằng global response

            //app.UseMiddleware<GlobalResponseMiddleware>();

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
