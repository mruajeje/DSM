using CarGoGen.Domain.Entities;

namespace CarGoGen.Application.Abstractions;

public interface IViajeRepository : IRepository<Viaje>
{
    Task<IReadOnlyList<Viaje>> ReadFilterByOrigenDestinoAsync(string p_origen, string p_destino);
    Task<IReadOnlyList<Viaje>> ReadFilterByFechaYPlazasAsync(DateTime p_desde, DateTime p_hasta, int p_plazasMinimas);
    Task<IReadOnlyList<Viaje>> ReadFilterByPrecioAsync(double p_precioMinimo, double p_precioMaximo);
    Task<IReadOnlyList<Viaje>> ReadFilterByPreferenciasYEstadoAsync(bool p_permiteMascotas, bool p_permiteEquipaje);
}
