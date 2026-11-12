using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using CarGo.Application.Abstractions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
using CarGo.Infrastructure.Exceptions;

namespace CarGo.Infrastructure.Repositories;

public sealed class MensajeRepository : RepositoryBase<Mensaje>, IMensajeRepository
{
    public MensajeRepository(ILogger<MensajeRepository>? logger = null)
        : base(logger)
    {
    }

    public MensajeRepository(ISession session, ILogger<MensajeRepository>? logger = null)
        : base(session, logger)
    {
    }

    public MensajeRepository(ISessionFactory sessionFactory, ILogger<MensajeRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Mensaje>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Mensaje>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Mensaje?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Mensaje>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Mensaje entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Mensaje entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}