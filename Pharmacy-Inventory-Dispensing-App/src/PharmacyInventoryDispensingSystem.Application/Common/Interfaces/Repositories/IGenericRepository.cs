using PharmacyInventoryDispensingSystem.Domain.Common;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;


public interface IGenericRepository<TEntity> where TEntity : Entity
{
    
    void Add(TEntity entity);

   
    void Update(TEntity entity);

    /// <summary>
    /// Performs a soft delete by setting <c>IsDeleted = true</c> on the entity.
    /// Only applicable to entities inheriting from <see cref="SoftDeletableEntity"/>.
    /// Audit fields (<c>DeletedAtUtc</c>, <c>DeletedBy</c>) are populated by the
    /// <c>AuditableEntityInterceptor</c> on <c>SaveChangesAsync</c>.
    /// </summary>
    void Delete(TEntity entity);

    void HardDelete(TEntity entity);


    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns an untracked <see cref="IQueryable{TEntity}"/> over the entity set.
    /// Global query filters (e.g., soft-delete) are applied by default.
    /// <para>
    /// Use <see cref="IAppDbContext"/> directly when you need
    /// <c>IgnoreQueryFilters()</c> or tracked queries.
    /// </para>
    /// </summary>
    IQueryable<TEntity> Query();

    /// <summary>
    /// Returns an untracked <see cref="IQueryable{TEntity}"/> over the entity set.
    /// <param name="trackChanges">Set to <c>true</c> to attach the query to the change tracker.</param>
    /// </summary>
    IQueryable<TEntity> QueryIncludingDeleted(bool trackChanges = false);

    
}
