using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using CarGoGen.Application.Abstractions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;
using CarGoGen.Domain.ValueObjects;
using CarGoGen.Infrastructure.Exceptions;

namespace CarGoGen.Infrastructure.Repositories;

public sealed class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ILogger<UsuarioRepository>? logger = null)
        : base(logger)
    {
    }

    public UsuarioRepository(ISession session, ILogger<UsuarioRepository>? logger = null)
        : base(session, logger)
    {
    }

    public UsuarioRepository(ISessionFactory sessionFactory, ILogger<UsuarioRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Usuario>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Usuario>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Usuario?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Usuario>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Usuario entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Usuario entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }

    public async Task<IReadOnlyList<Usuario>> ReadFilterByEmailAsync(string p_email)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Usuario>()
                .Where(entity => entity.Email.ToLower() == p_email.Trim().ToLower())
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Usuario.readFilterByEmail.");
            throw new DataException("Error executing ReadFilter Usuario.readFilterByEmail.", ex);
        }
    }

    public async Task<IReadOnlyList<Usuario>> ReadFilterByDniAsync(string p_dni)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Usuario>()
                .Where(entity => entity.Dni != null && entity.Dni.ToUpper() == p_dni.Trim().ToUpper())
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Usuario.readFilterByDni.");
            throw new DataException("Error executing ReadFilter Usuario.readFilterByDni.", ex);
        }
    }
}