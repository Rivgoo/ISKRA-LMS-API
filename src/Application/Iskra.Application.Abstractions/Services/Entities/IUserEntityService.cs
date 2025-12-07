using Iskra.Application.Abstractions.Services.Base;
using Iskra.Core.Domain.Entities;

namespace Iskra.Application.Abstractions.Services.Entities;

public interface IUserEntityService : IEntityService<User, Guid> { }