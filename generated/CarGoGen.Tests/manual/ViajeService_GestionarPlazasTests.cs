
using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Application.Services;
using CarGoGen.Domain.Entities;

namespace CarGoGen.Tests.Services;

public sealed class ViajeService_GestionarPlazasTests
{
    private const int ViajeId = 10;

    // TEST 1: Cambiar la capacidad correctamente
    [Fact]
    public async Task GestionarPlazas_CapacidadValida_ActualizaPlazasDisponibles()
    {
        // El viaje tiene 2 pasajeros y queremos una capacidad total de 5.
        var ctx = CrearContexto(numeroPasajeros: 2);

        await ctx.Service.GestionarPlazasAsync(ViajeId, 5);

        // 5 plazas de capacidad - 2 pasajeros = 3 plazas disponibles.
        Assert.Equal(3, ctx.Viaje.NumPlazasDisponibles);
        Assert.Equal(1, ctx.Repo.SaveCount);
    }

    // TEST 2: No se permiten capacidades negativas
    [Fact]
    public async Task GestionarPlazas_CapacidadNegativa_LanzaError()
    {
        var ctx = CrearContexto(numeroPasajeros: 0);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarPlazasAsync(ViajeId, -1));

        Assert.Equal(4, ctx.Viaje.NumPlazasDisponibles);
        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // TEST 3: No se puede reducir la capacidad por debajo
    // del número de pasajeros que ya están apuntados.
    [Fact]
    public async Task GestionarPlazas_CapacidadMenorQuePasajeros_LanzaError()
    {
        var ctx = CrearContexto(numeroPasajeros: 3);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarPlazasAsync(ViajeId, 2));

        Assert.Equal(4, ctx.Viaje.NumPlazasDisponibles);
        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // TEST 4: La capacidad puede coincidir con los pasajeros actuales.
    // En ese caso quedan cero plazas disponibles.
    [Fact]
    public async Task GestionarPlazas_CapacidadIgualAPasajeros_DejaCeroPlazas()
    {
        var ctx = CrearContexto(numeroPasajeros: 3);

        await ctx.Service.GestionarPlazasAsync(ViajeId, 3);

        Assert.Equal(0, ctx.Viaje.NumPlazasDisponibles);
        Assert.Equal(1, ctx.Repo.SaveCount);
    }

    // TEST 5: El viaje debe existir
    [Fact]
    public async Task GestionarPlazas_ViajeInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarPlazasAsync(999, 5));

        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // CONTEXTO DE PRUEBAS
    private static Contexto CrearContexto(int numeroPasajeros = 0)
    {
        var pasajeros = Enumerable.Range(1, numeroPasajeros)
            .Select(id => new Usuario
            {
                Id = id,
                NombreCompleto = $"Pasajero {id}"
            })
            .ToList();

        var viaje = new Viaje
        {
            Id = ViajeId,
            NumPlazasDisponibles = 4,
            Pasajero = pasajeros
        };

        var repo = new FakeViajeRepo(viaje);
        var uow = new FakeUnitOfWork(repo);

        return new Contexto(new ViajeService(uow), viaje, repo);
    }

    private sealed record Contexto(
        ViajeService Service,
        Viaje Viaje,
        FakeViajeRepo Repo);

    // REPOSITORIO FALSO DE VIAJES
    private sealed class FakeViajeRepo : IViajeRepository
    {
        private readonly Dictionary<int, Viaje> _items = new();

        public int SaveCount { get; private set; }

        public FakeViajeRepo(params Viaje[] viajes)
        {
            foreach (var viaje in viajes)
                _items[viaje.Id] = viaje;
        }

        public Task<IReadOnlyList<Viaje>> ReadAllAsync() =>
            Task.FromResult<IReadOnlyList<Viaje>>(_items.Values.ToList());

        public Task<Viaje?> ReadAsync(object id)
        {
            Viaje? viaje = null;

            if (id is int key)
                _items.TryGetValue(key, out viaje);

            return Task.FromResult(viaje);
        }

        public Task SaveAsync(Viaje entity)
        {
            _items[entity.Id] = entity;
            SaveCount++;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Viaje entity)
        {
            _items.Remove(entity.Id);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Viaje>> ReadFilterByOrigenDestinoAsync(
            string p_origen, string p_destino) =>
            Task.FromResult<IReadOnlyList<Viaje>>(Array.Empty<Viaje>());

        public Task<IReadOnlyList<Viaje>> ReadFilterByFechaYPlazasAsync(
            DateTime p_desde, DateTime p_hasta, int p_plazasMinimas) =>
            Task.FromResult<IReadOnlyList<Viaje>>(Array.Empty<Viaje>());

        public Task<IReadOnlyList<Viaje>> ReadFilterByPrecioAsync(
            double p_precioMinimo, double p_precioMaximo) =>
            Task.FromResult<IReadOnlyList<Viaje>>(Array.Empty<Viaje>());

        public Task<IReadOnlyList<Viaje>> ReadFilterByPreferenciasYEstadoAsync(
            bool p_permiteMascotas, bool p_permiteEquipaje) =>
            Task.FromResult<IReadOnlyList<Viaje>>(Array.Empty<Viaje>());

        public Task<IReadOnlyList<Viaje>> ReadFilterViajesDisponiblesAsync(
            DateTime p_fechaMinima) =>
            Task.FromResult<IReadOnlyList<Viaje>>(Array.Empty<Viaje>());

        public Task<IReadOnlyList<Viaje>> ReadFilterByConductorVerificadoAsync() =>
            Task.FromResult<IReadOnlyList<Viaje>>(Array.Empty<Viaje>());
    }

    // UNIT OF WORK FALSO
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public IViajeRepository ViajeRepository { get; }

        public FakeUnitOfWork(IViajeRepository viajeRepository)
        {
            ViajeRepository = viajeRepository;
        }

        public IUsuarioRepository UsuarioRepository => null!;
        public IResenaRepository ResenaRepository => null!;
        public INotificacionRepository NotificacionRepository => null!;
        public IVehiculoRepository VehiculoRepository => null!;
        public IReservaRepository ReservaRepository => null!;
        public IPagoRepository PagoRepository => null!;
        public IMensajeRepository MensajeRepository => null!;
        public IHariaPasswordHasher PasswordHasher => null!;
        public IHariaTokenGenerator TokenGenerator => null!;

        public Task BeginTransactionAsync(string? businessOperation = null) =>
            Task.CompletedTask;

        public Task CommitAsync() => Task.CompletedTask;

        public Task RollbackAsync() => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
