using Iskra.Application.Abstractions.Repositories.Base;
using Iskra.Core.Domain.Common;
using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Infrastructure.Persistence.Repositories.Base;

/// <summary>
/// A generic repository implementing standard IEntityOperations.
/// </summary>
internal abstract class EntityRepository<TEntity, TId>(AppDbContextBase dbContext)
    : Repository<TEntity>(dbContext), IEntityOperations<TEntity, TId>
    where TEntity : class, IBaseEntity<TId>
    where TId : notnull, IComparable<TId>
{
    public virtual void Add(TEntity entity)
    {
        Entities.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        Entities.AddRange(entities);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Entities.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        Entities.Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        Entities.Remove(entity);
    }
}