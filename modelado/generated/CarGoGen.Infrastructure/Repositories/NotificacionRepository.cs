using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using CarGoGen.Application.Abstractions;
using CarGoGen.Domain.Entities;

using CarGoGen.Infrastructure.Exceptions;

namespace CarGoGen.Infrastructure.Repositories;

public sealed class NotificacionRepository : RepositoryBase<Notificacion>, INotificacionRepository
{
    public NotificacionRepository(ILogger<NotificacionRepository>? logger = null)
        : base(logger)
    {
    }

    public NotificacionRepository(ISession session, ILogger<NotificacionRepository>? logger = null)
        : base(session, logger)
    {
    }

    public NotificacionRepository(ISessionFactory sessionFactory, ILogger<NotificacionRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Notificacion>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Notificacion>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Notificacion?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Notificacion>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Notificacion entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Notificacion entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}