using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Seed;
using PharmacyInventoryDispensingSystem.Infrastructure.Services.Jwt;

namespace PharmacyInventoryDispensingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        ConfigureJwt(services, configuration);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin));
            options.AddPolicy("PharmacistOrAdmin", policy => policy.RequireRole(AppRoles.Pharmacist, AppRoles.Admin));
            options.AddPolicy("DoctorOrAdmin", policy => policy.RequireRole(AppRoles.Doctor, AppRoles.Admin));
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<ITokenProvider, JwtTokenProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        //Register Identity:
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            //Password Rules:
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;

            //Lockout settings :

            options.Lockout.MaxFailedAccessAttempts = 5; //N=5
            options.Lockout.AllowedForNewUsers = true;
            //يستمر حظر 15 دقيقة:
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            //User requirements :

            //This means each email must be unique in the system:
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;




        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();


    private static void ConfigureJwt(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(options =>
        {
            options.Key = configuration["Jwt:Key"] ?? string.Empty;
            options.Issuer = configuration["Jwt:Issuer"] ?? string.Empty;
            options.Audience = configuration["Jwt:Audience"] ?? string.Empty;
            if (int.TryParse(configuration["Jwt:ExpiryMinutes"], out var expiryMinutes))
            {
                options.ExpiryMinutes = expiryMinutes;
            }
        });

        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is not configured.");
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        return services;
    }
}
