
using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Services;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Tests.Services;

public sealed class UsuarioService_OperacionesTests
{
    private const int UsuarioId = 10;
    private const string DniValido = "12345678Z";

    // ==========================================
    // TESTS DE LOGIN
    // ==========================================

    // TEST 1: Login correcto
    [Fact]
    public async Task LoginEmail_CredencialesCorrectas_DevuelveToken()
    {
        var ctx = CrearContextoUsuario(
            email: "usuario@test.com",
            password: "hash-correcto");

        ctx.Hasher.PasswordValido = true;

        var resultado = await ctx.Service.LoginEmailAsync(
            "usuario@test.com", "password123");

        Assert.Equal("token-de-prueba", resultado);
        Assert.Equal(1, ctx.Hasher.VerifyCount);
        Assert.Equal(1, ctx.TokenGenerator.CreateTokenCount);
    }

    // TEST 2: Contraseña incorrecta
    [Fact]
    public async Task LoginEmail_ContrasenaIncorrecta_DevuelveVacio()
    {
        var ctx = CrearContextoUsuario(
            email: "usuario@test.com",
            password: "hash-correcto");

        ctx.Hasher.PasswordValido = false;

        var resultado = await ctx.Service.LoginEmailAsync(
            "usuario@test.com", "incorrecta");

        Assert.Equal(string.Empty, resultado);
        Assert.Equal(1, ctx.Hasher.VerifyCount);
        Assert.Equal(0, ctx.TokenGenerator.CreateTokenCount);
    }

    // TEST 3: Correo inexistente
    [Fact]
    public async Task LoginEmail_CorreoInexistente_DevuelveVacio()
    {
        var ctx = CrearContextoUsuario(
            email: "usuario@test.com",
            password: "hash-correcto");

        var resultado = await ctx.Service.LoginEmailAsync(
            "otro@test.com", "password123");

        Assert.Equal(string.Empty, resultado);
        Assert.Equal(0, ctx.Hasher.VerifyCount);
        Assert.Equal(0, ctx.TokenGenerator.CreateTokenCount);
    }

    // TEST 4: El correo se busca sin distinguir mayúsculas
    [Fact]
    public async Task LoginEmail_CorreoConMayusculas_DevuelveToken()
    {
        var ctx = CrearContextoUsuario(
            email: "usuario@test.com",
            password: "hash-correcto");

        ctx.Hasher.PasswordValido = true;

        var resultado = await ctx.Service.LoginEmailAsync(
            "USUARIO@TEST.COM", "password123");

        Assert.Equal("token-de-prueba", resultado);
    }

    // ==========================================
    // TESTS DE VERIFICACIÓN DEL DNI
    // ==========================================

    // TEST 5: DNI correcto
    [Fact]
    public async Task VerificarDni_DniValido_GuardaDniYPendiente()
    {
        var ctx = CrearContextoUsuario();

        await ctx.Service.VerificarDniAsync(UsuarioId, DniValido);

        Assert.Equal(DniValido, ctx.Usuario.Dni);
        Assert.Equal(
            EstadoVerificacion.Pendiente,
            ctx.Usuario.EstadoVerificacion);

        Assert.Equal(1, ctx.Repo.SaveCount);
    }

    // TEST 6: El DNI se guarda normalizado
    [Fact]
    public async Task VerificarDni_DniConEspaciosYMinusculas_GuardaNormalizado()
    {
        var ctx = CrearContextoUsuario();

        await ctx.Service.VerificarDniAsync(UsuarioId, " 12345678z ");

        Assert.Equal(DniValido, ctx.Usuario.Dni);
    }

