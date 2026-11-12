using CarGo.Application.Services;
using CarGo.Application.UseCases.Pago;
using CarGo.Domain.Entities;
using Xunit.Abstractions;
using CarGo.Domain.ValueObjects;
using CarGo.Domain.Enums;

namespace CarGo.Tests.Manual;

public sealed class ManualOperationTests
{
    private readonly ITestOutputHelper _output;

    public ManualOperationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Usuario_VerificarDni_Custom_Manual_Contract()
    {
        // <haria-protected id="test:custom:Usuario:verificarDni:arrange">
        // Create a test unit of work and assign service before enabling the generated invocation.
        // </haria-protected>

        var operationName = "verificarDni";
        var operationType = "Custom";
        var executeManualInvocation = false;
        UsuarioService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:custom:Usuario:verificarDni:invoke">
        await ((dynamic)service!).VerificarDniAsync(default(int), "Sample Usuario Dni");
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:custom:Usuario:verificarDni:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Viaje_GestionarPlazas_Custom_Manual_Contract()
    {
        // <haria-protected id="test:custom:Viaje:gestionarPlazas:arrange">
        // Create a test unit of work and assign service before enabling the generated invocation.
        // </haria-protected>

        var operationName = "gestionarPlazas";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:custom:Viaje:gestionarPlazas:invoke">
        await ((dynamic)service!).GestionarPlazasAsync(default(int), 1);
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:custom:Viaje:gestionarPlazas:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Viaje_GestionarAltaPasajero_Custom_Manual_Contract()
    {
        // <haria-protected id="test:custom:Viaje:gestionarAltaPasajero:arrange">
        // Create a test unit of work and assign service before enabling the generated invocation.
        // </haria-protected>

        var operationName = "gestionarAltaPasajero";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:custom:Viaje:gestionarAltaPasajero:invoke">
        await ((dynamic)service!).GestionarAltaPasajeroAsync(default(int), default(int));
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:custom:Viaje:gestionarAltaPasajero:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Viaje_GeolocalizarTrayecto_Custom_Manual_Contract()
    {
        // <haria-protected id="test:custom:Viaje:geolocalizarTrayecto:arrange">
        // Create a test unit of work and assign service before enabling the generated invocation.
        // </haria-protected>

        var operationName = "geolocalizarTrayecto";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:custom:Viaje:geolocalizarTrayecto:invoke">
        await ((dynamic)service!).GeolocalizarTrayectoAsync(default(int), default(int), 1.5, 1.5, new DateTime(2026, 1, 1, 9, 0, 0));
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:custom:Viaje:geolocalizarTrayecto:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Viaje_GestionarBajaPasajero_Custom_Manual_Contract()
    {
        // <haria-protected id="test:custom:Viaje:gestionarBajaPasajero:arrange">
        // Create a test unit of work and assign service before enabling the generated invocation.
        // </haria-protected>

        var operationName = "gestionarBajaPasajero";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:custom:Viaje:gestionarBajaPasajero:invoke">
        await ((dynamic)service!).GestionarBajaPasajeroAsync(default(int), default(int));
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:custom:Viaje:gestionarBajaPasajero:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Pago_RealizarPago_CustomTransaction_Manual_Contract()
    {
        // <haria-protected id="test:customTransaction:Pago:realizarPago:arrange">
        // Create a test unit of work and assign useCase before enabling the generated invocation.
        // </haria-protected>

        var operationName = "realizarPago";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        PagoUseCase? useCase = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:customTransaction:Pago:realizarPago:invoke">
        await ((dynamic)useCase!).RealizarPagoAsync(default(int));
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:customTransaction:Pago:realizarPago:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Pago_ReembolsarPago_CustomTransaction_Manual_Contract()
    {
        // <haria-protected id="test:customTransaction:Pago:reembolsarPago:arrange">
        // Create a test unit of work and assign useCase before enabling the generated invocation.
        // </haria-protected>

        var operationName = "reembolsarPago";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        PagoUseCase? useCase = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:customTransaction:Pago:reembolsarPago:invoke">
        await ((dynamic)useCase!).ReembolsarPagoAsync(default(int));
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:customTransaction:Pago:reembolsarPago:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Resena_ModerarContenido_Custom_Manual_Contract()
    {
        // <haria-protected id="test:custom:Resena:moderarContenido:arrange">
        // Create a test unit of work and assign service before enabling the generated invocation.
        // </haria-protected>

        var operationName = "moderarContenido";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ResenaService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:custom:Resena:moderarContenido:invoke">
        await ((dynamic)service!).ModerarContenidoAsync(default(int), default(int), EstadoResena.Pendiente, "Sample Resena Motivo");
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:custom:Resena:moderarContenido:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    [Fact]
    public async Task Notificacion_MarcarComoLeida_Custom_Manual_Contract()
    {
        // <haria-protected id="test:custom:Notificacion:marcarComoLeida:arrange">
        // Create a test unit of work and assign service before enabling the generated invocation.
        // </haria-protected>

        var operationName = "marcarComoLeida";
        var operationType = "Custom";
        var executeManualInvocation = false;
        NotificacionService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        // <haria-protected id="test:custom:Notificacion:marcarComoLeida:invoke">
        await ((dynamic)service!).MarcarComoLeidaAsync(default(int), default(int));
        // </haria-protected>
        }
        _ = invocationResult;

        // <haria-protected id="test:custom:Notificacion:marcarComoLeida:assert">
        EnsureContractOutput(operationName, operationType);
        // </haria-protected>
    }

    private const string SuccessColor = "\u001b[32m";
    private const string ErrorColor = "\u001b[31m";
    private const string ResetColor = "\u001b[0m";

    private void EnsureContractOutput(string operationName, string operationType)
    {
        Ensure(!string.IsNullOrWhiteSpace(operationName), $"{operationName} declares the manual operation name.");
        Ensure(!string.IsNullOrWhiteSpace(operationType), $"{operationName} declares the manual operation type.");
        WriteTestMessage(true, $"{operationName} contract uses operation type {operationType}.");
    }

    private void Ensure(bool condition, string message)
    {
        WriteTestMessage(condition, message);
        Assert.True(condition, message);
    }

    private void WriteTestMessage(bool condition, string message)
    {
        var color = condition ? SuccessColor : ErrorColor;
        var status = condition ? "[OK] " : "[FAIL] ";
        _output.WriteLine($"{color}{status}{message}{ResetColor}");
    }

}
