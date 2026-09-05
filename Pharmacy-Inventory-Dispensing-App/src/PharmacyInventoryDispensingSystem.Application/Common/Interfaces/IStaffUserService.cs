using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces
{
    /// <summary>
    /// Application-layer abstraction for admin staff user management.
    /// Implemented by Infrastructure; Application handlers never reference Identity types directly.
    /// </summary>
    public interface IStaffUserService
    {
        Task<Result<PaginatedList<StaffUserDto>>> GetPagedAsync(
            string? searchTerm,
            string? role,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<StaffUserDto>> GetByIdAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task<Result<StaffUserDto>> CreateAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            string role,
            CancellationToken cancellationToken = default);

        Task<Result<Updated>> UpdateAsync(
            string userId,
            string firstName,
            string lastName,
            string email,
            string role,
            CancellationToken cancellationToken = default);

        Task<Result<Updated>> ActivateAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task<Result<Updated>> DeactivateAsync(
            string userId,
            CancellationToken cancellationToken = default);
    }
}
