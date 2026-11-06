using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Application.Services;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;
using CarGoGen.Domain.ValueObjects;

namespace CarGoGen.Tests.Services;

public sealed class ViajeServiceAceptarPasajeroTests
{
    private const int ConductorId = 10;
    private const int PasajeroId = 20;


    // TEST 1: Aceptar una reserva correctamente
    [Fact]
    public async Task AceptarPasajero_ReservaValida_AceptaYActualizaTodo()
    {
        var ctx = CrearContexto(plazasViaje: 4, plazasReserva: 2);

        await ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, ConductorId);

        Assert.Equal(EstadoReserva.Aceptada, ctx.Reserva.Estado);
        Assert.Equal(2, ctx.Viaje.NumPlazasDisponibles);
        Assert.Contains(ctx.Viaje.Pasajero, u => u.Id == PasajeroId);
        Assert.Contains(ctx.Pasajero.Viajespasajero, v => v.Id == ctx.Viaje.Id);

        var notificacion = Assert.Single(ctx.UnitOfWork.Notificaciones.Items);
        Assert.Equal(PasajeroId, notificacion.UsuarioNotificado.Id);
        Assert.False(string.IsNullOrWhiteSpace(notificacion.Titulo));
        Assert.False(string.IsNullOrWhiteSpace(notificacion.Texto));

