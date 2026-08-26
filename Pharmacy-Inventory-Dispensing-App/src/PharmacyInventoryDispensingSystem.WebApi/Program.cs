using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Infrastructure;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using PharmacyInventoryDispensingSystem.WebApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



 builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MechanicShop API V1");

        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });

    app.MapScalarApiReference();

}

else
{
    app.UseHsts();
}


//Custom for the order of Midllewares:
app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();


app.MapStaticAssets();

// Initialize database + seed data
await app.Services.InitializeInfrastructureAsync();


app.Run();