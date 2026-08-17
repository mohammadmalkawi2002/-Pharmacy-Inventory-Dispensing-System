using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString=configuration.GetConnectionString("DefaultConnection");

        // Register the AppDbContext with the connection string: 
        services.AddDbContext<AppDbContext>(options => 
        {
            options.UseSqlServer(connectionString);
            
        });

        //Register UnitOfWork ==>:
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




        return services;
    }
}

