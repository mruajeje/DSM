namespace CarGoGen.Application.Abstractions;

public interface IPasarelaPago
{
    Task<string> RealizarPagoAsync(int pagoId, double cantidad);

    Task ReembolsarPagoAsync(string idTransaccion);
}