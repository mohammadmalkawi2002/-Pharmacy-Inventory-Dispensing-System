using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Identity;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
   

    public DbSet<Medicine> Medicines => Set<Medicine>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

    public DbSet<Dispense> Dispenses => Set<Dispense>();

    public DbSet<DispenseItem> DispenseItems => Set<DispenseItem>();
    public DbSet<RefreshToken> RefreshTokens=> Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasSequence<int>("PrescriptionNumberSequence")
            .StartsAt(1)
            .IncrementsBy(1)
            .HasMax(999999)
            .IsCyclic(false);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedicineConfiguration).Assembly);

    }
}

