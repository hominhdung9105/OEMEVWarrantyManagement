using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.API.Policy.Role;
using OEMEVWarrantyManagement.Application.BackgroundJobs;
using OEMEVWarrantyManagement.Application.BackgroundServices;
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
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;

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
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<EmailUrlSettings>(
                builder.Configuration.GetSection("EmailUrlSettings"));

            // Add Controllers - ignore null value in response

            builder.Services.AddControllers()
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

            // Add Hangfire services
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            builder.Services.AddHangfireServer();

            // Authentication: Use JWT as default (avoid redirect to Google on API unauthorized)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse<object>.Fail(ResponseError.AuthenticationFailed);
                        return context.Response.WriteAsJsonAsync(response);
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
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
            })
            // Optional Google external login (not default challenge to avoid redirect on API calls)
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.CallbackPath = "/signin-google";
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

                options.AddPolicy("RequireAddmin", policy =>
                    policy.RequireRole(RoleIdEnum.Admin.GetRoleId()));

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
            // Organization
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            // Appointment
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            // Hangfire Jobs
            builder.Services.AddScoped<AppointmentCancellationJob>();
            // Email
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Dashboard
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
            // Campaign
            builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
            builder.Services.AddScoped<ICampaignService, CampaignService>();
            // Campaign Vehicle
            builder.Services.AddScoped<ICampaignVehicleRepository, CampaignVehicleRepository>();
            builder.Services.AddScoped<ICampaignVehicleService, CampaignVehicleService>();
            // Campaign Notification
            builder.Services.AddScoped<ICampaignNotificationRepository, CampaignNotificationRepository>();
            builder.Services.AddScoped<ICampaignNotificationService, CampaignNotificationService>();

            // Background Services
            builder.Services.AddHostedService<CampaignReminderBackgroundService>();
            builder.Services.AddHostedService<CampaignAutoCloseBackgroundService>();

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
            //// KH?I M? SEEDING: Thêm kh?i này vào | ch?y 1 l?n n?u mu?n l?y data m?u
            //// =================================================================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AppDbContext>();

                    // 1. T? đ?ng ch?y migration đ? t?o b?ng
                    dbContext.Database.Migrate();

                    // 2. G?i Seeder đ? thêm data (nó s? t? ki?m tra n?u DB tr?ng)
                    DataSeeder.SeedDatabase(dbContext);
                }
                catch (Exception ex)
                {
                    // Ghi log l?i n?u có
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Đ? x?y ra l?i khi seeding database.");
                }
            }
            //// =================================================================
            //// K?T THÚC KH?I M? SEEDING
            //// =================================================================

            // Dev

            app.UseCors("AllowAll");

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            // Configure Hangfire Dashboard (optional - for monitoring)
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            app.UseMiddleware<GlobalResponseMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }

    // Simple authorization filter for Hangfire Dashboard (allow all in dev, customize for prod)
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In production, add proper authentication
            return true;
        }
    }
}
