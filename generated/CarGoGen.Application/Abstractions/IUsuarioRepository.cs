using CarGoGen.Domain.Entities;

namespace CarGoGen.Application.Abstractions;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ReadByDniAsync(string dni);
    Task<Usuario?> ReadByEmailAsync(string email);
}
