using Iskra.Application.Abstractions.Repositories.Base;
using Iskra.Application.Abstractions.Services.Base;
using Iskra.Application.Abstractions.Validation;
using Iskra.Application.Results;
using Iskra.Core.Domain.Common;

namespace Iskra.Application.Services.Base;

/// <summary>
/// Provides a base implementation for a generic entity service with standard CRUD operations.
/// </summary>
internal abstract class EntityService<TEntity, TId, TRepository>
    (TRepository repository, IUnitOfWork unitOfWork, IValidationService<TEntity> validationService)
    : IEntityService<TEntity, TId>
    where TEntity : class, IBaseEntity<TId>
    where TId : notnull, IComparable<TId>
    where TRepository : IEntityOperations<TEntity, TId>
{
    protected readonly TRepository Repository = repository;
    protected readonly IUnitOfWork UnitOfWork = unitOfWork;
    protected readonly IValidationService<TEntity> ValidationService = validationService;

    public virtual async Task<Result<TEntity>> GetByIdAsync(TId entityId, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(entityId, cancellationToken);
        return entity is null ? EntityErrors<TEntity, TId>.NotFoundById(entityId) : Result<TEntity>.Ok(entity);
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Repository.GetAllAsync(cancellationToken);
    }

    public virtual async Task<Result> CheckExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default)
    {
        if (entityId is null)
            return EntityErrors<TEntity, TId>.NotFound;

        return await Repository.ExistsByIdAsync(entityId, cancellationToken)
            ? Result.Ok()
            : EntityErrors<TEntity, TId>.NotFoundById(entityId);
    }

    public virtual async Task<Result<TEntity>> CreateAsync(TEntity newEntity)
    {
        if (newEntity is null) return EntityErrors<TEntity, TId>.CreateNullFailure;

        var validationResult = await ValidationService.ValidateAsync(newEntity);
        if (validationResult.IsFailure) return validationResult.ToValue<TEntity>();

        Repository.Add(newEntity);
        await UnitOfWork.SaveChangesAsync();
        return Result<TEntity>.Ok(newEntity);
    }

    public virtual async Task<Result<TEntity>> UpdateAsync(TEntity changedEntity)
    {
        if (changedEntity is null) return EntityErrors<TEntity, TId>.UpdateNullFailure;

        var existsResult = await CheckExistsByIdAsync(changedEntity.Id, default);
        if (existsResult.IsFailure) return existsResult.ToValue<TEntity>();

        var validationResult = await ValidationService.ValidateAsync(changedEntity);
        if (validationResult.IsFailure) return validationResult.ToValue<TEntity>();

        Repository.Update(changedEntity);
        await UnitOfWork.SaveChangesAsync();
        return Result<TEntity>.Ok(changedEntity);
    }

    public virtual async Task<Result> DeleteByIdAsync(TId entityId)
    {
        var entityResult = await GetByIdAsync(entityId, default);
        if (entityResult.IsFailure) return entityResult;

        Repository.Remove(entityResult.Value!);
        await UnitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}