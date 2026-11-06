using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Application.Abstractions;

public interface IReservaRepository : IRepository<Reserva>
{
    Task<IReadOnlyList<Reserva>> ReadFilterByEstadoYFechaAsync(EstadoReserva p_estado, DateTime p_desde, DateTime p_hasta);
}
