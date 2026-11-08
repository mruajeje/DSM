
using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Application.Services;
using CarGoGen.Domain.Entities;

namespace CarGoGen.Tests.Services;

public sealed class ViajeService_PasajerosTests
{
    private const int ViajeId = 10;
    private const int UsuarioId = 20;

    // TEST 1: Alta correcta de un pasajero
    [Fact]
    public async Task GestionarAltaPasajero_AltaCorrecta_AgregaUsuarioYRestaPlaza()
    {
        var ctx = CrearContexto(plazasDisponibles: 2);

        await ctx.Service.GestionarAltaPasajeroAsync(ViajeId, UsuarioId);

        Assert.Contains(ctx.Usuario, ctx.Viaje.Pasajero!);
        Assert.Contains(ctx.Viaje, ctx.Usuario.Viajespasajero!);
        Assert.Equal(1, ctx.Viaje.NumPlazasDisponibles);
    }

    // TEST 2: No se puede añadir un pasajero si no quedan plazas
    [Fact]
    public async Task GestionarAltaPasajero_SinPlazas_LanzaError()
    {
        var ctx = CrearContexto(plazasDisponibles: 0);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarAltaPasajeroAsync(ViajeId, UsuarioId));

        Assert.Empty(ctx.Viaje.Pasajero!);
        Assert.Equal(0, ctx.Viaje.NumPlazasDisponibles);
    }

    // TEST 3: No se puede añadir dos veces al mismo usuario
    [Fact]
    public async Task GestionarAltaPasajero_UsuarioYaExiste_LanzaError()
    {
        var ctx = CrearContexto(plazasDisponibles: 2);
        ctx.Viaje.Pasajero!.Add(ctx.Usuario);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarAltaPasajeroAsync(ViajeId, UsuarioId));

        Assert.Equal(2, ctx.Viaje.NumPlazasDisponibles);
    }

    // TEST 4: El viaje debe existir
    [Fact]
    public async Task GestionarAltaPasajero_ViajeInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarAltaPasajeroAsync(999, UsuarioId));
    }

    // TEST 5: El usuario debe existir
    [Fact]
    public async Task GestionarAltaPasajero_UsuarioInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarAltaPasajeroAsync(ViajeId, 999));

        Assert.Empty(ctx.Viaje.Pasajero!);
    }

    // TEST 6: Baja correcta de un pasajero
    [Fact]
    public async Task GestionarBajaPasajero_BajaCorrecta_EliminaUsuarioYDevuelvePlaza()
    {
        var ctx = CrearContexto(plazasDisponibles: 1);
        ctx.Viaje.Pasajero!.Add(ctx.Usuario);
        ctx.Usuario.Viajespasajero!.Add(ctx.Viaje);

        await ctx.Service.GestionarBajaPasajeroAsync(ViajeId, UsuarioId);

        Assert.DoesNotContain(ctx.Usuario, ctx.Viaje.Pasajero);
        Assert.DoesNotContain(ctx.Viaje, ctx.Usuario.Viajespasajero);
        Assert.Equal(2, ctx.Viaje.NumPlazasDisponibles);
    }

    // TEST 7: No se puede dar de baja a alguien que no está en el viaje
    [Fact]
    public async Task GestionarBajaPasajero_UsuarioNoEstaEnViaje_LanzaError()
    {
        var ctx = CrearContexto(plazasDisponibles: 1);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarBajaPasajeroAsync(ViajeId, UsuarioId));

        Assert.Equal(1, ctx.Viaje.NumPlazasDisponibles);
    }

    // TEST 8: El viaje debe existir para dar de baja
    [Fact]
    public async Task GestionarBajaPasajero_ViajeInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarBajaPasajeroAsync(999, UsuarioId));
    }

    // TEST 9: El usuario debe existir para dar de baja
    [Fact]
    public async Task GestionarBajaPasajero_UsuarioInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.GestionarBajaPasajeroAsync(ViajeId, 999));
    }

    // CONTEXTO DE PRUEBAS
    private static Contexto CrearContexto(int plazasDisponibles = 2)
    {
        var usuario = new Usuario
        {
            Id = UsuarioId,
            NombreCompleto = "Usuario de prueba",
            Viajespasajero = new List<Viaje>()
        };

        var viaje = new Viaje
        {
            Id = ViajeId,
            NumPlazasDisponibles = plazasDisponibles,
            Pasajero = new List<Usuario>()
        };

        var uow = new FakeUnitOfWork(viaje, usuario);

        return new Contexto(new ViajeService(uow), uow, viaje, usuario);
    }

    private sealed record Contexto(
        ViajeService Service,
        FakeUnitOfWork UnitOfWork,
        Viaje Viaje,
        Usuario Usuario);

    // REPOSITORIO FALSO GENÉRICO
    private sealed class FakeRepo<T> where T : class
    {
        private readonly Func<T, int> _getId;
        private readonly Dictionary<int, T> _items = new();

        public FakeRepo(Func<T, int> getId, params T[] items)
        {
            _getId = getId;

            foreach (var item in items)
                _items[getId(item)] = item;
        }

        public Task<IReadOnlyList<T>> ReadAllAsync() =>
            Task.FromResult<IReadOnlyList<T>>(_items.Values.ToList());

        public Task<T?> ReadAsync(object id)
        {
            T? result = null;

            if (id is int key)
                _items.TryGetValue(key, out result);

            return Task.FromResult(result);
        }

        public Task SaveAsync(T entity)
        {
            _items[_getId(entity)] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _items.Remove(_getId(entity));
            return Task.CompletedTask;
        }
    }

    // UNIT OF WORK FALSO
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public IViajeRepository ViajeRepository { get; }
        public IUsuarioRepository UsuarioRepository { get; }

        public FakeUnitOfWork(Viaje viaje, Usuario usuario)
        {
            ViajeRepository = new FakeViajeRepo(
                new FakeRepo<Viaje>(v => v.Id, viaje));

            UsuarioRepository = new FakeUsuarioRepo(
                new FakeRepo<Usuario>(u => u.Id, usuario));
        }

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

    // ADAPTADOR DEL REPOSITORIO DE VIAJES
    private sealed class FakeViajeRepo : IViajeRepository
    {
        private readonly FakeRepo<Viaje> _inner;

        public FakeViajeRepo(FakeRepo<Viaje> inner) => _inner = inner;

        public Task<IReadOnlyList<Viaje>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Viaje?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Viaje entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Viaje entity) => _inner.DeleteAsync(entity);

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

    // ADAPTADOR DEL REPOSITORIO DE USUARIOS
    private sealed class FakeUsuarioRepo : IUsuarioRepository
    {
        private readonly FakeRepo<Usuario> _inner;

        public FakeUsuarioRepo(FakeRepo<Usuario> inner) => _inner = inner;

        public Task<IReadOnlyList<Usuario>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Usuario?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Usuario entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Usuario entity) => _inner.DeleteAsync(entity);

        public Task<IReadOnlyList<Usuario>> ReadFilterByEmailAsync(string p_email) =>
            Task.FromResult<IReadOnlyList<Usuario>>(Array.Empty<Usuario>());

        public Task<IReadOnlyList<Usuario>> ReadFilterByDniAsync(string p_dni) =>
            Task.FromResult<IReadOnlyList<Usuario>>(Array.Empty<Usuario>());
    }
}
