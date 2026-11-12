
using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Application.Services;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

namespace CarGo.Tests.Services;

public sealed class ViajeServiceGeolocalizarTrayectoTests
{
    private const int AdminId = 1;
    private const int ConductorId = 10;
    private const int PasajeroId = 20;
    private const int AjenoId = 30;

    // ==========================================
    // TEST 1: El conductor envía su posición
    // ==========================================

    [Fact]
    public async Task Geolocalizar_Conductor_GuardaLaPosicion()
    {
        var ctx = CrearContexto();
        var fecha = DateTime.Now;

        await ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, 39.4699, -0.3763, fecha);

        Assert.Equal((double?)39.4699, ctx.Viaje.LatitudActual);
        Assert.Equal((double?)-0.3763, ctx.Viaje.LongitudActual);
        Assert.NotNull(ctx.Viaje.FechaUltimaPosicion);
        Assert.Equal(1, ctx.UnitOfWork.CommitCount);
        Assert.Equal(0, ctx.UnitOfWork.RollbackCount);
    }

    // ==========================================
    // TEST 2: Un administrador también puede enviarla
    // ==========================================

    [Fact]
    public async Task Geolocalizar_Admin_GuardaLaPosicion()
    {
        var ctx = CrearContexto();
        var fecha = DateTime.Now;

        await ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, AdminId, 40.4168, -3.7038, fecha);

        Assert.Equal((double?)40.4168, ctx.Viaje.LatitudActual);
        Assert.Equal((double?)-3.7038, ctx.Viaje.LongitudActual);
        Assert.Equal(1, ctx.UnitOfWork.CommitCount);
    }

    // ==========================================
    // TEST 3: Se registra la fecha de la última posición
    // ==========================================

    [Fact]
    public async Task Geolocalizar_RegistraLaFechaDeLaUltimaPosicion()
    {
        var ctx = CrearContexto();
        var fecha = DateTime.Now;

        await ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, 39.0, -0.5, fecha);

        Assert.Equal(fecha, ctx.Viaje.FechaUltimaPosicion);
    }

    // ==========================================
    // TEST 4: La última posición sustituye a la anterior
    // ==========================================

    [Fact]
    public async Task Geolocalizar_SegundoEnvio_SobrescribeElPrimero()
    {
        var ctx = CrearContexto();

        await ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, 39.0, -0.5, DateTime.Now);
        await ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, 40.0, -1.5, DateTime.Now);

        Assert.Equal((double?)40.0, ctx.Viaje.LatitudActual);
        Assert.Equal((double?)-1.5, ctx.Viaje.LongitudActual);
        Assert.Equal(2, ctx.UnitOfWork.CommitCount);
    }

    // ==========================================
    // TEST 5: Viaje inexistente
    // ==========================================

    [Fact]
    public async Task Geolocalizar_ViajeInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GeolocalizarTrayectoAsync(999, ConductorId, 39.0, -0.5, DateTime.Now));

        Assert.Equal(0, ctx.UnitOfWork.CommitCount);
        Assert.Equal(1, ctx.UnitOfWork.RollbackCount);
    }

    // ==========================================
    // TEST 6: Solo se geolocaliza un viaje en curso
    // ==========================================

    [Theory]
    [InlineData(EstadoViaje.Publicado)]
    [InlineData(EstadoViaje.Finalizado)]
    [InlineData(EstadoViaje.Cancelado)]
    public async Task Geolocalizar_ViajeNoEnCurso_LanzaError(EstadoViaje estado)
    {
        var ctx = CrearContexto(estado: estado);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, 39.0, -0.5, DateTime.Now));

        AssertNadaCambio(ctx);
    }

    // ==========================================
    // TEST 7: Latitud fuera de rango (incluye NaN e infinito)
    // ==========================================

    [Theory]
    [InlineData(90.0001)]
    [InlineData(-90.0001)]
    [InlineData(1000)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public async Task Geolocalizar_LatitudFueraDeRango_LanzaError(double latitud)
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, latitud, -0.5, DateTime.Now));

        AssertNadaCambio(ctx);
    }

    // ==========================================
    // TEST 8: Longitud fuera de rango (incluye NaN e infinito)
    // ==========================================

    [Theory]
    [InlineData(180.0001)]
    [InlineData(-180.0001)]
    [InlineData(1000)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public async Task Geolocalizar_LongitudFueraDeRango_LanzaError(double longitud)
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, 39.0, longitud, DateTime.Now));

        AssertNadaCambio(ctx);
    }

    // ==========================================
    // TEST 9: Los límites exactos son válidos
    // ==========================================

    [Theory]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    [InlineData(0, 0)]
    public async Task Geolocalizar_LimitesExactos_SonValidos(double latitud, double longitud)
    {
        var ctx = CrearContexto();
        var fecha = DateTime.Now;

        await ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, latitud, longitud, fecha);

        Assert.Equal((double?)latitud, ctx.Viaje.LatitudActual);
        Assert.Equal((double?)longitud, ctx.Viaje.LongitudActual);
    }

    // ==========================================
    // TEST 10: Usuario inexistente
    // ==========================================

    [Fact]
    public async Task Geolocalizar_UsuarioInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, 999, 39.0, -0.5, DateTime.Now));

        AssertNadaCambio(ctx);
    }

    // ==========================================
    // TEST 11: Ni conductor ni admin no pueden enviar posiciones
    // ==========================================

    [Theory]
    [InlineData(PasajeroId)]
    [InlineData(AjenoId)]
    public async Task Geolocalizar_UsuarioSinPermiso_LanzaError(int usuarioId)
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, usuarioId, 39.0, -0.5, DateTime.Now));

        AssertNadaCambio(ctx);
    }

    // ==========================================
    // TEST 12: Si falla una validación, no se pisa una posición previa
    // ==========================================

    [Fact]
    public async Task Geolocalizar_EnvioRechazado_ConservaLaPosicionAnterior()
    {
        var ctx = CrearContexto();

        await ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, ConductorId, 39.0, -0.5, DateTime.Now);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GeolocalizarTrayectoAsync(ctx.Viaje.Id, PasajeroId, 41.0, -2.0, DateTime.Now));

        Assert.Equal((double?)39.0, ctx.Viaje.LatitudActual);
        Assert.Equal((double?)-0.5, ctx.Viaje.LongitudActual);
    }

    // ==========================================
    // HELPERS
    // ==========================================

    private static void AssertNadaCambio(Contexto ctx)
    {
        Assert.Null(ctx.Viaje.LatitudActual);
        Assert.Null(ctx.Viaje.LongitudActual);
        Assert.Null(ctx.Viaje.FechaUltimaPosicion);
        Assert.Equal(0, ctx.UnitOfWork.CommitCount);
        Assert.Equal(1, ctx.UnitOfWork.RollbackCount);
    }

    private static Contexto CrearContexto(EstadoViaje estado = EstadoViaje.EnCurso)
    {
        var admin = new Usuario { Id = AdminId, NombreCompleto = "Admin Test", EsAdmin = true };
        var conductor = new Usuario { Id = ConductorId, NombreCompleto = "Conductor Test", EsAdmin = false };
        var pasajero = new Usuario { Id = PasajeroId, NombreCompleto = "Pasajero Test", EsAdmin = false };
        var ajeno = new Usuario { Id = AjenoId, NombreCompleto = "Ajeno Test", EsAdmin = false };

        var viaje = new Viaje
        {
            Id = 1,
            Estado = estado,
            Conductor = conductor,
        };

        viaje.Pasajero.Add(pasajero);

        var uow = new FakeUnitOfWork(
            new FakeRepo<Viaje>(v => v.Id, viaje),
            new FakeRepo<Usuario>(u => u.Id, admin, conductor, pasajero, ajeno));

        return new Contexto(new ViajeService(uow), uow, viaje);
    }

    private sealed record Contexto(
        ViajeService Service,
        FakeUnitOfWork UnitOfWork,
        Viaje Viaje);

    // ==========================================
    // FAKES
    // ==========================================

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
        private readonly FakeViajeRepo _viajes;
        private readonly FakeUsuarioRepo _usuarios;

        public int CommitCount { get; private set; }
        public int RollbackCount { get; private set; }

        public FakeUnitOfWork(FakeRepo<Viaje> viajes, FakeRepo<Usuario> usuarios)
        {
            _viajes = new FakeViajeRepo(viajes);
            _usuarios = new FakeUsuarioRepo(usuarios);
        }

        public IViajeRepository ViajeRepository => _viajes;
        public IUsuarioRepository UsuarioRepository => _usuarios;

        public IVehiculoRepository VehiculoRepository => null!;
        public IReservaRepository ReservaRepository => null!;
        public IPagoRepository PagoRepository => null!;
        public IMensajeRepository MensajeRepository => null!;
        public IResenaRepository ResenaRepository => null!;
        public INotificacionRepository NotificacionRepository => null!;
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

    // Adaptadores: cada interfaz de repositorio delega en el FakeRepo genérico.

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

            public Task<IReadOnlyList<Viaje>> ReadFilterViajesDisponiblesAsync(
                DateTime p_fechaMinima)
            {
                return Task.FromResult<IReadOnlyList<Viaje>>(
                    Array.Empty<Viaje>());
            }

            public Task<IReadOnlyList<Viaje>> ReadFilterByConductorVerificadoAsync()
            {
                return Task.FromResult<IReadOnlyList<Viaje>>(
                    Array.Empty<Viaje>());
            }
    }

    private sealed class FakeUsuarioRepo : IUsuarioRepository
    {
        private readonly FakeRepo<Usuario> _inner;
        public FakeUsuarioRepo(FakeRepo<Usuario> inner) => _inner = inner;
        public Task<IReadOnlyList<Usuario>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Usuario?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Usuario entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Usuario entity) => _inner.DeleteAsync(entity);
        public Task<IReadOnlyList<Usuario>> ReadFilterByEmailAsync(string p_email) =>
            Task.FromResult<IReadOnlyList<Usuario>>(
                _inner.Items
                    .Where(u => u.Email != null &&
                                u.Email.Trim().Equals(p_email.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList());

        public Task<IReadOnlyList<Usuario>> ReadFilterByDniAsync(string p_dni) =>
            Task.FromResult<IReadOnlyList<Usuario>>(
                _inner.Items
                    .Where(u => u.Dni != null &&
                                u.Dni.Trim().Equals(p_dni.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList());
    }
}
