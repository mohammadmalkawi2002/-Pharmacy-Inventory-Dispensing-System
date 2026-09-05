using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Seed
{
    internal static class DatabaseSeeder
    {

        internal const string AdminEmail = "mohammadmalkawi681@gmail.com";
        internal const string AdminPassword = "Admin@12345!";
        internal const string PharmacistEmail = "pharmacist@pharmacy.local";
        internal const string DoctorEmail = "doctor@pharmacy.local";
        internal const string ReceptionistEmail = "receptionist@pharmacy.local";
        private const string DefaultPassword = "User#12345!";

        public static async Task SeedIdentityAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            // 1. Resolve Identity services
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 2. Seed roles:
            await SeedRolesAsync(roleManager);
            // 3. Seed role permissions
            await SeedRolePermissionsAsync(roleManager, cancellationToken);

            // 4. Seed development users

            var (admin, pharmacist, doctor, receptionist) =
                await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in RoleNames.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {

                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ",
                         result.Errors.Select(error => error.Description));
                        throw new InvalidOperationException(
                                          $"Failed to create role '{role}': {errors}");
                    }
                }
            }
        }

        private static async Task SeedRolePermissionsAsync(
        RoleManager<IdentityRole> roleManager,
        CancellationToken cancellationToken)
        {
            var rolePermissions = new Dictionary<string, string[]>
            {
                [RoleNames.Admin] = Permissions.All,

                [RoleNames.Receptionist] =
                [
                    Permissions.Patients.Read,
            Permissions.Patients.Create,
            Permissions.Patients.Update
                ],

                [RoleNames.Doctor] =
                [
                    Permissions.Patients.Read,
            Permissions.Medicines.Read,

            Permissions.Prescriptions.Read,
            Permissions.Prescriptions.Create,
            Permissions.Prescriptions.Update,
            Permissions.Prescriptions.Cancel
                ],

                [RoleNames.Pharmacist] =
                [
                    Permissions.Medicines.Read,
            Permissions.Medicines.Create,
            Permissions.Medicines.Update,
            Permissions.Medicines.Activate,
            Permissions.Medicines.Deactivate,
            Permissions.Medicines.ReadLowStock,

            Permissions.Prescriptions.Lookup,

            Permissions.Dispenses.Read,
            Permissions.Dispenses.Create
                ]
            };

            foreach (var rolePermission in rolePermissions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var role = await roleManager.FindByNameAsync(rolePermission.Key);

                if (role is null)
                    continue;

                var existingPermissionClaims = (await roleManager.GetClaimsAsync(role))
                    .Where(claim =>
                        claim.Type == ApplicationClaimTypes.Permission)
                    .ToList();

                var requiredPermissions = rolePermission.Value.ToHashSet();

                // Remove permissions that are no longer assigned to this role.
                foreach (var existingClaim in existingPermissionClaims)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (requiredPermissions.Contains(existingClaim.Value))
                        continue;

                    var result = await roleManager.RemoveClaimAsync(
                        role,
                        existingClaim);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            result.Errors.Select(error => error.Description));

                        throw new InvalidOperationException(
                            $"Failed to remove permission '{existingClaim.Value}' " +
                            $"from role '{role.Name}': {errors}");
                    }
                }

                var existingPermissions = existingPermissionClaims
                    .Select(claim => claim.Value)
                    .ToHashSet();

                // Add permissions that are missing from this role.
                foreach (var permission in requiredPermissions)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (existingPermissions.Contains(permission))
                        continue;

                    var result = await roleManager.AddClaimAsync(
                        role,
                        new Claim(
                            ApplicationClaimTypes.Permission,
                            permission));

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            result.Errors.Select(error => error.Description));

                        throw new InvalidOperationException(
                            $"Failed to seed permission '{permission}' " +
                            $"for role '{role.Name}': {errors}");
                    }
                }
            }
        }





        private static async Task<(ApplicationUser Admin, ApplicationUser Pharmacist, ApplicationUser Doctor, ApplicationUser Receptionist)> SeedUsersAsync(
            UserManager<ApplicationUser> userManager)
        {
            var admin = await EnsureUserAsync(userManager,
                            email: AdminEmail,
                          firstName: "System",
                          lastName: "Administrator",
                          password: AdminPassword,
                          role: RoleNames.Admin);

            var pharmacist = await EnsureUserAsync(
                        userManager,
                        email: PharmacistEmail,
                        firstName: "Default Pharmacist",
                        lastName: ".",
                        password: DefaultPassword,
                        role: RoleNames.Pharmacist);



            var doctor = await EnsureUserAsync(
                         userManager,
                         email: DoctorEmail,
                         firstName: "System",
                          lastName: "Doctor",
                         password: DefaultPassword,
                         role: RoleNames.Doctor);


            var receptionist = await EnsureUserAsync(
                         userManager,
                         email: ReceptionistEmail,
                         firstName: "System",
                         lastName: "Receptionist",
                         password: DefaultPassword,
                         role: RoleNames.Receptionist);

            return (admin, pharmacist, doctor, receptionist);

        }

        private static async Task<ApplicationUser> EnsureUserAsync(
                                                          UserManager<ApplicationUser> userManager,
                                                          string email,
                                                          string firstName,
                                                          string lastName,
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
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        createResult.Errors.Select(error => error.Description));

                    throw new InvalidOperationException(
                        $"Failed to create seed user '{email}': {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(error => error.Description));

                    throw new InvalidOperationException(
                        $"Failed to assign role '{role}' to user '{email}': {errors}");
                }
            }

            return user;
        }


        public static async Task SeedCatalogAsync(
            IServiceProvider services,
            CancellationToken cancellationToken = default)
        {
            var context = services.GetRequiredService<AppDbContext>();

            await SeedPatientsAsync(context, cancellationToken);
            await SeedMedicinesAsync(context, cancellationToken);

        }


        private static async Task SeedPatientsAsync(AppDbContext context, CancellationToken cancellationToken)
        {
            // Include archived patients when checking whether patient seed data exists.
            if (await context.Patients
                .IgnoreQueryFilters()
                .AnyAsync(cancellationToken))
            {
                return;
            }

            var patients = new List<Patient>
    {
        // =========================================================
        // Active Citizens - 25
        // =========================================================

        new()
        {
            DocumentId = "1000000001",
            FullName = "Mohammad Alharbi",
            DateOfBirth = new DateTime(1998, 3, 15),
            PhoneNumber = "+966501000001"
        },
        new()
        {
            DocumentId = "1000000002",
            FullName = "Mohammad Alqahtani",
            DateOfBirth = new DateTime(1987, 7, 21),
            PhoneNumber = "+966501000002"
        },
        new()
        {
            DocumentId = "1000000003",
            FullName = "Ahmed Alotaibi",
            DateOfBirth = new DateTime(1992, 11, 4),
            PhoneNumber = "+966501000003"
        },
        new()
        {
            DocumentId = "1000000004",
            FullName = "Ahmed Alzahrani",
            DateOfBirth = new DateTime(2000, 5, 19),
            PhoneNumber = "+966501000004"
        },
        new()
        {
            DocumentId = "1000000005",
            FullName = "Khalid Alghamdi",
            DateOfBirth = new DateTime(1985, 9, 12),
            PhoneNumber = "+966501000005"
        },
        new()
        {
            DocumentId = "1000000006",
            FullName = "Abdullah Alshammari",
            DateOfBirth = new DateTime(1995, 1, 27),
            PhoneNumber = "+966501000006"
        },
        new()
        {
            DocumentId = "1000000007",
            FullName = "Faisal Aldosari",
            DateOfBirth = new DateTime(1979, 6, 8),
            PhoneNumber = "+966501000007"
        },
        new()
        {
            DocumentId = "1000000008",
            FullName = "Saud Almutairi",
            DateOfBirth = new DateTime(2001, 10, 3),
            PhoneNumber = "+966501000008"
        },
        new()
        {
            DocumentId = "1000000009",
            FullName = "Omar Alsubaie",
            DateOfBirth = new DateTime(1990, 4, 17),
            PhoneNumber = "+966501000009"
        },
        new()
        {
            DocumentId = "1000000010",
            FullName = "Yousef Alenezi",
            DateOfBirth = new DateTime(1997, 8, 30),
            PhoneNumber = "+966501000010"
        },
        new()
        {
            DocumentId = "1100000011",
            FullName = "Sara Alharbi",
            DateOfBirth = new DateTime(1999, 12, 11),
            PhoneNumber = "+966551000011"
        },
        new()
        {
            DocumentId = "1100000012",
            FullName = "Sara Alqahtani",
            DateOfBirth = new DateTime(1993, 2, 25),
            PhoneNumber = "+966551000012"
        },
        new()
        {
            DocumentId = "1100000013",
            FullName = "Noura Alotaibi",
            DateOfBirth = new DateTime(1988, 7, 14),
            PhoneNumber = "+966551000013"
        },
        new()
        {
            DocumentId = "1100000014",
            FullName = "Reem Alzahrani",
            DateOfBirth = new DateTime(2002, 3, 6),
            PhoneNumber = "+966551000014"
        },
        new()
        {
            DocumentId = "1100000015",
            FullName = "Huda Alghamdi",
            DateOfBirth = new DateTime(1982, 10, 20),
            PhoneNumber = "+966551000015"
        },
        new()
        {
            DocumentId = "1100000016",
            FullName = "Maha Alshammari",
            DateOfBirth = new DateTime(1996, 5, 9),
            PhoneNumber = "+966551000016"
        },
        new()
        {
            DocumentId = "1100000017",
            FullName = "Lama Aldosari",
            DateOfBirth = new DateTime(2003, 1, 18),
            PhoneNumber = "+966551000017"
        },
        new()
        {
            DocumentId = "1100000018",
            FullName = "Abeer Almutairi",
            DateOfBirth = new DateTime(1975, 11, 29),
            PhoneNumber = "+966551000018"
        },
        new()
        {
            DocumentId = "1100000019",
            FullName = "Mariam Alsubaie",
            DateOfBirth = new DateTime(1991, 6, 23),
            PhoneNumber = "+966551000019"
        },
        new()
        {
            DocumentId = "1100000020",
            FullName = "Dalal Alenezi",
            DateOfBirth = new DateTime(1984, 4, 2),
            PhoneNumber = "+966551000020"
        },
        new()
        {
            DocumentId = "1200000021",
            FullName = "Mohammad Almutairi",
            DateOfBirth = new DateTime(2000, 9, 13),
            PhoneNumber = "+966541000021"
        },
        new()
        {
            DocumentId = "1200000022",
            FullName = "Mohammad Aldosari",
            DateOfBirth = new DateTime(1989, 12, 7),
            PhoneNumber = "+966541000022"
        },
        new()
        {
            DocumentId = "1200000023",
            FullName = "Ahmed Alenezi",
            DateOfBirth = new DateTime(1994, 8, 16),
            PhoneNumber = "+966541000023"
        },
        new()
        {
            DocumentId = "1200000024",
            FullName = "Ali Alharbi",
            DateOfBirth = new DateTime(1978, 2, 28),
            PhoneNumber = "+966541000024"
        },
        new()
        {
            DocumentId = "1200000025",
            FullName = "Hassan Alqahtani",
            DateOfBirth = new DateTime(2004, 6, 5),
            PhoneNumber = "+966541000025"
        },

        // =========================================================
        // Active Residents - 25
        // =========================================================

        new()
        {
            DocumentId = "2000000026",
            FullName = "Omar Hassan",
            DateOfBirth = new DateTime(1986, 1, 19),
            PhoneNumber = "+966561000026"
        },
        new()
        {
            DocumentId = "2000000027",
            FullName = "Mohammad Saleh",
            DateOfBirth = new DateTime(1992, 5, 24),
            PhoneNumber = "+966561000027"
        },
        new()
        {
            DocumentId = "2000000028",
            FullName = "Ahmed Ibrahim",
            DateOfBirth = new DateTime(1998, 10, 9),
            PhoneNumber = "+966561000028"
        },
        new()
        {
            DocumentId = "2000000029",
            FullName = "Mahmoud Khalil",
            DateOfBirth = new DateTime(1981, 3, 31),
            PhoneNumber = "+966561000029"
        },
        new()
        {
            DocumentId = "2000000030",
            FullName = "Yousef Nasser",
            DateOfBirth = new DateTime(1995, 7, 12),
            PhoneNumber = "+966561000030"
        },
        new()
        {
            DocumentId = "2100000031",
            FullName = "Khaled Mustafa",
            DateOfBirth = new DateTime(1989, 11, 3),
            PhoneNumber = "+966531000031"
        },
        new()
        {
            DocumentId = "2100000032",
            FullName = "Ibrahim Adel",
            DateOfBirth = new DateTime(2001, 2, 14),
            PhoneNumber = "+966531000032"
        },
        new()
        {
            DocumentId = "2100000033",
            FullName = "Mustafa Samir",
            DateOfBirth = new DateTime(1997, 9, 27),
            PhoneNumber = "+966531000033"
        },
        new()
        {
            DocumentId = "2100000034",
            FullName = "Tariq Mahmoud",
            DateOfBirth = new DateTime(1976, 12, 6),
            PhoneNumber = "+966531000034"
        },
        new()
        {
            DocumentId = "2100000035",
            FullName = "Sami Younis",
            DateOfBirth = new DateTime(1983, 4, 22),
            PhoneNumber = "+966531000035"
        },
        new()
        {
            DocumentId = "2200000036",
            FullName = "Sara Hassan",
            DateOfBirth = new DateTime(1994, 6, 18),
            PhoneNumber = "+966591000036"
        },
        new()
        {
            DocumentId = "2200000037",
            FullName = "Mariam Ibrahim",
            DateOfBirth = new DateTime(2000, 1, 7),
            PhoneNumber = "+966591000037"
        },
        new()
        {
            DocumentId = "2200000038",
            FullName = "Noor Saleh",
            DateOfBirth = new DateTime(1996, 8, 26),
            PhoneNumber = "+966591000038"
        },
        new()
        {
            DocumentId = "2200000039",
            FullName = "Hala Nasser",
            DateOfBirth = new DateTime(1988, 10, 15),
            PhoneNumber = "+966591000039"
        },
        new()
        {
            DocumentId = "2200000040",
            FullName = "Rana Khalil",
            DateOfBirth = new DateTime(2002, 12, 20),
            PhoneNumber = "+966591000040"
        },
        new()
        {
            DocumentId = "2300000041",
            FullName = "Mohammad Hassan",
            DateOfBirth = new DateTime(1980, 5, 11),
            PhoneNumber = "+966581000041"
        },
        new()
        {
            DocumentId = "2300000042",
            FullName = "Ahmed Saleh",
            DateOfBirth = new DateTime(1993, 3, 9),
            PhoneNumber = "+966581000042"
        },
        new()
        {
            DocumentId = "2300000043",
            FullName = "Yousef Ibrahim",
            DateOfBirth = new DateTime(1999, 7, 28),
            PhoneNumber = "+966581000043"
        },
        new()
        {
            DocumentId = "2300000044",
            FullName = "Amina Mahmoud",
            DateOfBirth = new DateTime(1985, 11, 16),
            PhoneNumber = "+966581000044"
        },
        new()
        {
            DocumentId = "2300000045",
            FullName = "Layla Samir",
            DateOfBirth = new DateTime(1991, 9, 4),
            PhoneNumber = "+966581000045"
        },
        new()
        {
            DocumentId = "2400000046",
            FullName = "Ali Hassan",
            DateOfBirth = new DateTime(2003, 4, 13),
            PhoneNumber = "+966571000046"
        },
        new()
        {
            DocumentId = "2400000047",
            FullName = "Hassan Ibrahim",
            DateOfBirth = new DateTime(1977, 8, 8),
            PhoneNumber = "+966571000047"
        },
        new()
        {
            DocumentId = "2400000048",
            FullName = "Nadia Saleh",
            DateOfBirth = new DateTime(1990, 2, 17),
            PhoneNumber = "+966571000048"
        },
        new()
        {
            DocumentId = "2400000049",
            FullName = "Salma Nasser",
            DateOfBirth = new DateTime(1998, 6, 21),
            PhoneNumber = "+966571000049"
        },
        new()
        {
            DocumentId = "2400000050",
            FullName = "Mona Khalil",
            DateOfBirth = new DateTime(1986, 12, 1),
            PhoneNumber = "+966571000050"
        },

        // =========================================================
        // Archived Citizens - 10
        // =========================================================

        new()
        {
            DocumentId = "1300000051",
            FullName = "Abdulrahman Alharbi",
            DateOfBirth = new DateTime(1984, 3, 18),
            PhoneNumber = "+966521000051",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 1, 10, 9, 0, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1300000052",
            FullName = "Mohammad Alzahrani",
            DateOfBirth = new DateTime(1996, 7, 7),
            PhoneNumber = "+966521000052",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 1, 18, 10, 15, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1300000053",
            FullName = "Fahad Alqahtani",
            DateOfBirth = new DateTime(1974, 9, 26),
            PhoneNumber = "+966521000053",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 2, 3, 12, 30, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1300000054",
            FullName = "Nasser Alotaibi",
            DateOfBirth = new DateTime(1989, 1, 30),
            PhoneNumber = "+966521000054",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 2, 15, 14, 0, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1300000055",
            FullName = "Rashed Alshammari",
            DateOfBirth = new DateTime(2001, 5, 14),
            PhoneNumber = "+966521000055",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 2, 28, 15, 10, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1400000056",
            FullName = "Norah Aldosari",
            DateOfBirth = new DateTime(1993, 8, 22),
            PhoneNumber = "+966511000056",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 3, 12, 9, 40, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1400000057",
            FullName = "Hessa Almutairi",
            DateOfBirth = new DateTime(1987, 2, 9),
            PhoneNumber = "+966511000057",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 3, 24, 11, 20, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1400000058",
            FullName = "Sara Alenezi",
            DateOfBirth = new DateTime(1999, 10, 17),
            PhoneNumber = "+966511000058",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 4, 8, 13, 0, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1400000059",
            FullName = "Amal Alghamdi",
            DateOfBirth = new DateTime(1982, 6, 12),
            PhoneNumber = "+966511000059",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 4, 20, 14, 45, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "1400000060",
            FullName = "Reem Alsubaie",
            DateOfBirth = new DateTime(1995, 12, 28),
            PhoneNumber = "+966511000060",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 5, 5, 15, 30, 0, TimeSpan.Zero)
        },

        // =========================================================
        // Archived Residents - 10
        // =========================================================

        new()
        {
            DocumentId = "2500000061",
            FullName = "Mahmoud Hassan",
            DateOfBirth = new DateTime(1979, 4, 25),
            PhoneNumber = "+966561000061",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 1, 14, 9, 30, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2500000062",
            FullName = "Mohammad Ibrahim",
            DateOfBirth = new DateTime(1990, 11, 6),
            PhoneNumber = "+966561000062",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 1, 27, 10, 50, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2500000063",
            FullName = "Ahmed Nasser",
            DateOfBirth = new DateTime(1997, 3, 21),
            PhoneNumber = "+966561000063",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 2, 11, 12, 10, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2500000064",
            FullName = "Samir Saleh",
            DateOfBirth = new DateTime(1985, 8, 13),
            PhoneNumber = "+966561000064",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 2, 25, 13, 35, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2500000065",
            FullName = "Khaled Younis",
            DateOfBirth = new DateTime(1992, 1, 5),
            PhoneNumber = "+966561000065",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 3, 9, 15, 0, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2600000066",
            FullName = "Rania Hassan",
            DateOfBirth = new DateTime(2000, 7, 29),
            PhoneNumber = "+966531000066",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 3, 21, 9, 55, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2600000067",
            FullName = "Mona Ibrahim",
            DateOfBirth = new DateTime(1988, 5, 16),
            PhoneNumber = "+966531000067",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 4, 4, 11, 25, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2600000068",
            FullName = "Heba Mahmoud",
            DateOfBirth = new DateTime(1994, 9, 10),
            PhoneNumber = "+966531000068",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 4, 18, 12, 50, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2600000069",
            FullName = "Dina Saleh",
            DateOfBirth = new DateTime(1981, 12, 23),
            PhoneNumber = "+966531000069",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 5, 2, 14, 20, 0, TimeSpan.Zero)
        },
        new()
        {
            DocumentId = "2600000070",
            FullName = "Amina Nasser",
            DateOfBirth = new DateTime(1998, 2, 4),
            PhoneNumber = "+966531000070",
            IsDeleted = true,
            DeletedAtUtc = new DateTimeOffset(
                2026, 5, 16, 15, 40, 0, TimeSpan.Zero)
        }
    };

            await context.Patients.AddRangeAsync(
                patients,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);


        }


        private static async Task SeedMedicinesAsync(
     AppDbContext context,
     CancellationToken cancellationToken)
        {
            // Include archived medicines when checking whether seed data already exists.
            if (await context.Medicines
                .IgnoreQueryFilters()
                .AnyAsync(cancellationToken))
            {
                return;
            }

            // =========================================================
            // Tablets / Capsules
            // =========================================================

            var panadol500 = new Medicine
            {
                Code = "6281104738291",
                Name = "Panadol",
                Strength = "500 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 24,
                ReorderLevel = 50,
                IsActive = true
            };
            panadol500.IncreaseStock(240);

            var panadolExtra = new Medicine
            {
                Code = "6282573946102",
                Name = "Panadol Extra",
                Strength = "500 mg / 65 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 24,
                ReorderLevel = 40,
                IsActive = true
            };
            panadolExtra.IncreaseStock(35); // Low stock

            var paracetamol = new Medicine
            {
                Code = "5017283946102",
                Name = "Paracetamol",
                Strength = "500 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 50,
                IsActive = true
            };
            paracetamol.IncreaseStock(300);

            var augmentin625 = new Medicine
            {
                Code = "4038172659401",
                Name = "Augmentin",
                Strength = "625 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 14,
                ReorderLevel = 30,
                IsActive = true
            };
            augmentin625.IncreaseStock(84);

            var amoxicillin500 = new Medicine
            {
                Code = "8802417395163",
                Name = "Amoxicillin",
                Strength = "500 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 40,
                IsActive = true
            };
            amoxicillin500.IncreaseStock(30); // Low stock

            var azithromycin500 = new Medicine
            {
                Code = "3159074281635",
                Name = "Azithromycin",
                Strength = "500 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 3,
                ReorderLevel = 15,
                IsActive = true
            };
            azithromycin500.IncreaseStock(30);

            var metformin500 = new Medicine
            {
                Code = "6924183057216",
                Name = "Metformin",
                Strength = "500 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 60,
                IsActive = true
            };
            metformin500.IncreaseStock(240);

            var metformin850 = new Medicine
            {
                Code = "6927351840269",
                Name = "Metformin",
                Strength = "850 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 50,
                IsActive = true
            };
            metformin850.IncreaseStock(45); // Low stock

            var atorvastatin20 = new Medicine
            {
                Code = "7293846150274",
                Name = "Atorvastatin",
                Strength = "20 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 40,
                IsActive = true
            };
            atorvastatin20.IncreaseStock(180);

            var atorvastatin40 = new Medicine
            {
                Code = "7296518304728",
                Name = "Atorvastatin",
                Strength = "40 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 30,
                IsActive = true
            };
            atorvastatin40.IncreaseStock(90);

            var amlodipine5 = new Medicine
            {
                Code = "5013928476150",
                Name = "Amlodipine",
                Strength = "5 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 40,
                IsActive = true
            };
            amlodipine5.IncreaseStock(150);

            var amlodipine10 = new Medicine
            {
                Code = "5018462753194",
                Name = "Amlodipine",
                Strength = "10 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 40,
                IsActive = true
            };
            amlodipine10.IncreaseStock(25); // Low stock

            var losartan50 = new Medicine
            {
                Code = "8805361927408",
                Name = "Losartan",
                Strength = "50 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 40,
                IsActive = true
            };
            losartan50.IncreaseStock(120);

            var bisoprolol5 = new Medicine
            {
                Code = "3152847169035",
                Name = "Bisoprolol",
                Strength = "5 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 35,
                IsActive = true
            };
            bisoprolol5.IncreaseStock(90);

            var aspirin81 = new Medicine
            {
                Code = "4035918274063",
                Name = "Aspirin",
                Strength = "81 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 50,
                IsActive = true
            };
            aspirin81.IncreaseStock(200);

            var clopidogrel75 = new Medicine
            {
                Code = "6287391052846",
                Name = "Clopidogrel",
                Strength = "75 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 28,
                ReorderLevel = 35,
                IsActive = true
            };
            clopidogrel75.IncreaseStock(28); // Low stock

            var omeprazole20 = new Medicine
            {
                Code = "6921574836209",
                Name = "Omeprazole",
                Strength = "20 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 14,
                ReorderLevel = 40,
                IsActive = true
            };
            omeprazole20.IncreaseStock(140);

            var esomeprazole40 = new Medicine
            {
                Code = "7298063415724",
                Name = "Esomeprazole",
                Strength = "40 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 14,
                ReorderLevel = 30,
                IsActive = true
            };
            esomeprazole40.IncreaseStock(70);

            var cetirizine10 = new Medicine
            {
                Code = "5014738296051",
                Name = "Cetirizine",
                Strength = "10 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 30,
                IsActive = true
            };
            cetirizine10.IncreaseStock(100);

            var loratadine10 = new Medicine
            {
                Code = "8807192635048",
                Name = "Loratadine",
                Strength = "10 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 25,
                IsActive = true
            };
            loratadine10.IncreaseStock(20); // Low stock

            var diclofenac50 = new Medicine
            {
                Code = "3156482079315",
                Name = "Diclofenac",
                Strength = "50 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 30,
                IsActive = true
            };
            diclofenac50.IncreaseStock(100);

            var ibuprofen400 = new Medicine
            {
                Code = "4032759186407",
                Name = "Ibuprofen",
                Strength = "400 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 40,
                IsActive = true
            };
            ibuprofen400.IncreaseStock(160);

            var naproxen500 = new Medicine
            {
                Code = "6284917362058",
                Name = "Naproxen",
                Strength = "500 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 30,
                IsActive = true
            };
            naproxen500.IncreaseStock(60);

            var prednisolone5 = new Medicine
            {
                Code = "6928405173264",
                Name = "Prednisolone",
                Strength = "5 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 25,
                IsActive = true
            };
            prednisolone5.IncreaseStock(80);

            var levothyroxine50 = new Medicine
            {
                Code = "7293168504729",
                Name = "Levothyroxine",
                Strength = "50 mcg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 50,
                IsActive = true
            };
            levothyroxine50.IncreaseStock(180);

            var gabapentin300 = new Medicine
            {
                Code = "5019273648150",
                Name = "Gabapentin",
                Strength = "300 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 40,
                IsActive = true
            };
            gabapentin300.IncreaseStock(90);

            var fluconazole150 = new Medicine
            {
                Code = "8803647182059",
                Name = "Fluconazole",
                Strength = "150 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            fluconazole150.IncreaseStock(8); // Low stock

            var doxycycline100 = new Medicine
            {
                Code = "3158204716395",
                Name = "Doxycycline",
                Strength = "100 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 10,
                ReorderLevel = 20,
                IsActive = true
            };
            doxycycline100.IncreaseStock(50);

            var celecoxib200 = new Medicine
            {
                Code = "4036825197401",
                Name = "Celecoxib",
                Strength = "200 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 10,
                ReorderLevel = 20,
                IsActive = true
            };
            celecoxib200.IncreaseStock(60);

            var pregabalin75 = new Medicine
            {
                Code = "6283059714826",
                Name = "Pregabalin",
                Strength = "75 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 14,
                ReorderLevel = 25,
                IsActive = true
            };
            pregabalin75.IncreaseStock(70);

            // =========================================================
            // Syrups / Bottles
            // =========================================================

            var paracetamolSyrup = new Medicine
            {
                Code = "6925731084269",
                Name = "Paracetamol Syrup",
                Strength = "120 mg / 5 mL",
                Form = MedicineForm.Syrup,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 15,
                IsActive = true
            };
            paracetamolSyrup.IncreaseStock(40);

            var ibuprofenSyrup = new Medicine
            {
                Code = "7291482635074",
                Name = "Ibuprofen Syrup",
                Strength = "100 mg / 5 mL",
                Form = MedicineForm.Syrup,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 15,
                IsActive = true
            };
            ibuprofenSyrup.IncreaseStock(12); // Low stock

            var amoxicillinSuspension = new Medicine
            {
                Code = "5016859204731",
                Name = "Amoxicillin Suspension",
                Strength = "250 mg / 5 mL",
                Form = MedicineForm.Syrup,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 12,
                IsActive = true
            };
            amoxicillinSuspension.IncreaseStock(30);

            var augmentinSuspension = new Medicine
            {
                Code = "8809274613058",
                Name = "Augmentin Suspension",
                Strength = "457 mg / 5 mL",
                Form = MedicineForm.Syrup,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            augmentinSuspension.IncreaseStock(25);

            var lactulose = new Medicine
            {
                Code = "3154739206815",
                Name = "Lactulose",
                Strength = "3.35 g / 5 mL",
                Form = MedicineForm.Syrup,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            lactulose.IncreaseStock(20);

            // =========================================================
            // Creams / Tubes
            // =========================================================

            var hydrocortisoneCream = new Medicine
            {
                Code = "4039182746502",
                Name = "Hydrocortisone Cream",
                Strength = "1%",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 12,
                IsActive = true
            };
            hydrocortisoneCream.IncreaseStock(35);

            var clotrimazoleCream = new Medicine
            {
                Code = "6281573948206",
                Name = "Clotrimazole Cream",
                Strength = "1%",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            clotrimazoleCream.IncreaseStock(8); // Low stock

            var fusidicAcidCream = new Medicine
            {
                Code = "6923048175269",
                Name = "Fusidic Acid Cream",
                Strength = "2%",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            fusidicAcidCream.IncreaseStock(25);

            var acyclovirCream = new Medicine
            {
                Code = "7295810364279",
                Name = "Acyclovir Cream",
                Strength = "5%",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 8,
                IsActive = true
            };
            acyclovirCream.IncreaseStock(15);

            var betamethasoneCream = new Medicine
            {
                Code = "5012406839751",
                Name = "Betamethasone Cream",
                Strength = "0.1%",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            betamethasoneCream.IncreaseStock(20);

            // =========================================================
            // Injections
            // =========================================================

            var ceftriaxone1g = new Medicine
            {
                Code = "8806139472058",
                Name = "Ceftriaxone",
                Strength = "1 g",
                Form = MedicineForm.Injection,
                StockUnit = StockUnit.Vial,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 20,
                IsActive = true
            };
            ceftriaxone1g.IncreaseStock(60);

            var omeprazoleInjection = new Medicine
            {
                Code = "3157962048315",
                Name = "Omeprazole Injection",
                Strength = "40 mg",
                Form = MedicineForm.Injection,
                StockUnit = StockUnit.Vial,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 15,
                IsActive = true
            };
            omeprazoleInjection.IncreaseStock(12); // Low stock

            var dexamethasoneInjection = new Medicine
            {
                Code = "4031478295602",
                Name = "Dexamethasone Injection",
                Strength = "8 mg / 2 mL",
                Form = MedicineForm.Injection,
                StockUnit = StockUnit.Ampoule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 10,
                ReorderLevel = 20,
                IsActive = true
            };
            dexamethasoneInjection.IncreaseStock(50);

            var diclofenacInjection = new Medicine
            {
                Code = "6289047163528",
                Name = "Diclofenac Injection",
                Strength = "75 mg / 3 mL",
                Form = MedicineForm.Injection,
                StockUnit = StockUnit.Ampoule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 10,
                ReorderLevel = 20,
                IsActive = true
            };
            diclofenacInjection.IncreaseStock(40);

            var metoclopramideInjection = new Medicine
            {
                Code = "6926813052479",
                Name = "Metoclopramide Injection",
                Strength = "10 mg / 2 mL",
                Form = MedicineForm.Injection,
                StockUnit = StockUnit.Ampoule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 10,
                ReorderLevel = 20,
                IsActive = true
            };
            metoclopramideInjection.IncreaseStock(60);

            // =========================================================
            // Drops
            // =========================================================

            var artificialTears = new Medicine
            {
                Code = "7294208156379",
                Name = "Artificial Tears",
                Strength = "0.5%",
                Form = MedicineForm.Drops,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 15,
                IsActive = true
            };
            artificialTears.IncreaseStock(40);

            var chloramphenicolDrops = new Medicine
            {
                Code = "5017632948051",
                Name = "Chloramphenicol Eye Drops",
                Strength = "0.5%",
                Form = MedicineForm.Drops,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            chloramphenicolDrops.IncreaseStock(9); // Low stock

            var olopatadineDrops = new Medicine
            {
                Code = "8802586173049",
                Name = "Olopatadine Eye Drops",
                Strength = "0.1%",
                Form = MedicineForm.Drops,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            olopatadineDrops.IncreaseStock(20);

            var ciprofloxacinDrops = new Medicine
            {
                Code = "3159614827305",
                Name = "Ciprofloxacin Eye Drops",
                Strength = "0.3%",
                Form = MedicineForm.Drops,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true
            };
            ciprofloxacinDrops.IncreaseStock(25);

            var sodiumChlorideDrops = new Medicine
            {
                Code = "4035207189462",
                Name = "Sodium Chloride Nasal Drops",
                Strength = "0.9%",
                Form = MedicineForm.Drops,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 12,
                IsActive = true
            };
            sodiumChlorideDrops.IncreaseStock(30);

            // =========================================================
            // Active - Out Of Stock
            // QuantityInStock intentionally remains 0
            // =========================================================

            var montelukast10 = new Medicine
            {
                Code = "6283419752068",
                Name = "Montelukast",
                Strength = "10 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 30,
                IsActive = true
            };

            var glimepiride2 = new Medicine
            {
                Code = "6929183047256",
                Name = "Glimepiride",
                Strength = "2 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 30,
                IsActive = true
            };

            var pantoprazole40 = new Medicine
            {
                Code = "7292736481059",
                Name = "Pantoprazole",
                Strength = "40 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 30,
                IsActive = true
            };

            var furosemide40 = new Medicine
            {
                Code = "5018947203615",
                Name = "Furosemide",
                Strength = "40 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 20,
                IsActive = true
            };

            var spironolactone25 = new Medicine
            {
                Code = "8804719263058",
                Name = "Spironolactone",
                Strength = "25 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 20,
                IsActive = true
            };

            // =========================================================
            // Inactive
            // Some intentionally still have stock.
            // =========================================================

            var captopril25 = new Medicine
            {
                Code = "3157305842196",
                Name = "Captopril",
                Strength = "25 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 30,
                IsActive = false
            };
            captopril25.IncreaseStock(90);

            var ranitidine150 = new Medicine
            {
                Code = "4038612957402",
                Name = "Ranitidine",
                Strength = "150 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 20,
                IsActive = false
            };

            var erythromycin500 = new Medicine
            {
                Code = "6285203947168",
                Name = "Erythromycin",
                Strength = "500 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 25,
                IsActive = false
            };
            erythromycin500.IncreaseStock(40);

            var ketoconazoleCream = new Medicine
            {
                Code = "6923748152069",
                Name = "Ketoconazole Cream",
                Strength = "2%",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = false
            };
            ketoconazoleCream.IncreaseStock(15);

            var gentamicinInjection = new Medicine
            {
                Code = "7296158204739",
                Name = "Gentamicin Injection",
                Strength = "80 mg / 2 mL",
                Form = MedicineForm.Injection,
                StockUnit = StockUnit.Ampoule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 10,
                ReorderLevel = 20,
                IsActive = false
            };
            gentamicinInjection.IncreaseStock(30);

            // =========================================================
            // Archived
            // IsActive intentionally mixed because archive state
            // is independent from operational active/inactive state.
            // =========================================================

            var archivedEnalapril = new Medicine
            {
                Code = "5012369485701",
                Name = "Enalapril",
                Strength = "10 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 30,
                IsActive = true,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 2, 5, 10, 0, 0, TimeSpan.Zero)
            };

            var archivedSimvastatin = new Medicine
            {
                Code = "8805937162048",
                Name = "Simvastatin",
                Strength = "20 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 30,
                ReorderLevel = 30,
                IsActive = false,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 2, 18, 11, 30, 0, TimeSpan.Zero)
            };

            var archivedFamotidine = new Medicine
            {
                Code = "3158472609315",
                Name = "Famotidine",
                Strength = "20 mg",
                Form = MedicineForm.Tablet,
                StockUnit = StockUnit.Tablet,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 20,
                IsActive = true,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 3, 3, 9, 15, 0, TimeSpan.Zero)
            };

            var archivedMiconazole = new Medicine
            {
                Code = "4036928147502",
                Name = "Miconazole Cream",
                Strength = "2%",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = false,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 3, 17, 13, 0, 0, TimeSpan.Zero)
            };

            var archivedAmpicillin = new Medicine
            {
                Code = "6282739514068",
                Name = "Ampicillin",
                Strength = "500 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 25,
                IsActive = true,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 4, 1, 8, 45, 0, TimeSpan.Zero)
            };

            var archivedNystatin = new Medicine
            {
                Code = "6925061843279",
                Name = "Nystatin Cream",
                Strength = "100,000 units/g",
                Form = MedicineForm.Cream,
                StockUnit = StockUnit.Tube,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 8,
                IsActive = false,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 4, 16, 14, 20, 0, TimeSpan.Zero)
            };

            var archivedCefalexin = new Medicine
            {
                Code = "7298304715269",
                Name = "Cefalexin",
                Strength = "500 mg",
                Form = MedicineForm.Capsule,
                StockUnit = StockUnit.Capsule,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 20,
                ReorderLevel = 20,
                IsActive = true,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 5, 2, 10, 30, 0, TimeSpan.Zero)
            };

            var archivedSalbutamol = new Medicine
            {
                Code = "5016073928451",
                Name = "Salbutamol",
                Strength = "2 mg / 5 mL",
                Form = MedicineForm.Syrup,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = false,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 5, 19, 12, 0, 0, TimeSpan.Zero)
            };

            var archivedTobramycin = new Medicine
            {
                Code = "8803147596208",
                Name = "Tobramycin Eye Drops",
                Strength = "0.3%",
                Form = MedicineForm.Drops,
                StockUnit = StockUnit.Bottle,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = true,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 6, 4, 9, 30, 0, TimeSpan.Zero)
            };

            var archivedHydrocortisone = new Medicine
            {
                Code = "3159284076315",
                Name = "Hydrocortisone Injection",
                Strength = "100 mg",
                Form = MedicineForm.Injection,
                StockUnit = StockUnit.Vial,
                PackageUnit = PackageUnit.Box,
                UnitsPerPackage = 1,
                ReorderLevel = 10,
                IsActive = false,
                IsDeleted = true,
                DeletedAtUtc = new DateTimeOffset(
                    2026, 6, 20, 15, 0, 0, TimeSpan.Zero)
            };

            // =========================================================
            // Add all medicines
            // =========================================================

            var medicines = new List<Medicine>
    {
        panadol500,
        panadolExtra,
        paracetamol,
        augmentin625,
        amoxicillin500,
        azithromycin500,
        metformin500,
        metformin850,
        atorvastatin20,
        atorvastatin40,
        amlodipine5,
        amlodipine10,
        losartan50,
        bisoprolol5,
        aspirin81,
        clopidogrel75,
        omeprazole20,
        esomeprazole40,
        cetirizine10,
        loratadine10,
        diclofenac50,
        ibuprofen400,
        naproxen500,
        prednisolone5,
        levothyroxine50,
        gabapentin300,
        fluconazole150,
        doxycycline100,
        celecoxib200,
        pregabalin75,
        paracetamolSyrup,
        ibuprofenSyrup,
        amoxicillinSuspension,
        augmentinSuspension,
        lactulose,
        hydrocortisoneCream,
        clotrimazoleCream,
        fusidicAcidCream,
        acyclovirCream,
        betamethasoneCream,
        ceftriaxone1g,
        omeprazoleInjection,
        dexamethasoneInjection,
        diclofenacInjection,
        metoclopramideInjection,
        artificialTears,
        chloramphenicolDrops,
        olopatadineDrops,
        ciprofloxacinDrops,
        sodiumChlorideDrops,
        montelukast10,
        glimepiride2,
        pantoprazole40,
        furosemide40,
        spironolactone25,
        captopril25,
        ranitidine150,
        erythromycin500,
        ketoconazoleCream,
        gentamicinInjection,
        archivedEnalapril,
        archivedSimvastatin,
        archivedFamotidine,
        archivedMiconazole,
        archivedAmpicillin,
        archivedNystatin,
        archivedCefalexin,
        archivedSalbutamol,
        archivedTobramycin,
        archivedHydrocortisone
    };

            await context.Medicines.AddRangeAsync(
                medicines,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

    }

    }