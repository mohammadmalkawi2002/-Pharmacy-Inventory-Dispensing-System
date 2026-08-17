namespace PharmacyInventoryDispensingSystem.Application.Common.Authorization;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Pharmacist = "Pharmacist";
    public const string Doctor = "Doctor";

    public static readonly string[] All = [Admin, Pharmacist, Doctor];
}

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string PharmacistOrAdmin = "PharmacistOrAdmin";
    public const string DoctorOrAdmin = "DoctorOrAdmin";
}
