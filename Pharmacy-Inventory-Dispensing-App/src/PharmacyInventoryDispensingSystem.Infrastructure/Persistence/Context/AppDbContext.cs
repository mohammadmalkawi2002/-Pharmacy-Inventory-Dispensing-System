using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Domain.Entities;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
   

    public DbSet<Medicine> Medicines => Set<Medicine>();

    public DbSet<MedicineBatch> MedicineBatches => Set<MedicineBatch>();

    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

    public DbSet<Dispense> Dispenses => Set<Dispense>();

    public DbSet<DispenseItem> DispenseItems => Set<DispenseItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedicineConfiguration).Assembly);

    }
}

