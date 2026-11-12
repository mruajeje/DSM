using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Application.Services;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

namespace CarGo.Tests.Services;

public sealed class ResenaServiceModerarContenidoTests
{
    private const int AdminId = 1;
    private const int AutorId = 2;
    private const string ComentarioOriginal = "Comentario de prueba";


    // TEST 1: Ocultar -> vacia el comentario y conserva la puntuacion
    [Fact]
    public async Task ModerarContenido_Oculta_VaciaComentarioYConservaPuntuacion()
    {
        var ctx = CrearContexto(puntuacion: 4);

        await ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Oculta, "Lenguaje vulgar");

        Assert.Equal(EstadoResena.Oculta, ctx.Resena.Estado);
        Assert.Null(ctx.Resena.Comentario);
        Assert.Equal(4, ctx.Resena.Puntuacion);
        Assert.Equal("Lenguaje vulgar", ctx.Resena.MotivoModeracion);
        Assert.Equal(1, ctx.UnitOfWork.CommitCount);
        Assert.Equal(0, ctx.UnitOfWork.RollbackCount);
    }


    // TEST 2: Aceptar -> conserva el comentario
    [Fact]
    public async Task ModerarContenido_Aceptada_ConservaComentarioYPuntuacion()
    {
        var ctx = CrearContexto(puntuacion: 5);

        await ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Aceptada, null!);

        Assert.Equal(EstadoResena.Aceptada, ctx.Resena.Estado);
        Assert.Equal(ComentarioOriginal, ctx.Resena.Comentario);
        Assert.Equal(5, ctx.Resena.Puntuacion);
        Assert.Null(ctx.Resena.MotivoModeracion);
        Assert.Equal(1, ctx.UnitOfWork.CommitCount);
    }


    // TEST 3: Rechazar -> conserva todo (borrado logico)
    [Fact]
    public async Task ModerarContenido_Rechazada_ConservaLaFilaYElComentario()
    {
        var ctx = CrearContexto(puntuacion: 1);

        await ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Rechazada, "Insultos");

        Assert.Equal(EstadoResena.Rechazada, ctx.Resena.Estado);
        Assert.Equal(ComentarioOriginal, ctx.Resena.Comentario);
        Assert.Equal(1, ctx.Resena.Puntuacion);
        Assert.Equal("Insultos", ctx.Resena.MotivoModeracion);
        Assert.Single(ctx.UnitOfWork.Resenas.Items);
    }


    // TEST 4: El motivo se guarda sin espacios sobrantes
    [Fact]
    public async Task ModerarContenido_Motivo_SeGuardaSinEspaciosSobrantes()
    {
        var ctx = CrearContexto();

        await ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Rechazada, "   Insultos   ");

        Assert.Equal("Insultos", ctx.Resena.MotivoModeracion);
    }


    // TEST 5: Se notifica al autor con cada decision
    [Theory]
    [InlineData(EstadoResena.Aceptada)]
    [InlineData(EstadoResena.Oculta)]
    [InlineData(EstadoResena.Rechazada)]
    public async Task ModerarContenido_CreaNotificacionParaElAutor(EstadoResena decision)
    {
        var ctx = CrearContexto();

        await ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, decision, "Motivo de prueba");

        var notificacion = Assert.Single(ctx.UnitOfWork.Notificaciones.Items);
        Assert.Equal(AutorId, notificacion.UsuarioNotificado.Id);
        Assert.False(string.IsNullOrWhiteSpace(notificacion.Titulo));
        Assert.False(string.IsNullOrWhiteSpace(notificacion.Texto));
    }


    // TEST 6: Resena inexistente
    [Fact]
    public async Task ModerarContenido_ResenaInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(999, AdminId, EstadoResena.Aceptada, null!));

        Assert.Equal(0, ctx.UnitOfWork.CommitCount);
        Assert.Equal(1, ctx.UnitOfWork.RollbackCount);
        Assert.Empty(ctx.UnitOfWork.Notificaciones.Items);
    }


    // TEST 7: Solo se moderan resenas pendientes
    [Theory]
    [InlineData(EstadoResena.Aceptada)]
    [InlineData(EstadoResena.Oculta)]
    [InlineData(EstadoResena.Rechazada)]
    public async Task ModerarContenido_ResenaYaModerada_LanzaError(EstadoResena estadoActual)
    {
        var ctx = CrearContexto(estadoInicial: estadoActual);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Rechazada, "Motivo"));

        AssertNadaCambio(ctx, estadoActual);
    }


    // TEST 8: Admin inexistente
    [Fact]
    public async Task ModerarContenido_AdminInexistente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, 999, EstadoResena.Aceptada, null!));

        AssertNadaCambio(ctx, EstadoResena.Pendiente);
    }

    // ==========================================
    // TEST 9: El usuario no es administrador
    [Fact]
    public async Task ModerarContenido_UsuarioNoAdmin_LanzaError()
    {
        var ctx = CrearContexto();

        // AutorId existe pero no es administrador
        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AutorId, EstadoResena.Aceptada, null!));

        AssertNadaCambio(ctx, EstadoResena.Pendiente);
    }


    // TEST 10: Un admin no puede moderar su propia resena
    [Fact]
    public async Task ModerarContenido_AdminModeraSuPropiaResena_LanzaError()
    {
        var ctx = CrearContexto(autorEsElAdmin: true);

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Aceptada, null!));

        AssertNadaCambio(ctx, EstadoResena.Pendiente);
    }


    // TEST 11: Decision no permitida (Pendiente)
    [Fact]
    public async Task ModerarContenido_DecisionPendiente_LanzaError()
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Pendiente, "Motivo"));

        AssertNadaCambio(ctx, EstadoResena.Pendiente);
    }


    // TEST 12: Motivo obligatorio al ocultar o rechazar
    [Theory]
    [InlineData(EstadoResena.Oculta, null)]
    [InlineData(EstadoResena.Oculta, "")]
    [InlineData(EstadoResena.Oculta, "   ")]
    [InlineData(EstadoResena.Rechazada, null)]
    [InlineData(EstadoResena.Rechazada, "")]
    [InlineData(EstadoResena.Rechazada, "   ")]
    public async Task ModerarContenido_SinMotivo_LanzaError(EstadoResena decision, string? motivo)
    {
        var ctx = CrearContexto();

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, decision, motivo!));

        AssertNadaCambio(ctx, EstadoResena.Pendiente);
    }


    // TEST 13: Aceptar no exige motivo
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ModerarContenido_AceptarSinMotivo_NoLanzaError(string? motivo)
    {
        var ctx = CrearContexto();

        await ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Aceptada, motivo!);

        Assert.Equal(EstadoResena.Aceptada, ctx.Resena.Estado);
        Assert.Null(ctx.Resena.MotivoModeracion);
    }


    // TEST 14: Una vez moderada no se puede moderar de nuevo
    [Fact]
    public async Task ModerarContenido_SegundaModeracion_LanzaErrorYConservaLaPrimera()
    {
        var ctx = CrearContexto();

        await ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Oculta, "Vulgar");

        await Assert.ThrowsAsync<ModelException>(
            () => ctx.Service.ModerarContenidoAsync(ctx.Resena.Id, AdminId, EstadoResena.Aceptada, null!));

        Assert.Equal(EstadoResena.Oculta, ctx.Resena.Estado);
        Assert.Null(ctx.Resena.Comentario);
        Assert.Single(ctx.UnitOfWork.Notificaciones.Items);
    }

    // HELPERS
    // Comprueba que una operacion fallida no ha modificado nada.
    private static void AssertNadaCambio(Contexto ctx, EstadoResena estadoEsperado)
    {
        Assert.Equal(estadoEsperado, ctx.Resena.Estado);
        Assert.Equal(ComentarioOriginal, ctx.Resena.Comentario);
        Assert.Null(ctx.Resena.MotivoModeracion);
        Assert.Empty(ctx.UnitOfWork.Notificaciones.Items);
        Assert.Equal(0, ctx.UnitOfWork.CommitCount);
        Assert.Equal(1, ctx.UnitOfWork.RollbackCount);
    }

    private static Contexto CrearContexto(
        EstadoResena estadoInicial = EstadoResena.Pendiente,
        int puntuacion = 3,
        bool autorEsElAdmin = false)
    {
        var admin = new Usuario { Id = AdminId, NombreCompleto = "Admin Test", EsAdmin = true };
        var autor = autorEsElAdmin
            ? admin
            : new Usuario { Id = AutorId, NombreCompleto = "Autor Test", EsAdmin = false };

        var resena = new Resena
        {
            Id = 100,
            Puntuacion = puntuacion,
            Comentario = ComentarioOriginal,
            FechaCreacion = DateTime.Now,
            Estado = estadoInicial,
            Autor = autor,
        };

        var uow = new FakeUnitOfWork(
            new FakeRepo<Resena>(r => r.Id, resena),
            new FakeRepo<Usuario>(u => u.Id, admin, autor),
            new FakeRepo<Notificacion>(n => n.Id));

        return new Contexto(new ResenaService(uow), uow, resena);
    }

    private sealed record Contexto(
        ResenaService Service,
        FakeUnitOfWork UnitOfWork,
        Resena Resena);

  
    // FAKES
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
        private readonly FakeUsuarioRepo _usuarios;

        public FakeResenaRepo Resenas { get; }
        public FakeNotificacionRepo Notificaciones { get; }

        public int CommitCount { get; private set; }
        public int RollbackCount { get; private set; }

        public FakeUnitOfWork(
            FakeRepo<Resena> resenas,
            FakeRepo<Usuario> usuarios,
            FakeRepo<Notificacion> notificaciones)
        {
            Resenas = new FakeResenaRepo(resenas);
            _usuarios = new FakeUsuarioRepo(usuarios);
            Notificaciones = new FakeNotificacionRepo(notificaciones);
        }

        public IResenaRepository ResenaRepository => Resenas;
        public IUsuarioRepository UsuarioRepository => _usuarios;
        public INotificacionRepository NotificacionRepository => Notificaciones;

        public IVehiculoRepository VehiculoRepository => null!;
        public IViajeRepository ViajeRepository => null!;
        public IReservaRepository ReservaRepository => null!;
        public IPagoRepository PagoRepository => null!;
        public IMensajeRepository MensajeRepository => null!;
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

    private sealed class FakeResenaRepo : IResenaRepository
    {
        private readonly FakeRepo<Resena> _inner;
        public FakeResenaRepo(FakeRepo<Resena> inner) => _inner = inner;
        public IReadOnlyList<Resena> Items => _inner.Items;
        public Task<IReadOnlyList<Resena>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<Resena?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Resena entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Resena entity) => _inner.DeleteAsync(entity);
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
                                u.Email.Equals(p_email.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList());

        public Task<IReadOnlyList<Usuario>> ReadFilterByDniAsync(string p_dni) =>
        Task.FromResult<IReadOnlyList<Usuario>>(
            _inner.Items
                .Where(u => u.Dni != null &&
                            u.Dni.Equals(p_dni.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList());
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