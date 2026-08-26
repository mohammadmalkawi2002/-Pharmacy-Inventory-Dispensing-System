using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PharmacyInventoryDispensingSystem.Application.Common.Behaviours;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
              
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
               
            });

            return services;
        }
    }
}