    // TEST 7: DNI vacío
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task VerificarDni_DniVacio_LanzaError(string? dni)
    {
        var ctx = CrearContextoUsuario();

        await Assert.ThrowsAsync<CarGoGen.Application.Exceptions.ModelException>(
            () => ctx.Service.VerificarDniAsync(UsuarioId, dni!));

        Assert.Null(ctx.Usuario.Dni);
        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // TEST 8: Formato incorrecto
    [Theory]
    [InlineData("1234567Z")]
    [InlineData("123456789Z")]
    [InlineData("ABCDEFGHZ")]
    [InlineData("12345678")]
    public async Task VerificarDni_FormatoIncorrecto_LanzaError(string dni)
    {
        var ctx = CrearContextoUsuario();

        await Assert.ThrowsAsync<CarGoGen.Application.Exceptions.ModelException>(
            () => ctx.Service.VerificarDniAsync(UsuarioId, dni));

        Assert.Null(ctx.Usuario.Dni);
        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // TEST 9: Letra incorrecta
    [Fact]
    public async Task VerificarDni_LetraIncorrecta_LanzaError()
    {
        var ctx = CrearContextoUsuario();

        await Assert.ThrowsAsync<CarGoGen.Application.Exceptions.ModelException>(
            () => ctx.Service.VerificarDniAsync(UsuarioId, "12345678A"));

        Assert.Null(ctx.Usuario.Dni);
        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // TEST 10: Usuario inexistente
    [Fact]
    public async Task VerificarDni_UsuarioInexistente_LanzaError()
    {
        var ctx = CrearContextoUsuario();

        await Assert.ThrowsAsync<CarGoGen.Application.Exceptions.ModelException>(
            () => ctx.Service.VerificarDniAsync(999, DniValido));

        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // TEST 11: DNI duplicado
    [Fact]
    public async Task VerificarDni_DniYaRegistrado_LanzaError()
    {
        var ctx = CrearContextoUsuario();

        var otroUsuario = new Usuario
        {
            Id = 20,
            Dni = DniValido
        };

        await ctx.Repo.SaveAsync(otroUsuario);

        await Assert.ThrowsAsync<CarGoGen.Application.Exceptions.ModelException>(
            () => ctx.Service.VerificarDniAsync(UsuarioId, DniValido));

        Assert.Null(ctx.Usuario.Dni);
    }

    // TEST 12: Un DNI ya registrado no se puede cambiar
    [Fact]
    public async Task VerificarDni_DniYaAsignadoNoCoincide_LanzaError()
    {
        var ctx = CrearContextoUsuario();
        ctx.Usuario.Dni = DniValido;

        await Assert.ThrowsAsync<CarGoGen.Application.Exceptions.ModelException>(
            () => ctx.Service.VerificarDniAsync(UsuarioId, "87654321X"));

        Assert.Equal(DniValido, ctx.Usuario.Dni);
    }

    // TEST 13: Volver a verificar el mismo DNI no modifica nada
    [Fact]
    public async Task VerificarDni_MismoDni_NoVuelveAGuardar()
    {
        var ctx = CrearContextoUsuario();
        ctx.Usuario.Dni = DniValido;

        await ctx.Service.VerificarDniAsync(UsuarioId, DniValido);

        Assert.Equal(DniValido, ctx.Usuario.Dni);
        Assert.Equal(0, ctx.Repo.SaveCount);
    }

    // ==========================================
    // CONTEXTO DE PRUEBAS
    // ==========================================

    private static Contexto CrearContextoUsuario(
    string email = "usuario@test.com",
    string password = "hash-correcto")
    {
        var usuario = new Usuario
        {
            Id = UsuarioId,
            Email = email,
            Password = password
        };

        var repo = new FakeUsuarioRepo(usuario);
        var hasher = new FakePasswordHasher();
        var tokenGenerator = new FakeTokenGenerator();

        var uow = new FakeUnitOfWork(repo, hasher, tokenGenerator);

        return new Contexto(
            new UsuarioService(uow),
            usuario,
            repo,
            hasher,
            tokenGenerator);
    }

    private sealed record Contexto(
        UsuarioService Service,
        Usuario Usuario,
        FakeUsuarioRepo Repo,
        FakePasswordHasher Hasher,
        FakeTokenGenerator TokenGenerator);

    // ==========================================
    // REPOSITORIO FALSO DE USUARIOS
    // ==========================================

    private sealed class FakeUsuarioRepo : IUsuarioRepository
    {
        private readonly Dictionary<int, Usuario> _items = new();

        public int SaveCount { get; private set; }

        public FakeUsuarioRepo(params Usuario[] usuarios)
        {
            foreach (var usuario in usuarios)
                _items[usuario.Id] = usuario;
        }

        public Task<IReadOnlyList<Usuario>> ReadAllAsync() =>
            Task.FromResult<IReadOnlyList<Usuario>>(_items.Values.ToList());

        public Task<Usuario?> ReadAsync(object id)
        {
            Usuario? usuario = null;

            if (id is int key)
                _items.TryGetValue(key, out usuario);

            return Task.FromResult(usuario);
        }

        public Task SaveAsync(Usuario entity)
        {
            _items[entity.Id] = entity;
            SaveCount++;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Usuario entity)
        {
            _items.Remove(entity.Id);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Usuario>> ReadFilterByEmailAsync(string p_email)
        {
            var resultado = _items.Values
                .Where(u => u.Email != null &&
                    u.Email.Trim().Equals(
                        p_email.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult<IReadOnlyList<Usuario>>(resultado);
        }

        public Task<IReadOnlyList<Usuario>> ReadFilterByDniAsync(string p_dni)
        {
            var resultado = _items.Values
                .Where(u => u.Dni != null &&
                    u.Dni.Trim().Equals(
                        p_dni.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult<IReadOnlyList<Usuario>>(resultado);
        }
    }

    // ==========================================
    // HASHER FALSO
    // ==========================================

    private sealed class FakePasswordHasher : IHariaPasswordHasher
    {
        public bool PasswordValido { get; set; }
        public int VerifyCount { get; private set; }

        public string HashPasswordIfNeeded(string password) => password;

        public string HashPassword(string password) => password;

        public bool VerifyPassword(string? storedHash, string? password)
        {
            VerifyCount++;
            return PasswordValido;
        }
    }

    // ==========================================
    // GENERADOR DE TOKEN FALSO
    // ==========================================

    private sealed class FakeTokenGenerator : IHariaTokenGenerator
    {
        public int CreateTokenCount { get; private set; }

        public TimeSpan DefaultLifetime { get; set; } =
            TimeSpan.FromHours(1);

        public string CreateToken(
            string subject,
            IReadOnlyDictionary<string, string?> claims)
        {
            CreateTokenCount++;
            return "token-de-prueba";
        }

        public string CreateToken(HariaTokenDescriptor descriptor)
        {
            CreateTokenCount++;
            return "token-de-prueba";
        }

        public bool TryValidateToken(
            string token,
            out HariaTokenDescriptor? descriptor)
        {
            descriptor = null;
            return false;
        }
    }

    // ==========================================
    // UNIT OF WORK FALSO
    // ==========================================

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public IUsuarioRepository UsuarioRepository { get; }
        public IHariaPasswordHasher PasswordHasher { get; }
        public IHariaTokenGenerator TokenGenerator { get; }

        public FakeUnitOfWork(
            IUsuarioRepository usuarioRepository,
            IHariaPasswordHasher passwordHasher,
            IHariaTokenGenerator tokenGenerator)
        {
            UsuarioRepository = usuarioRepository;
            PasswordHasher = passwordHasher;
            TokenGenerator = tokenGenerator;
        }

        public IViajeRepository ViajeRepository => null!;
        public IResenaRepository ResenaRepository => null!;
        public INotificacionRepository NotificacionRepository => null!;
        public IVehiculoRepository VehiculoRepository => null!;
        public IReservaRepository ReservaRepository => null!;
        public IPagoRepository PagoRepository => null!;
        public IMensajeRepository MensajeRepository => null!;

        public Task BeginTransactionAsync(string? businessOperation = null) =>
            Task.CompletedTask;

        public Task CommitAsync() => Task.CompletedTask;

        public Task RollbackAsync() => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
