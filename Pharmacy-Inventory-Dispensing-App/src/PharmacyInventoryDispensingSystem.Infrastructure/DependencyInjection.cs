using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories;

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


        return services;
    }
}

