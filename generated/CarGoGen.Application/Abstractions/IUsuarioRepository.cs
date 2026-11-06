using CarGoGen.Domain.Entities;

namespace CarGoGen.Application.Abstractions;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ReadByDniAsync(string p_dni);
    Task<Usuario?> ReadByEmailAsync(string p_email);
}
