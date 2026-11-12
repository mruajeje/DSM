using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

namespace CarGo.Application.Abstractions;

public interface IReservaRepository : IRepository<Reserva>
{
    Task<IReadOnlyList<Reserva>> ReadFilterByEstadoYFechaAsync(EstadoReserva p_estado, DateTime p_desde, DateTime p_hasta);
}
