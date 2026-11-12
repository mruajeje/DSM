using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Application.UseCases.Pago;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
using CarGo.Infrastructure.Services;

namespace CarGo.Tests.Manual;

public sealed class PagoUseCaseTests
{
    // TEST 1: Realizar un pago correctamente
    [Fact]
    public async Task RealizarPago_PagoPendiente_CompletaElPago()
    {
        var pago = CrearPago(1, EstadoPago.Pendiente);
        var contexto = CrearContexto(pago);

        await contexto.UseCase.RealizarPagoAsync(pago.Id);

        Assert.Equal(EstadoPago.Completado, pago.Estado);
        Assert.False(string.IsNullOrWhiteSpace(pago.IdTransaccionPasarela));
        Assert.StartsWith("SIM-", pago.IdTransaccionPasarela!);

        Assert.Equal(1, contexto.Repository.SaveCount);
        Assert.Equal(1, contexto.UnitOfWork.CommitCount);
        Assert.Equal(0, contexto.UnitOfWork.RollbackCount);
    }


    // TEST 2: No realizar un pago inexistente
    [Fact]
    public async Task RealizarPago_PagoInexistente_LanzaError()
    {
        var contexto = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => contexto.UseCase.RealizarPagoAsync(999));

        Assert.Equal(0, contexto.UnitOfWork.CommitCount);
        Assert.Equal(1, contexto.UnitOfWork.RollbackCount);
    }

    // TEST 3: No pagar un pago ya completado
    [Fact]
    public async Task RealizarPago_PagoYaCompletado_LanzaError()
    {
        var pago = CrearPago(2, EstadoPago.Completado);
        var contexto = CrearContexto(pago);

        await Assert.ThrowsAsync<ModelException>(
            () => contexto.UseCase.RealizarPagoAsync(pago.Id));

        Assert.Equal(EstadoPago.Completado, pago.Estado);
        Assert.Equal(0, contexto.Repository.SaveCount);
        Assert.Equal(0, contexto.UnitOfWork.CommitCount);
        Assert.Equal(1, contexto.UnitOfWork.RollbackCount);
    }

    // TEST 4: Reembolsar un pago correctamente
    [Fact]
    public async Task ReembolsarPago_PagoCompletado_ReembolsaElPago()
    {
        var pasarela = new PasarelaPagoSimulada();
        var pago = CrearPago(3, EstadoPago.Completado);

        // Creamos una transacción existente en la pasarela.
        pago.IdTransaccionPasarela =
            await pasarela.RealizarPagoAsync(
                pago.Id,
                pago.Monto.Cantidad);

        var contexto = CrearContexto(pasarela, pago);

        await contexto.UseCase.ReembolsarPagoAsync(pago.Id);

        Assert.Equal(EstadoPago.Reembolsado, pago.Estado);
        Assert.NotNull(pago.IdTransaccionPasarela);

        Assert.Equal(1, contexto.Repository.SaveCount);
        Assert.Equal(1, contexto.UnitOfWork.CommitCount);
        Assert.Equal(0, contexto.UnitOfWork.RollbackCount);
    }

    // TEST 5: No reembolsar un pago pendiente
    [Fact]
    public async Task ReembolsarPago_PagoPendiente_LanzaError()
    {
        var pago = CrearPago(4, EstadoPago.Pendiente);
        var contexto = CrearContexto(pago);

        await Assert.ThrowsAsync<ModelException>(
            () => contexto.UseCase.ReembolsarPagoAsync(pago.Id));

        Assert.Equal(EstadoPago.Pendiente, pago.Estado);
        Assert.Equal(0, contexto.Repository.SaveCount);
        Assert.Equal(0, contexto.UnitOfWork.CommitCount);
        Assert.Equal(1, contexto.UnitOfWork.RollbackCount);
    }


    // TEST 6: No reembolsar sin transacción
    [Fact]
    public async Task ReembolsarPago_SinTransaccion_LanzaError()
    {
        var pago = CrearPago(5, EstadoPago.Completado);
        var contexto = CrearContexto(pago);

        await Assert.ThrowsAsync<ModelException>(
            () => contexto.UseCase.ReembolsarPagoAsync(pago.Id));

        Assert.Equal(EstadoPago.Completado, pago.Estado);
        Assert.Equal(0, contexto.Repository.SaveCount);
        Assert.Equal(1, contexto.UnitOfWork.RollbackCount);
    }


    // TEST 7: No reembolsar dos veces
    [Fact]
    public async Task ReembolsarPago_DosVeces_LanzaError()
    {
        var pasarela = new PasarelaPagoSimulada();
        var pago = CrearPago(6, EstadoPago.Completado);

        pago.IdTransaccionPasarela =
            await pasarela.RealizarPagoAsync(
                pago.Id,
                pago.Monto.Cantidad);

        var contexto = CrearContexto(pasarela, pago);

        // Primer reembolso: correcto.
        await contexto.UseCase.ReembolsarPagoAsync(pago.Id);

        Assert.Equal(EstadoPago.Reembolsado, pago.Estado);

        // Segundo reembolso: debe fallar.
        await Assert.ThrowsAsync<ModelException>(
            () => contexto.UseCase.ReembolsarPagoAsync(pago.Id));

        Assert.Equal(EstadoPago.Reembolsado, pago.Estado);
        Assert.Equal(1, contexto.UnitOfWork.CommitCount);
        Assert.Equal(1, contexto.UnitOfWork.RollbackCount);
    }


    // TEST 8: Rollback cuando falla la pasarela
    [Fact]
    public async Task RealizarPago_FallaLaPasarela_EjecutaRollback()
    {
        var pago = CrearPago(7, EstadoPago.Pendiente);

        // Creamos una pasarela que simula un error.
        var pasarela = new PasarelaQueFalla();

        var contexto = CrearContexto(pasarela, pago);

        await Assert.ThrowsAsync<ModelException>(
            () => contexto.UseCase.RealizarPagoAsync(pago.Id));

        // El pago debe continuar pendiente.
        Assert.Equal(EstadoPago.Pendiente, pago.Estado);

        // No debe haberse guardado ni confirmado.
        Assert.Equal(0, contexto.Repository.SaveCount);
        Assert.Equal(0, contexto.UnitOfWork.CommitCount);

        // Debe haberse ejecutado el rollback.
        Assert.Equal(1, contexto.UnitOfWork.RollbackCount);
    }


    // TEST 9: Comprobar el importe enviado
    [Fact]
    public async Task RealizarPago_EnviaElImporteCorrectoALaPasarela()
    {
        var pago = CrearPago(8, EstadoPago.Pendiente);

        var pasarela = new PasarelaCapturadora();

        var contexto = CrearContexto(pasarela, pago);

        await contexto.UseCase.RealizarPagoAsync(pago.Id);

        // Comprobar el identificador del pago enviado.
        Assert.Equal(pago.Id, pasarela.PagoIdRecibido);

        // Comprobar el importe enviado.
        Assert.Equal(20.00, pasarela.CantidadRecibida);

        // Comprobar que el pago se ha completado.
        Assert.Equal(EstadoPago.Completado, pago.Estado);

        // Comprobar que se ha guardado la transacción devuelta.
        Assert.Equal("SIM-PRUEBA-123", pago.IdTransaccionPasarela);
    }

    // MÉTODOS AUXILIARES
    private static Pago CrearPago(int id, EstadoPago estado)
    {
        return new Pago
        {
            Id = id,
            Monto = new Dinero
            {
                Moneda = EnumMoneda.EUR,
                Cantidad = 20.00
            },
            FechaPago = DateTime.UtcNow,
            Estado = estado,
            Reserva = null!
        };
    }

    private static ContextoPrueba CrearContexto(params Pago[] pagos)
    {
        return CrearContexto(new PasarelaPagoSimulada(), pagos);
    }

    private static ContextoPrueba CrearContexto(
        IPasarelaPago pasarela,
        params Pago[] pagos)
    {
        var repository = new FakePagoRepository(pagos);
        var unitOfWork = new FakeUnitOfWork(repository);
        var utils = new FakeUnitOfWorkUtils(pasarela);

        var useCase = new PagoUseCase(unitOfWork, utils);

        return new ContextoPrueba(
            useCase,
            repository,
            unitOfWork);
    }

    private sealed class ContextoPrueba
    {
        public PagoUseCase UseCase { get; }
        public FakePagoRepository Repository { get; }
        public FakeUnitOfWork UnitOfWork { get; }

        public ContextoPrueba(
            PagoUseCase useCase,
            FakePagoRepository repository,
            FakeUnitOfWork unitOfWork)
        {
            UseCase = useCase;
            Repository = repository;
            UnitOfWork = unitOfWork;
        }
    }

    // REPOSITORIO SIMULADO
    private sealed class FakePagoRepository : IPagoRepository
    {
        private readonly Dictionary<int, Pago> _pagos = new();

        public int SaveCount { get; private set; }

        public FakePagoRepository(params Pago[] pagos)
        {
            foreach (var pago in pagos)
                _pagos[pago.Id] = pago;
        }

        public Task<IReadOnlyList<Pago>> ReadAllAsync()
        {
            IReadOnlyList<Pago> resultado = _pagos.Values.ToList();
            return Task.FromResult(resultado);
        }

        public Task<Pago?> ReadAsync(object id)
        {
            Pago? resultado = null;

            if (id is int pagoId)
                _pagos.TryGetValue(pagoId, out resultado);

            return Task.FromResult(resultado);
        }

        public Task SaveAsync(Pago entity)
        {
            _pagos[entity.Id] = entity;
            SaveCount++;

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Pago entity)
        {
            _pagos.Remove(entity.Id);
            return Task.CompletedTask;
        }
    }

    // UNIT OF WORK SIMULADO
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public IPagoRepository PagoRepository { get; }

        public int CommitCount { get; private set; }
        public int RollbackCount { get; private set; }

        public FakeUnitOfWork(IPagoRepository pagoRepository)
        {
            PagoRepository = pagoRepository;
        }

        public IUsuarioRepository UsuarioRepository => null!;
        public IVehiculoRepository VehiculoRepository => null!;
        public IViajeRepository ViajeRepository => null!;
        public IReservaRepository ReservaRepository => null!;
        public IMensajeRepository MensajeRepository => null!;
        public IResenaRepository ResenaRepository => null!;
        public INotificacionRepository NotificacionRepository => null!;

        public IHariaPasswordHasher PasswordHasher => null!;
        public IHariaTokenGenerator TokenGenerator => null!;

        public Task BeginTransactionAsync(string? businessOperation = null)
        {
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            CommitCount++;
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            RollbackCount++;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    // UNIT OF WORK UTILS SIMULADO
    private sealed class FakeUnitOfWorkUtils : IUnitOfWorkUtils
    {
        private readonly IPasarelaPago _pasarela;

        public IServiceProvider Services { get; }

        public FakeUnitOfWorkUtils(IPasarelaPago pasarela)
        {
            _pasarela = pasarela;
            Services = new FakeServiceProvider(pasarela);
        }

        public T GetRequiredService<T>() where T : notnull
        {
            if (_pasarela is T servicio)
                return servicio;

            throw new InvalidOperationException(
                $"El servicio {typeof(T).Name} no está registrado.");
        }

        public T? GetService<T>()
        {
            return _pasarela is T servicio ? servicio : default;
        }
    }

    private sealed class FakeServiceProvider : IServiceProvider
    {
        private readonly IPasarelaPago _pasarela;

        public FakeServiceProvider(IPasarelaPago pasarela)
        {
            _pasarela = pasarela;
        }

        public object? GetService(Type serviceType)
        {
            return serviceType == typeof(IPasarelaPago)
                ? _pasarela
                : null;
        }
    }


    // PASARELA QUE SIMULA UN ERROR
    private sealed class PasarelaQueFalla : IPasarelaPago
    {
        public Task<string> RealizarPagoAsync(int pagoId, double cantidad)
        {
            return Task.FromException<string>(
                new InvalidOperationException(
                    "Error simulado al realizar el pago."));
        }

        public Task ReembolsarPagoAsync(string idTransaccion)
        {
            return Task.CompletedTask;
        }
    }


    // PASARELA QUE CAPTURA LOS DATOS RECIBIDOS
    private sealed class PasarelaCapturadora : IPasarelaPago
    {
        public int? PagoIdRecibido { get; private set; }

        public double? CantidadRecibida { get; private set; }

        public Task<string> RealizarPagoAsync(int pagoId, double cantidad)
        {
            PagoIdRecibido = pagoId;
            CantidadRecibida = cantidad;

            return Task.FromResult("SIM-PRUEBA-123");
        }

        public Task ReembolsarPagoAsync(string idTransaccion)
        {
            return Task.CompletedTask;
        }
    }
}