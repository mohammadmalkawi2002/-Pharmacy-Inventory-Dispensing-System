using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity>(AppDbContext context)
    : IGenericRepository<TEntity>
    where TEntity : Entity
{
    protected AppDbContext DbContext => context;

    public void Add(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        context.Set<TEntity>().Update(entity);
    }


    public void Delete(TEntity entity)
    {
        if (entity is SoftDeletableEntity softDeletable)
        {
            softDeletable.Delete();
            context.Set<TEntity>().Update(entity);

        }

        else
        {
            
            throw new InvalidOperationException(
                $"Entity of type {typeof(TEntity).Name} does not support Soft Delete. " +
                $"Please use HardDelete() method instead.");
        }

    }

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entity => entity.Id == id,
                cancellationToken);
    }

    public IQueryable<TEntity> Query()
    {
        return context.Set<TEntity>().AsNoTracking();
    }

     public IQueryable<TEntity> QueryIncludingDeleted(bool trackChanges = false)
    {
        var query = context.Set<TEntity>().IgnoreQueryFilters();
        
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }
        return query;
    }

    public void HardDelete(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }
}
