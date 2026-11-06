using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using CarGoGen.Application.Abstractions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;
using CarGoGen.Domain.ValueObjects;
using CarGoGen.Infrastructure.Exceptions;

namespace CarGoGen.Infrastructure.Repositories;

public sealed class ReservaRepository : RepositoryBase<Reserva>, IReservaRepository
{
    public ReservaRepository(ILogger<ReservaRepository>? logger = null)
        : base(logger)
    {
    }

    public ReservaRepository(ISession session, ILogger<ReservaRepository>? logger = null)
        : base(session, logger)
    {
    }

    public ReservaRepository(ISessionFactory sessionFactory, ILogger<ReservaRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Reserva>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Reserva>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Reserva?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Reserva>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Reserva entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Reserva entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }

    public async Task<IReadOnlyList<Reserva>> ReadFilterByEstadoYFechaAsync(EstadoReserva p_estado, DateTime p_desde, DateTime p_hasta)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Reserva>()
                .Where(entity => entity.Estado == p_estado && entity.FechaSolicitud >= p_desde && entity.FechaSolicitud < p_hasta)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Reserva.readFilterByEstadoYFecha.");
            throw new DataException("Error executing ReadFilter Reserva.readFilterByEstadoYFecha.", ex);
        }
    }
}