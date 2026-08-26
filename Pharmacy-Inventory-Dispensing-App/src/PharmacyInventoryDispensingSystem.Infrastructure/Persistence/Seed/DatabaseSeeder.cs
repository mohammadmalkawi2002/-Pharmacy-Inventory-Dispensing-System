using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
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
        internal const string ReceptionistEmail = "reception@pharmacy.local";
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

        private static async Task SeedRolePermissionsAsync(RoleManager<IdentityRole> roleManager, CancellationToken cancellationToken)
        {
            //1] create dictionary to store  each role with her permissions

            var rolePermissions = new Dictionary<string, string[]>()
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

                    Permissions.Prescriptions.Create,
                    Permissions.Prescriptions.Update,
                    Permissions.Prescriptions.Read,
                    Permissions.Prescriptions.Cancel,
                    Permissions.Prescriptions.Lookup,

                ],

                [RoleNames.Pharmacist] =
                [
                     Permissions.Medicines.Read,
                     Permissions.Medicines.Create,
                     Permissions.Medicines.Update,
                     Permissions.Medicines.Activate,
                     Permissions.Medicines.Deactivate,
                     Permissions.Medicines.ReadLowStock,

                     Permissions.Prescriptions.Read,
                     Permissions.Prescriptions.Lookup,

                     Permissions.Dispenses.Read,
                     Permissions.Dispenses.Create
                ]

            };

            foreach (var rolePermission in rolePermissions)
            {
                cancellationToken.ThrowIfCancellationRequested();
                //2] Find or get the role:
                var role = await roleManager.FindByNameAsync(rolePermission.Key);

                if (role is null)
                {
                    continue;
                }

                //3] Get the existingClaims  if found :

                var existingPermissions = (await roleManager.GetClaimsAsync(role))
                                    .Where(claim => claim.Type == ApplicationClaimTypes.Permission)
                                    .Select(claim => claim.Value)
                                    .ToHashSet();


                foreach (var permission in rolePermission.Value)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (existingPermissions.Contains(permission))
                    {
                        continue;

                    }


                    var result = await roleManager.AddClaimAsync(role,
                            new Claim(ApplicationClaimTypes.Permission, permission

                            ));

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                                             ", ",
                        result.Errors.Select(error => error.Description));

                        throw new InvalidOperationException(
                            $"Failed to seed permission '{permission}' " +
                            $"for role '{role.Name}': {errors}");
                    }

                    existingPermissions.Add(permission);
                }


            }


        }

        private static async Task<(ApplicationUser Admin,ApplicationUser Pharmacist,ApplicationUser Doctor,ApplicationUser Receptionist)> SeedUsersAsync(
            UserManager<ApplicationUser> userManager)
        {
            var admin = await EnsureUserAsync(userManager,
                            email: AdminEmail,
                          firstName: "System ",
                          lastName: "Administrator",
                          password: AdminPassword,
                          role: RoleNames.Admin);

            var pharmacist = await EnsureUserAsync(
                        userManager,
                        email: PharmacistEmail,
                        firstName: "Default Pharmacist",
                        lastName:".",
                        password: DefaultPassword,
                        role: RoleNames.Pharmacist);



            var doctor = await EnsureUserAsync(
                         userManager,
                         email: DoctorEmail,
                         firstName: "System ",
                          lastName:"Doctor",
                         password: DefaultPassword,
                         role: RoleNames.Doctor);


            var receptionist = await EnsureUserAsync(
                         userManager,
                         email: "receptionist@pharmacy.local",
                         firstName: "System",
                         lastName: "Receptionist",
                         password: "User#12345!",
                         role: RoleNames.Receptionist);

            return (admin,pharmacist, doctor, receptionist);

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

    }
}