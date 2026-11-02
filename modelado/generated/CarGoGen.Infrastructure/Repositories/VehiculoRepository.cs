using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using CarGoGen.Application.Abstractions;
using CarGoGen.Domain.Entities;

using CarGoGen.Infrastructure.Exceptions;

namespace CarGoGen.Infrastructure.Repositories;

public sealed class VehiculoRepository : RepositoryBase<Vehiculo>, IVehiculoRepository
{
    public VehiculoRepository(ILogger<VehiculoRepository>? logger = null)
        : base(logger)
    {
    }

    public VehiculoRepository(ISession session, ILogger<VehiculoRepository>? logger = null)
        : base(session, logger)
    {
    }

    public VehiculoRepository(ISessionFactory sessionFactory, ILogger<VehiculoRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Vehiculo>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Vehiculo>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Vehiculo?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Vehiculo>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Vehiculo entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Vehiculo entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}