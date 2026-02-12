using AbySalto.Mid.Application;
using AbySalto.Mid.Infrastructure;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddPresentation()
                .AddApplication()
                .AddInfrastructure(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        )
                    };
                });
            
            builder.Services
                .AddAuthentication()
                .AddJwtBearer(options =>
                {
                    var key = builder.Configuration["Jwt:Key"]!;
                    var issuer = builder.Configuration["Jwt:Issuer"];
                    var audience = builder.Configuration["Jwt:Audience"];

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
                        ValidIssuer = issuer,
                        ValidateAudience = !string.IsNullOrWhiteSpace(audience),
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(2)
                    };
                });

            builder.Services.AddAuthorization();
            
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Desk Link");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
