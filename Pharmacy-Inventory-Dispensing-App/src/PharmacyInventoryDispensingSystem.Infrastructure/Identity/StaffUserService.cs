using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity
{
    
    public sealed class StaffUserService(
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        ILogger<StaffUserService> logger)
        : IStaffUserService
    {
        // ---------------------------------------------------------------------------
        // READ: paginated list
        // ---------------------------------------------------------------------------

        public async Task<Result<PaginatedList<StaffUserDto>>> GetPagedAsync(
            string? searchTerm,
            string? role,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // Single join query: AspNetUsers → AspNetUserRoles → AspNetRoles.
            // Admin accounts are excluded at the SQL level so they are never loaded.
            var query = context.Users.AsNoTracking()
                .Join(
                    context.UserRoles.AsNoTracking(),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole }
                )
                .Join(
                    context.Roles.AsNoTracking(),
                    joined => joined.userRole.RoleId,
                    identityRole => identityRole.Id,
                    (joined, identityRole) => new { joined.user, RoleName = identityRole.Name! }
                )
                .Where(x => x.RoleName != RoleNames.Admin);



            // Optional role filter
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(x => x.RoleName == role);
            }

            // Optional search (first name, last name, or email)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.user.FirstName.ToLower().Contains(term) ||
                    x.user.LastName.ToLower().Contains(term) ||
                    x.user.Email!.ToLower().Contains(term));
            }


            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.user.CreatedAtUtc)
                .ThenByDescending(x => x.user.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StaffUserDto(
                    x.user.Id,
                    x.user.FirstName,
                    x.user.LastName,
                    x.user.Email!,
                    x.RoleName,
                    x.user.IsActive,
                    x.user.CreatedAtUtc))
                .ToListAsync(cancellationToken);

            return new PaginatedList<StaffUserDto>(items, totalCount, pageNumber, pageSize);
        }

        // ---------------------------------------------------------------------------
        // READ: single user by ID
        // ---------------------------------------------------------------------------

        public async Task<Result<StaffUserDto>> GetByIdAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var dto = await context.Users.AsNoTracking()
                .Join(
                    context.UserRoles.AsNoTracking(),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole }
                )
                .Join(
                    context.Roles.AsNoTracking(),
                    joined => joined.userRole.RoleId,
                    identityRole => identityRole.Id,
                    (joined, identityRole) => new { joined.user, identityRole }
                )
                .Where(x => x.user.Id == userId && x.identityRole.Name != RoleNames.Admin)
                .Select(x => new StaffUserDto(
                    x.user.Id,
                    x.user.FirstName,
                    x.user.LastName,
                    x.user.Email!,
                    x.identityRole.Name!,
                    x.user.IsActive,
                    x.user.CreatedAtUtc))
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
            {
                logger.LogWarning(
                    "Staff user with ID {UserId} was not found or is an admin account",
                    userId);

                // Return NotFound without revealing whether the user is an Admin.
                return UserErrors.NotFound(userId);
            }

            return dto;
        }

        // ---------------------------------------------------------------------------
        // WRITE: create staff user (atomic via explicit DB transaction)
       
        
        // ---------------------------------------------------------------------------

        public async Task<Result<StaffUserDto>> CreateAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            string role,
            CancellationToken cancellationToken = default)
        {
            // Email uniqueness check (before opening a transaction to keep it short)
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
            {
                logger.LogWarning(
                    "Staff user creation rejected: email {Email} already exists",
                    email);

                return UserErrors.EmailConflict;
            }

            // Open an explicit transaction so that CreateAsync + AddToRoleAsync
            // are committed or rolled back as a single unit.
            await using var transaction =
                await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    logger.LogWarning(
                        "Identity failed to create staff user {Email}. Errors: {Errors}",
                        email,
                        string.Join(", ", createResult.Errors.Select(e => e.Code)));

                    return createResult.Errors
                        .Select(e => Error.Validation(e.Code, e.Description))
                        .ToList();
                }

                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    // Role assignment failed: roll back the transaction so the
                    // user row created by CreateAsync is also removed.
                    await transaction.RollbackAsync(cancellationToken);

                    logger.LogError(
                        "Failed to assign role {Role} to user {UserId}. Rolling back. Errors: {Errors}",
                        role,
                        user.Id,
                        string.Join(", ", roleResult.Errors.Select(e => e.Code)));

                    return roleResult.Errors
                        .Select(e => Error.Validation(e.Code, e.Description))
                        .ToList();
                }

                await transaction.CommitAsync(cancellationToken);

                return new StaffUserDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    role,
                    user.IsActive,
                    user.CreatedAtUtc);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                logger.LogError(
                    ex,
                    "Unexpected error during staff user creation for {Email}. Transaction rolled back.",
                    email);

                throw;
            }
        }

        // ---------------------------------------------------------------------------
        // WRITE: update staff user (fully atomic via explicit DB transaction)
       
     
        // ---------------------------------------------------------------------------

        public async Task<Result<Updated>> UpdateAsync(
            string userId,
            string firstName,
            string lastName,
            string email,
            string role,
            CancellationToken cancellationToken = default)
        {
           

            var user = await userManager.FindByIdAsync(userId);

            if (user is null || await IsAdminAsync(user))
            {
                logger.LogWarning(
                    "Staff user update rejected: user {UserId} not found or is an admin",
                    userId);

                return UserErrors.NotFound(userId);
            }

            var emailOwner = await userManager.FindByEmailAsync(email);
            if (emailOwner is not null && emailOwner.Id != userId)
            {
                logger.LogWarning(
                    "Staff user update rejected: email {Email} already in use by another user",
                    email);

                return UserErrors.EmailConflict;
            }

            // Determine whether the role is actually changing before opening the
            // transaction, so the decision is made on stable pre-transaction data.
            var currentRoles = await userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();
            bool roleChanging = currentRole != role;

            bool emailChanging = !string.Equals(
                user.Email, email, StringComparison.OrdinalIgnoreCase);

            

            await using var transaction =
                await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // ── Profile: FirstName / LastName ──────────────────────────────────
                user.FirstName = firstName.Trim();
                user.LastName = lastName.Trim();

                if (emailChanging)
                {
                    var setUserNameResult = await userManager.SetUserNameAsync(user, email);
                    if (!setUserNameResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        logger.LogError(
                            "Failed to update username for user {UserId}. Rolling back. Errors: {Errors}",
                            userId,
                            string.Join(", ", setUserNameResult.Errors.Select(e => e.Code)));

                        return setUserNameResult.Errors
                            .Select(e => Error.Failure(e.Code, e.Description))
                            .ToList();
                    }

                    var setEmailResult = await userManager.SetEmailAsync(user, email);
                    if (!setEmailResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        logger.LogError(
                            "Failed to update email for user {UserId}. Rolling back. Errors: {Errors}",
                            userId,
                            string.Join(", ", setEmailResult.Errors.Select(e => e.Code)));

                        return setEmailResult.Errors
                            .Select(e => Error.Failure(e.Code, e.Description))
                            .ToList();
                    }
                }
                else
                {
                    // Email unchanged: UpdateAsync persists FirstName / LastName.
                    var profileResult = await userManager.UpdateAsync(user);
                    if (!profileResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        logger.LogError(
                            "Identity failed to persist profile update for user {UserId}. Rolling back. Errors: {Errors}",
                            userId,
                            string.Join(", ", profileResult.Errors.Select(e => e.Code)));

                        return profileResult.Errors
                            .Select(e => Error.Failure(e.Code, e.Description))
                            .ToList();
                    }
                }

                // ── Role change (inside the same transaction) ──────────────────────
                
                {
                    var addRoleResult = await userManager.AddToRoleAsync(user, role);
                    if (!addRoleResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        logger.LogError(
                            "Failed to add new role {Role} to user {UserId}. Rolling back. Errors: {Errors}",
                            role,
                            userId,
                            string.Join(", ", addRoleResult.Errors.Select(e => e.Code)));

                        return addRoleResult.Errors
                            .Select(e => Error.Validation(e.Code, e.Description))
                            .ToList();
                    }

                    if (currentRole is not null)
                    {
                        var removeRoleResult =
                            await userManager.RemoveFromRoleAsync(user, currentRole);

                        if (!removeRoleResult.Succeeded)
                        {
                            await transaction.RollbackAsync(cancellationToken);

                            logger.LogError(
                                "Failed to remove old role {OldRole} from user {UserId}. Rolling back. Errors: {Errors}",
                                currentRole,
                                userId,
                                string.Join(", ", removeRoleResult.Errors.Select(e => e.Code)));

                            return removeRoleResult.Errors
                                .Select(e => Error.Failure(e.Code, e.Description))
                                .ToList();
                        }
                    }
                }

                await transaction.CommitAsync(cancellationToken);

                logger.LogInformation(
                    "Staff user {UserId} updated successfully. RoleChanged: {RoleChanged}",
                    userId,
                    roleChanging);

                return Result.Updated;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                logger.LogError(
                    ex,
                    "Unexpected error during staff user update for {UserId}. Transaction rolled back.",
                    userId);

                throw;
            }
        }

        // ---------------------------------------------------------------------------
        // WRITE: activate
        // ---------------------------------------------------------------------------

        public async Task<Result<Updated>> ActivateAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null || await IsAdminAsync(user))
            {
                logger.LogWarning(
                    "Activation rejected: user {UserId} not found or is an admin",
                    userId);

                return UserErrors.NotFound(userId);
            }

            if (user.IsActive)
            {
                logger.LogWarning(
                    "Activation rejected: user {UserId} is already active",
                    userId);

                return UserErrors.AlreadyActive;
            }

            user.IsActive = true;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                logger.LogError(
                    "Identity failed to activate user {UserId}. Errors: {Errors}",
                    userId,
                    string.Join(", ", result.Errors.Select(e => e.Code)));

                return result.Errors
                    .Select(e => Error.Failure(e.Code, e.Description))
                    .ToList();
            }

            return Result.Updated;
        }

        // ---------------------------------------------------------------------------
        // WRITE: deactivate
        // ---------------------------------------------------------------------------

        public async Task<Result<Updated>> DeactivateAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null || await IsAdminAsync(user))
            {
                logger.LogWarning(
                    "Deactivation rejected: user {UserId} not found or is an admin",
                    userId);

                return UserErrors.NotFound(userId);
            }

            if (!user.IsActive)
            {
                logger.LogWarning(
                    "Deactivation rejected: user {UserId} is already inactive",
                    userId);

                return UserErrors.AlreadyInactive;
            }

            user.IsActive = false;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                logger.LogError(
                    "Identity failed to deactivate user {UserId}. Errors: {Errors}",
                    userId,
                    string.Join(", ", result.Errors.Select(e => e.Code)));

                return result.Errors
                    .Select(e => Error.Failure(e.Code, e.Description))
                    .ToList();
            }

            return Result.Updated;
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Returns true if the user has the Admin role.
        /// Used server-side to prevent any management of admin accounts.
        /// </summary>
        private async Task<bool> IsAdminAsync(ApplicationUser user)
            => await userManager.IsInRoleAsync(user, RoleNames.Admin);
    }
}