        Assert.Equal(1, ctx.UnitOfWork.CommitCount);
        Assert.Equal(0, ctx.UnitOfWork.RollbackCount);
    }


    // TEST 2: Reserva inexistente
    [Fact]
    public async Task AceptarPasajero_ReservaInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.Reserva_aceptarPasajeroAsync(999, ConductorId));

        Assert.Equal(0, ctx.UnitOfWork.CommitCount);
        Assert.Equal(1, ctx.UnitOfWork.RollbackCount);
        Assert.Empty(ctx.UnitOfWork.Notificaciones.Items);
    }


    // TEST 3: Solo se aceptan reservas pendientes
    [Theory]
    [InlineData(EstadoReserva.Aceptada)]
    [InlineData(EstadoReserva.Rechazada)]
    [InlineData(EstadoReserva.Cancelada)]
    public async Task AceptarPasajero_ReservaNoPendiente_LanzaError(EstadoReserva estado)
    {
        var ctx = CrearContexto(estadoReserva: estado);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, ConductorId));

        AssertNadaCambio(ctx, estado, plazasEsperadas: 4);
    }


    // TEST 4: El viaje debe estar publicado
    [Theory]
    [InlineData(EstadoViaje.EnCurso)]
    [InlineData(EstadoViaje.Finalizado)]
    [InlineData(EstadoViaje.Cancelado)]
    public async Task AceptarPasajero_ViajeNoPublicado_LanzaError(EstadoViaje estadoViaje)
    {
        var ctx = CrearContexto(estadoViaje: estadoViaje);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, ConductorId));

        AssertNadaCambio(ctx, EstadoReserva.Pendiente, plazasEsperadas: 4);
    }


    // TEST 5: Quien acepta debe ser el conductor
    [Fact]
    public async Task AceptarPasajero_UsuarioNoEsConductor_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, PasajeroId));

        AssertNadaCambio(ctx, EstadoReserva.Pendiente, plazasEsperadas: 4);
    }


    // TEST 6: Plazas insuficientes
    [Fact]
    public async Task AceptarPasajero_PlazasInsuficientes_LanzaErrorYNoTocaNada()
    {
        var ctx = CrearContexto(plazasViaje: 1, plazasReserva: 2);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, ConductorId));

        AssertNadaCambio(ctx, EstadoReserva.Pendiente, plazasEsperadas: 1);
    }


    // TEST 7: Justo las plazas que quedan
    [Fact]
    public async Task AceptarPasajero_PlazasExactas_DejaElViajeSinPlazas()
    {
        var ctx = CrearContexto(plazasViaje: 2, plazasReserva: 2);

        await ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, ConductorId);

        Assert.Equal(0, ctx.Viaje.NumPlazasDisponibles);
        Assert.Equal(EstadoReserva.Aceptada, ctx.Reserva.Estado);
        Assert.Equal(1, ctx.UnitOfWork.CommitCount);
    }


    // TEST 8: Reserva sin plazas solicitadas
    [Fact]
    public async Task AceptarPasajero_ReservaConCeroPlazas_LanzaError()
    {
        var ctx = CrearContexto(plazasReserva: 0);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, ConductorId));

        AssertNadaCambio(ctx, EstadoReserva.Pendiente, plazasEsperadas: 4);
    }

    // TEST 9: No duplicar al pasajero en el viaje
    [Fact]
    public async Task AceptarPasajero_PasajeroYaEnViaje_NoLoDuplica()
    {
        var ctx = CrearContexto();
        ctx.Viaje.Pasajero.Add(ctx.Pasajero);
        ctx.Pasajero.Viajespasajero.Add(ctx.Viaje);

        await ctx.Service.Reserva_aceptarPasajeroAsync(ctx.Reserva.Id, ConductorId);

        Assert.Single(ctx.Viaje.Pasajero, u => u.Id == PasajeroId);
        Assert.Single(ctx.Pasajero.Viajespasajero, v => v.Id == ctx.Viaje.Id);
    }


    private static void AssertNadaCambio(Contexto ctx, EstadoReserva estadoReserva, int plazasEsperadas)
    {
        Assert.Equal(estadoReserva, ctx.Reserva.Estado);
        Assert.Equal(plazasEsperadas, ctx.Viaje.NumPlazasDisponibles);
        Assert.Empty(ctx.Viaje.Pasajero);
        Assert.Empty(ctx.UnitOfWork.Notificaciones.Items);
        Assert.Equal(0, ctx.UnitOfWork.CommitCount);
        Assert.Equal(1, ctx.UnitOfWork.RollbackCount);
    }

    private static Contexto CrearContexto(
        int plazasViaje = 4,
        int plazasReserva = 1,
        EstadoReserva estadoReserva = EstadoReserva.Pendiente,
        EstadoViaje estadoViaje = EstadoViaje.Publicado)
    {
        var conductor = new Usuario { Id = ConductorId, NombreCompleto = "Conductor Test" };
        var pasajero = new Usuario { Id = PasajeroId, NombreCompleto = "Pasajero Test" };

        var viaje = new Viaje
        {
            Id = 1,
            Origen = new Ubicacion { Ciudad = "Valencia" },
            Destino = new Ubicacion { Ciudad = "Madrid" },
            NumPlazasDisponibles = plazasViaje,
            Estado = estadoViaje,
            Conductor = conductor,
        };

        var reserva = new Reserva
        {
            Id = 100,
            NumPlazasReservadas = plazasReserva,
            Estado = estadoReserva,
            Viaje = viaje,
            Pasajero = pasajero,
        };
        viaje.Reservas.Add(reserva);

        var uow = new FakeUnitOfWork(
            new FakeRepo<Reserva>(r => r.Id, reserva),
            new FakeRepo<Viaje>(v => v.Id, viaje),
            new FakeRepo<Usuario>(u => u.Id, conductor, pasajero),
            new FakeRepo<Notificacion>(n => n.Id));

        return new Contexto(new ViajeService(uow), uow, viaje, reserva, pasajero);
    }

    private sealed record Contexto(
        ViajeService Service,
        FakeUnitOfWork UnitOfWork,
        Viaje Viaje,
        Reserva Reserva,
        Usuario Pasajero);



    private class FakeRepo<T> where T : class
    {
        private readonly Func<T, int> _getId;
        private readonly Dictionary<int, T> _items = new();
        private int _nextId = 1000;

        public FakeRepo(Func<T, int> getId, params T[] items)
        {
            _getId = getId;
            foreach (var item in items)
                _items[getId(item)] = item;
        }

        public IReadOnlyList<T> Items => _items.Values.ToList();

        public Task<IReadOnlyList<T>> ReadAllAsync() => Task.FromResult(Items);

        public Task<T?> ReadAsync(object id)
        {
            T? result = null;
            if (id is int key)
                _items.TryGetValue(key, out result);
            return Task.FromResult(result);
        }

        public Task SaveAsync(T entity)
        {
            var id = _getId(entity);
            if (id == 0)
                id = _nextId++;
            _items[id] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _items.Remove(_getId(entity));
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        private readonly FakeReservaRepo _reservas;
        private readonly FakeViajeRepo _viajes;
        private readonly FakeUsuarioRepo _usuarios;

        public FakeNotificacionRepo Notificaciones { get; }

        public int CommitCount { get; private set; }
        public int RollbackCount { get; private set; }

        public FakeUnitOfWork(
            FakeRepo<Reserva> reservas,
            FakeRepo<Viaje> viajes,
            FakeRepo<Usuario> usuarios,
            FakeRepo<Notificacion> notificaciones)
        {
            _reservas = new FakeReservaRepo(reservas);
            _viajes = new FakeViajeRepo(viajes);
            _usuarios = new FakeUsuarioRepo(usuarios);
            Notificaciones = new FakeNotificacionRepo(notificaciones);
        }

        public IReservaRepository ReservaRepository => _reservas;
        public IViajeRepository ViajeRepository => _viajes;
        public IUsuarioRepository UsuarioRepository => _usuarios;
        public INotificacionRepository NotificacionRepository => Notificaciones;

        public IVehiculoRepository VehiculoRepository => null!;
        public IPagoRepository PagoRepository => null!;
        public IMensajeRepository MensajeRepository => null!;
        public IResenaRepository ResenaRepository => null!;
        public IHariaPasswordHasher PasswordHasher => null!;
        public IHariaTokenGenerator TokenGenerator => null!;

        public Task BeginTransactionAsync(string? businessOperation = null) => Task.CompletedTask;

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

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    // Adaptadores: cada interfaz de repositorio delega en el FakeRepo generico.

    private sealed class FakeReservaRepo : IReservaRepository
    {
        private readonly FakeRepo<Reserva> _inner;
        public FakeReservaRepo(FakeRepo<Reserva> inner) => _inner = inner;
        public Task<IReadOnlyList<Reserva>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Reserva?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Reserva entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Reserva entity) => _inner.DeleteAsync(entity);
    }

    private sealed class FakeViajeRepo : IViajeRepository
    {
        private readonly FakeRepo<Viaje> _inner;
        public FakeViajeRepo(FakeRepo<Viaje> inner) => _inner = inner;
        public Task<IReadOnlyList<Viaje>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Viaje?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Viaje entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Viaje entity) => _inner.DeleteAsync(entity);

        public Task<IReadOnlyList<Viaje>> ReadFilterByOrigenDestinoAsync(string p_origen, string p_destino) =>
            throw new NotImplementedException();
        public Task<IReadOnlyList<Viaje>> ReadFilterByFechaYPlazasAsync(DateTime p_desde, DateTime p_hasta, int p_plazasMinimas) =>
            throw new NotImplementedException();
        public Task<IReadOnlyList<Viaje>> ReadFilterByPrecioAsync(double p_precioMinimo, double p_precioMaximo) =>
            throw new NotImplementedException();
        public Task<IReadOnlyList<Viaje>> ReadFilterByPreferenciasYEstadoAsync(bool p_permiteMascotas, bool p_permiteEquipaje) =>
            throw new NotImplementedException();
    }

    private sealed class FakeUsuarioRepo : IUsuarioRepository
    {
        private readonly FakeRepo<Usuario> _inner;
        public FakeUsuarioRepo(FakeRepo<Usuario> inner) => _inner = inner;
        public Task<IReadOnlyList<Usuario>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Usuario?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Usuario entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Usuario entity) => _inner.DeleteAsync(entity);
        public Task<Usuario?> ReadByEmailAsync(string email) =>
            Task.FromResult(_inner.Items.FirstOrDefault(u => u.Email == email));
        public Task<Usuario?> ReadByDniAsync(string dni) =>
            Task.FromResult(_inner.Items.FirstOrDefault(u => u.Dni == dni));
    }

    private sealed class FakeNotificacionRepo : INotificacionRepository
    {
        private readonly FakeRepo<Notificacion> _inner;
        public FakeNotificacionRepo(FakeRepo<Notificacion> inner) => _inner = inner;
        public IReadOnlyList<Notificacion> Items => _inner.Items;
        public Task<IReadOnlyList<Notificacion>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Notificacion?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Notificacion entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Notificacion entity) => _inner.DeleteAsync(entity);
    }
}