using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public   static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register  infrastructure services here
            // Example: services.AddScoped<IMyService, MyService>();
            
            return services;
        }
    }
}
