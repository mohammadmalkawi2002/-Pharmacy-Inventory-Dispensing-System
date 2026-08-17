using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PharmacyInventoryDispensingSystem.Domain.Entities.Batches;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Seed;

internal static class DatabaseSeeder
{
    internal const string AdminEmail = "admin@pharmacy.local";
    internal const string AdminPassword = "Admin#12345!";
    internal const string PharmacistEmail = "pharmacist@pharmacy.local";
    internal const string DoctorEmail = "doctor@pharmacy.local";
    private const string DefaultPassword = "User#12345!";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRolesAsync(roleManager);
        var users = await SeedUsersAsync(userManager);
        await SeedCatalogAsync(context, users, cancellationToken);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task<(ApplicationUser Admin, ApplicationUser Pharmacist, ApplicationUser Doctor)> SeedUsersAsync(
        UserManager<ApplicationUser> userManager)
    {
        var admin = await EnsureUserAsync(
            userManager,
            email: AdminEmail,
            fullName: "System Administrator",
            password: AdminPassword,
            role: AppRoles.Admin);

        var pharmacist = await EnsureUserAsync(
            userManager,
            email: PharmacistEmail,
            fullName: "Default Pharmacist",
            password: DefaultPassword,
            role: AppRoles.Pharmacist);

        var doctor = await EnsureUserAsync(
            userManager,
            email: DoctorEmail,
            fullName: "Default Doctor",
            password: DefaultPassword,
            role: AppRoles.Doctor);

        return (admin, pharmacist, doctor);
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed user '{email}': {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }

    private static async Task SeedCatalogAsync(
        AppDbContext context,
        (ApplicationUser Admin, ApplicationUser Pharmacist, ApplicationUser Doctor) users,
        CancellationToken cancellationToken)
    {
        if (await context.Medicines.IgnoreQueryFilters().AnyAsync(cancellationToken))
        {
            return;
        }

        var medicines = CreateMedicines();
        context.Medicines.AddRange(medicines);

        var batches = CreateBatches(medicines);
        context.MedicineBatches.AddRange(batches);

        var (prescriptions, items) = CreatePrescriptions(users.Doctor.Id, medicines);
        foreach (var item in items)
        {
            item.EnsureId();
        }

        context.Prescriptions.AddRange(prescriptions);
        context.PrescriptionItems.AddRange(items);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static List<Medicine> CreateMedicines()
    {
        var definitions = new (string Code, string Name, string Strength, MedicineForm Form, string Unit, int ReorderLevel)[]
        {
            ("PAR-500", "Paracetamol", "500mg", MedicineForm.Tablet, "Box", 50),
            ("AMOX-250", "Amoxicillin", "250mg", MedicineForm.Capsule, "Box", 40),
            ("IBU-400", "Ibuprofen", "400mg", MedicineForm.Tablet, "Box", 45),
            ("CET-10", "Cetirizine", "10mg", MedicineForm.Tablet, "Strip", 30),
            ("OMEP-20", "Omeprazole", "20mg", MedicineForm.Capsule, "Box", 25),
            ("MET-500", "Metformin", "500mg", MedicineForm.Tablet, "Box", 35),
            ("VITD-1K", "Vitamin D3", "1000IU", MedicineForm.Capsule, "Bottle", 20),
            ("HYD-1", "Hydrocortisone", "1%", MedicineForm.Cream, "Tube", 15),
            ("INS-100", "Insulin Regular", "100IU/ml", MedicineForm.Injection, "Vial", 10),
            ("REF-EYE", "Refresh Tears", "0.5%", MedicineForm.Drops, "Bottle", 18)
        };

        return definitions
            .Select(item => Medicine.Create(
                Guid.CreateVersion7(),
                item.Code,
                item.Name,
                item.Strength,
                item.Form,
                item.Unit,
                item.ReorderLevel).Value)
            .ToList();
    }

    private static List<MedicineBatch> CreateBatches(IReadOnlyList<Medicine> medicines)
    {
        var today = DateTime.UtcNow.Date;
        var receivedAt = DateTimeOffset.UtcNow;

        var definitions = new (int MedicineIndex, string BatchNumber, DateTime Expiry, int Quantity)[]
        {
            (0, "PAR-B001", today.AddMonths(18), 200),
            (1, "AMOX-B001", today.AddMonths(12), 150),
            (2, "IBU-B001", today.AddMonths(15), 180),
            (3, "CET-B001", today.AddMonths(20), 120),
            (4, "OMEP-B001", today.AddMonths(14), 90),
            (5, "MET-B001", today.AddMonths(16), 160)
        };

        return definitions
            .Select(item => MedicineBatch.Create(
                Guid.CreateVersion7(),
                medicines[item.MedicineIndex].Id,
                item.BatchNumber,
                item.Expiry,
                item.Quantity,
                receivedAt).Value)
            .ToList();
    }

    private static (List<Prescription> Prescriptions, List<PrescriptionItem> Items) CreatePrescriptions(
        string doctorId,
        IReadOnlyList<Medicine> medicines)
    {
        var validFrom = DateTime.UtcNow.Date;
        var validTo = validFrom.AddDays(30);

        var first = Prescription.Create(
            Guid.CreateVersion7(),
            "RX-1001",
            "Ahmed Hassan",
            "01000000001",
            doctorId,
            validFrom,
            validTo,
            maxRefills: 2,
            notes: "Take after meals.").Value;

        var second = Prescription.Create(
            Guid.CreateVersion7(),
            "RX-1002",
            "Sara Ali",
            "01000000002",
            doctorId,
            validFrom,
            validTo,
            maxRefills: 1,
            notes: "Monitor blood glucose.").Value;

        var items = new List<PrescriptionItem>
        {
            new()
            {
                PrescriptionId = first.Id,
                MedicineId = medicines[0].Id,
                QuantityPrescribed = 20,
                QuantityDispensed = 0,
                DosageInstructions = "1 tablet every 8 hours"
            },
            new()
            {
                PrescriptionId = first.Id,
                MedicineId = medicines[2].Id,
                QuantityPrescribed = 15,
                QuantityDispensed = 0,
                DosageInstructions = "1 tablet every 12 hours after food"
            },
            new()
            {
                PrescriptionId = second.Id,
                MedicineId = medicines[5].Id,
                QuantityPrescribed = 30,
                QuantityDispensed = 0,
                DosageInstructions = "1 tablet twice daily"
            }
        };

        return ([first, second], items);
    }
}
