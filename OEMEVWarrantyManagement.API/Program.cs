
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.API.Policy.Role;
using OEMEVWarrantyManagement.Application.Dtos.Config;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.Mapping;
using OEMEVWarrantyManagement.Application.Services;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Infrastructure.Repositories;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using Scalar.AspNetCore;
using System;
using System.Text;

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

            // Add services to the container.
            //builder.Services.AddControllers();

            //Ignore Null
            builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);

            //Auto Mapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult(); // ngan asp .net xu ly mac dinh
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = ApiResponse<object>.ErrorResponse(ResponseError.AuthenticationFailed);

                        return context.Response.WriteAsJsonAsync(response);
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse(); // chan challenge mac dinh
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = ApiResponse<object>.ErrorResponse(ResponseError.AuthenticationFailed);

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

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy =>
                    policy.Requirements.Add(new RoleRequirement("ROL-ADMIN")));

                options.AddPolicy("RequireScStaff", policy =>
                    policy.Requirements.Add(new RoleRequirement("ROL-STAFF")));

                options.AddPolicy("RequireScTech", policy =>
                    policy.Requirements.Add(new RoleRequirement("ROL-TECH")));

                options.AddPolicy("RequireEvmStaff", policy =>
                    policy.Requirements.Add(new RoleRequirement("ROL-EVM")));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IWarrantyRecordService, WarrantyRecordService>();
            builder.Services.AddScoped<IWarrantyRecordRepository, WarrantyRecordRepository>();
            builder.Services.AddScoped<IWarrantyRequestRepository, WarrantyRequestRepository>();
            builder.Services.AddScoped<IWarrantyRequestService, WarrantyRequestService>();
            builder.Services.AddScoped<ICarconditionService, CarConditionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseMiddleware<ApiExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseMiddleware<ApiExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
