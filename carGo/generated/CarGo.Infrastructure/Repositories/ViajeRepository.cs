using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using CarGo.Application.Abstractions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
using CarGo.Infrastructure.Exceptions;

namespace CarGo.Infrastructure.Repositories;

public sealed class ViajeRepository : RepositoryBase<Viaje>, IViajeRepository
{
    public ViajeRepository(ILogger<ViajeRepository>? logger = null)
        : base(logger)
    {
    }

    public ViajeRepository(ISession session, ILogger<ViajeRepository>? logger = null)
        : base(session, logger)
    {
    }

    public ViajeRepository(ISessionFactory sessionFactory, ILogger<ViajeRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Viaje>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Viaje>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Viaje?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Viaje>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Viaje entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Viaje entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }

    public async Task<IReadOnlyList<Viaje>> ReadFilterByOrigenDestinoAsync(string p_origen, string p_destino)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Viaje>()
                .Where(entity => (string.IsNullOrWhiteSpace(p_origen) || entity.Origen.Ciudad.ToLower() == p_origen.Trim().ToLower()) && (string.IsNullOrWhiteSpace(p_destino) || entity.Destino.Ciudad.ToLower() == p_destino.Trim().ToLower()))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Viaje.readFilterByOrigenDestino.");
            throw new DataException("Error executing ReadFilter Viaje.readFilterByOrigenDestino.", ex);
        }
    }

    public async Task<IReadOnlyList<Viaje>> ReadFilterByFechaYPlazasAsync(DateTime p_desde, DateTime p_hasta, int p_plazasMinimas)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Viaje>()
                .Where(entity => entity.FechaHoraSalida >= p_desde && entity.FechaHoraSalida < p_hasta && entity.NumPlazasDisponibles >= p_plazasMinimas)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Viaje.readFilterByFechaYPlazas.");
            throw new DataException("Error executing ReadFilter Viaje.readFilterByFechaYPlazas.", ex);
        }
    }

    public async Task<IReadOnlyList<Viaje>> ReadFilterByPrecioAsync(double p_precioMinimo, double p_precioMaximo)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Viaje>()
                .Where(entity => entity.Precio.Cantidad >= p_precioMinimo && entity.Precio.Cantidad <= p_precioMaximo)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Viaje.readFilterByPrecio.");
            throw new DataException("Error executing ReadFilter Viaje.readFilterByPrecio.", ex);
        }
    }

    public async Task<IReadOnlyList<Viaje>> ReadFilterByPreferenciasYEstadoAsync(bool p_permiteMascotas, bool p_permiteEquipaje)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Viaje>()
                .Where(entity => (!p_permiteMascotas || entity.PermiteMascotas) && (!p_permiteEquipaje || entity.PermiteEquipaje) && (entity.Estado == EstadoViaje.Publicado || entity.Estado == EstadoViaje.EnCurso))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Viaje.readFilterByPreferenciasYEstado.");
            throw new DataException("Error executing ReadFilter Viaje.readFilterByPreferenciasYEstado.", ex);
        }
    }

    public async Task<IReadOnlyList<Viaje>> ReadFilterViajesDisponiblesAsync(DateTime p_fechaMinima)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Viaje>()
                .Where(entity =>entity.Estado == EstadoViaje.Publicado && entity.NumPlazasDisponibles > 0 && entity.FechaHoraSalida >= p_fechaMinima)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Viaje.readFilterViajesDisponibles.");
            throw new DataException("Error executing ReadFilter Viaje.readFilterViajesDisponibles.", ex);
        }
    }

    public async Task<IReadOnlyList<Viaje>> ReadFilterByConductorVerificadoAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Viaje>()
                .Where(entity => entity.Conductor.EstadoVerificacion == EstadoVerificacion.Verificado)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Viaje.readFilterByConductorVerificado.");
            throw new DataException("Error executing ReadFilter Viaje.readFilterByConductorVerificado.", ex);
        }
    }
}