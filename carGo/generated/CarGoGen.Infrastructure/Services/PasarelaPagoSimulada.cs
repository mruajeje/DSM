using System.Collections.Concurrent;
using CarGoGen.Application.Abstractions;

namespace CarGoGen.Infrastructure.Services;

public sealed class PasarelaPagoSimulada : IPasarelaPago
{
    // Guarda las transacciones y si siguen siendo reembolsables.
    private readonly ConcurrentDictionary<string, bool> _transacciones = new();

    public Task<string> RealizarPagoAsync(int pagoId, double cantidad)
    {
        if (pagoId <= 0)
            throw new ArgumentException("El identificador del pago no es válido.");

        if (!double.IsFinite(cantidad) || cantidad <= 0)
            throw new ArgumentException("El importe debe ser positivo y válido.");

        // Generamos un identificador único para la transacción.
        string idTransaccion = $"SIM-{Guid.NewGuid():N}";

        _transacciones.TryAdd(idTransaccion, true);

        return Task.FromResult(idTransaccion);
    }

    public Task ReembolsarPagoAsync(string idTransaccion)
    {
        if (string.IsNullOrWhiteSpace(idTransaccion))
            throw new ArgumentException("El identificador de transacción no es válido.");

        if (!_transacciones.TryGetValue(idTransaccion, out bool reembolsable))
            throw new InvalidOperationException("La transacción no existe.");

        if (!reembolsable)
            throw new InvalidOperationException("La transacción ya ha sido reembolsada.");

        // Evita que dos peticiones reembolsen simultáneamente la misma transacción.
        if (!_transacciones.TryUpdate(idTransaccion, false, true))
            throw new InvalidOperationException(
                "La transacción ya ha sido reembolsada.");

        return Task.CompletedTask;
    }
}