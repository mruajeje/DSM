using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using CarGo.Application.Abstractions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
using CarGo.Infrastructure.Exceptions;

namespace CarGo.Infrastructure.Repositories;

public sealed class ResenaRepository : RepositoryBase<Resena>, IResenaRepository
{
    public ResenaRepository(ILogger<ResenaRepository>? logger = null)
        : base(logger)
    {
    }

    public ResenaRepository(ISession session, ILogger<ResenaRepository>? logger = null)
        : base(session, logger)
    {
    }

    public ResenaRepository(ISessionFactory sessionFactory, ILogger<ResenaRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Resena>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Resena>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Resena?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Resena>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Resena entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Resena entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}