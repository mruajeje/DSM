using CarGoGen.Application.Abstractions;
using System.Collections.Concurrent;

namespace CarGoGen.Infrastructure.Services;

public sealed class PasarelaPagoSimulada : IPasarelaPago
{
    private readonly ConcurrentDictionary<string, bool> _transacciones = new();

    public Task<string> RealizarPagoAsync(int pagoId, double cantidad)
    {
        if (pagoId <= 0)
            throw new ArgumentException("El identificador del pago no es válido.");

        if (cantidad <= 0)
            throw new ArgumentException("El importe debe ser positivo.");

        string idTransaccion = $"SIM-{Guid.NewGuid():N}";

        _transacciones.TryAdd(idTransaccion, true);

        return Task.FromResult(idTransaccion);
    }

    public Task ReembolsarPagoAsync(string idTransaccion)
    {
        if (string.IsNullOrWhiteSpace(idTransaccion))
            throw new ArgumentException("El identificador de transacción no es válido.");

        if (!_transacciones.TryGetValue(idTransaccion, out bool reembolsable) || !reembolsable)
            throw new InvalidOperationException("La transacción no existe o ya ha sido reembolsada.");

        _transacciones[idTransaccion] = false;

        return Task.CompletedTask;
    }
}