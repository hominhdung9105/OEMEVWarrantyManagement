
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OEMEVWarrantyManagement.API.Models.Response;
using OEMEVWarrantyManagement.API.Policy.Role;
using OEMEVWarrantyManagement.Database.Data;
using OEMEVWarrantyManagement.Services;
using Scalar.AspNetCore;

namespace OEMEVWarrantyManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ApiExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
