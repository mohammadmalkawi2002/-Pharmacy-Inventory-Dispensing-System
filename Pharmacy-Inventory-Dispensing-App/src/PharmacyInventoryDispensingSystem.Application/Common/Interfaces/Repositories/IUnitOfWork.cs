namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
