namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Pharmacist = "Pharmacist";
    public const string Doctor = "Doctor";

    public static readonly string[] All = [Admin, Pharmacist, Doctor];
}
