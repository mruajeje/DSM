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
        /*PROTECTED REGION ID(test:custom:Usuario:verificarDni:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "verificarDni";
        var operationType = "Custom";
        var executeManualInvocation = false;
        UsuarioService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Usuario:verificarDni:invoke) ENABLED START*/
        await ((dynamic)service!).VerificarDniAsync(default(int), "Sample Usuario Dni");
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Usuario:verificarDni:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Viaje_GestionarPlazas_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Viaje:gestionarPlazas:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "gestionarPlazas";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Viaje:gestionarPlazas:invoke) ENABLED START*/
        await ((dynamic)service!).GestionarPlazasAsync(default(int), 1);
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Viaje:gestionarPlazas:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Viaje_GestionarAltaPasajero_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Viaje:gestionarAltaPasajero:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "gestionarAltaPasajero";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Viaje:gestionarAltaPasajero:invoke) ENABLED START*/
        await ((dynamic)service!).GestionarAltaPasajeroAsync(default(int), default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Viaje:gestionarAltaPasajero:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Viaje_GeolocalizarTrayecto_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Viaje:geolocalizarTrayecto:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "geolocalizarTrayecto";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Viaje:geolocalizarTrayecto:invoke) ENABLED START*/
        await ((dynamic)service!).GeolocalizarTrayectoAsync(default(int), default(int), 1.5, 1.5, new DateTime(2026, 1, 1, 9, 0, 0));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Viaje:geolocalizarTrayecto:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Viaje_GestionarBajaPasajero_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Viaje:gestionarBajaPasajero:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "gestionarBajaPasajero";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ViajeService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Viaje:gestionarBajaPasajero:invoke) ENABLED START*/
        await ((dynamic)service!).GestionarBajaPasajeroAsync(default(int), default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Viaje:gestionarBajaPasajero:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Pago_RealizarPago_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Pago:realizarPago:arrange) ENABLED START*/
        // Create a test unit of work and assign useCase before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "realizarPago";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        PagoUseCase? useCase = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:customTransaction:Pago:realizarPago:invoke) ENABLED START*/
        await ((dynamic)useCase!).RealizarPagoAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:customTransaction:Pago:realizarPago:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Pago_ReembolsarPago_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Pago:reembolsarPago:arrange) ENABLED START*/
        // Create a test unit of work and assign useCase before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "reembolsarPago";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        PagoUseCase? useCase = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:customTransaction:Pago:reembolsarPago:invoke) ENABLED START*/
        await ((dynamic)useCase!).ReembolsarPagoAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:customTransaction:Pago:reembolsarPago:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Resena_ModerarContenido_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Resena:moderarContenido:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "moderarContenido";
        var operationType = "Custom";
        var executeManualInvocation = false;
        ResenaService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Resena:moderarContenido:invoke) ENABLED START*/
        await ((dynamic)service!).ModerarContenidoAsync(default(int), default(int), EstadoResena.Pendiente, "Sample Resena Motivo");
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Resena:moderarContenido:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Notificacion_MarcarComoLeida_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Notificacion:marcarComoLeida:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "marcarComoLeida";
        var operationType = "Custom";
        var executeManualInvocation = false;
        NotificacionService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Notificacion:marcarComoLeida:invoke) ENABLED START*/
        await ((dynamic)service!).MarcarComoLeidaAsync(default(int), default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Notificacion:marcarComoLeida:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
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
