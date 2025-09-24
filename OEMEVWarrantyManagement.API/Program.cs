using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.API.Policy.Role;
using OEMEVWarrantyManagement.Application.Dtos.Config;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.Mapping;
using OEMEVWarrantyManagement.Application.Services;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Infrastructure.Repositories;
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
            builder.Services.AddControllers();

            //Auto Mapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
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


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
