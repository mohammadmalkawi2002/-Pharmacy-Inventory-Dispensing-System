using PharmacyInventoryDispensingSystem.Application;
using PharmacyInventoryDispensingSystem.Infrastructure;
using PharmacyInventoryDispensingSystem.WebApi;
using Scalar.AspNetCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//QuestPDF License:
QuestPDF.Settings.License = LicenseType.Community;

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "PharmacyInventoryDispensingSystem API V1");

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

app.UseRouting();

//Custom for the order of Midllewares:
app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();


app.MapStaticAssets();

// Initialize database + seed data
await app.Services.InitializeInfrastructureAsync();


app.Run();