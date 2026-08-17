using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Batches;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Entities.StockMovements;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentUser? _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Medicine> Medicines => Set<Medicine>();

    public DbSet<MedicineBatch> MedicineBatches => Set<MedicineBatch>();

    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

    public DbSet<Dispense> Dispenses => Set<Dispense>();

    public DbSet<DispenseItem> DispenseItems => Set<DispenseItem>();

    public override int SaveChanges()
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDelete();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private void ApplyAuditAndSoftDelete()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var userId = _currentUser?.UserId;

        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.EnsureId();
            }
        }

        foreach (var entry in ChangeTracker.Entries<SoftDeletableEntity>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.Delete(userId, utcNow);
        }

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.CreatedBy ??= userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}
