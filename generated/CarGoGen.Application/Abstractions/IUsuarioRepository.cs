using CarGoGen.Domain.Entities;

namespace CarGoGen.Application.Abstractions;

public interface IUsuarioRepository : IRepository<Usuario>
{
    
    Task<Usuario?> ReadByEmailAsync(string email);
    Task<Usuario?> ReadByDniAsync(string dni);
}
