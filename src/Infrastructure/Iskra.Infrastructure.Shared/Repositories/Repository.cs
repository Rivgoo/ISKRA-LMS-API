using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Common;
using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Infrastructure.Persistence.Repositories;

/// <summary>
/// A marker base class for repositories.
/// </summary>
public abstract class Repository(AppDbContextBase dbContext) : IRepository
{
    protected readonly AppDbContextBase DbContext = dbContext;
}

/// <summary>
/// A base class providing typed DbSet access.
/// </summary>
public abstract class Repository<TEntity>(AppDbContextBase dbContext) : Repository(dbContext)
    where TEntity : class, IEntity
{
    protected readonly DbSet<TEntity> Entities = dbContext.Set<TEntity>();
}