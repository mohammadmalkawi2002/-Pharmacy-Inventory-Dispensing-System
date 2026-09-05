using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.BackgroundJobs
{
    public sealed class PrescriptionExpirationBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<PrescriptionExpirationBackgroundService> logger,
        TimeProvider timeProvider)  : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

            while(await timer.WaitForNextTickAsync(stoppingToken))
            {
                logger.LogInformation(
                "Checking for expired prescriptions at {Now}.",
                timeProvider.GetUtcNow());

                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var context = scope.ServiceProvider
                        .GetRequiredService<AppDbContext>();

                    var today = DateOnly.FromDateTime(
                        timeProvider.GetUtcNow().UtcDateTime);

                    int expiredCount = await context.Prescriptions
                   .Where(prescription =>
                       prescription.Status == PrescriptionStatus.Active &&
                       prescription.ValidTo < today)
                   .ExecuteUpdateAsync(
                       setters => setters.SetProperty(
                           prescription => prescription.Status,
                           PrescriptionStatus.Expired),
                       stoppingToken);


                    if (expiredCount > 0)
                    {
                        logger.LogInformation(
                            "Marked {Count} prescriptions as expired.",
                            expiredCount);
                    }


                    else
                    {
                        logger.LogInformation(
                      "No expired prescriptions found.");
                    }

                }
                catch (OperationCanceledException)
               when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Error while updating expired prescriptions.");
                }



            }
        }
    }
}
