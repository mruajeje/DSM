using CarGoGen.Domain.Entities;

namespace CarGoGen.Application.Abstractions;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<IReadOnlyList<Usuario>> ReadFilterByEmailAsync(string p_email);
    Task<IReadOnlyList<Usuario>> ReadFilterByDniAsync(string p_dni);
}
