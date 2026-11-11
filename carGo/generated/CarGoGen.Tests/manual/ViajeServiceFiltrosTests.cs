using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Services;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;
using CarGoGen.Domain.ValueObjects;
 
namespace CarGoGen.Tests.Services;
 
// IMPORTANTE: estas pruebas NO ejecutan las consultas contra una base de datos.
//  - Parte 1: comprueba que ViajeService delega en el repositorio con los parametros correctos.
//  - Parte 2: comprueba la LOGICA de cada filtro aplicando una copia de la expresion sobre una lista
//    en memoria (LINQ to Objects). Si cambiais el filtro en carGo.tdm, actualizad tambien la copia de aqui.
//    No comprueban que NHibernate sepa traducir la expresion a SQL.
public sealed class ViajeServiceFiltrosTests
{

    // PARTE 1: el servicio delega en el repositorio
    [Fact]
    public async Task ReadFilterByOrigenDestino_DelegaEnElRepositorio()
    {
        var repo = new FakeViajeRepo { Resultado = new List<Viaje> { new() { Id = 7 } } };
        var service = new ViajeService(new FakeUnitOfWork(repo));
 
        var resultado = await service.ReadFilterByOrigenDestinoAsync("Valencia", "Madrid");
 
        Assert.Equal(nameof(IViajeRepository.ReadFilterByOrigenDestinoAsync), repo.UltimoMetodo);
        Assert.Equal(new object[] { "Valencia", "Madrid" }, repo.UltimosArgumentos);
        Assert.Same(repo.Resultado, resultado);
    }
 
    [Fact]
    public async Task ReadFilterByFechaYPlazas_DelegaEnElRepositorio()
    {
        var repo = new FakeViajeRepo { Resultado = new List<Viaje> { new() { Id = 8 } } };
        var service = new ViajeService(new FakeUnitOfWork(repo));
        var desde = new DateTime(2026, 12, 1);
        var hasta = new DateTime(2026, 12, 2);
 
        var resultado = await service.ReadFilterByFechaYPlazasAsync(desde, hasta, 2);
 
        Assert.Equal(nameof(IViajeRepository.ReadFilterByFechaYPlazasAsync), repo.UltimoMetodo);
        Assert.Equal(new object[] { desde, hasta, 2 }, repo.UltimosArgumentos);
        Assert.Same(repo.Resultado, resultado);
    }
 
    [Fact]
    public async Task ReadFilterByPrecio_DelegaEnElRepositorio()
    {
        var repo = new FakeViajeRepo { Resultado = new List<Viaje> { new() { Id = 9 } } };
        var service = new ViajeService(new FakeUnitOfWork(repo));
 
        var resultado = await service.ReadFilterByPrecioAsync(10.5, 40.0);
 
        Assert.Equal(nameof(IViajeRepository.ReadFilterByPrecioAsync), repo.UltimoMetodo);
        Assert.Equal(new object[] { 10.5, 40.0 }, repo.UltimosArgumentos);
        Assert.Same(repo.Resultado, resultado);
    }
 
    [Fact]
    public async Task ReadFilterByPreferenciasYEstado_DelegaEnElRepositorio()
    {
        var repo = new FakeViajeRepo { Resultado = new List<Viaje> { new() { Id = 10 } } };
        var service = new ViajeService(new FakeUnitOfWork(repo));
 
        var resultado = await service.ReadFilterByPreferenciasYEstadoAsync(true, false);
 
        Assert.Equal(nameof(IViajeRepository.ReadFilterByPreferenciasYEstadoAsync), repo.UltimoMetodo);
        Assert.Equal(new object[] { true, false }, repo.UltimosArgumentos);
        Assert.Same(repo.Resultado, resultado);
    }
 

    // PARTE 2: logica de cada filtro (copias de las expresiones del TDM)
 
    // Copia de readFilterByOrigenDestino.
    // CHICAS: en LINQ to Objects "null" no da problemas por el cortocircuito de ||, pero en NHibernate
    // si podria fallar. Esta prueba NO detecta ese caso: usad cadena vacia, no null.
    private static Func<Viaje, bool> OrigenDestino(string p_origen, string p_destino) =>
        entity => (string.IsNullOrWhiteSpace(p_origen) || entity.Origen.Ciudad.ToLower() == p_origen.Trim().ToLower())
               && (string.IsNullOrWhiteSpace(p_destino) || entity.Destino.Ciudad.ToLower() == p_destino.Trim().ToLower());
 
    // Copia de readFilterByFechaYPlazas
    private static Func<Viaje, bool> FechaYPlazas(DateTime p_desde, DateTime p_hasta, int p_plazasMinimas) =>
        entity => entity.FechaHoraSalida >= p_desde && entity.FechaHoraSalida < p_hasta && entity.NumPlazasDisponibles >= p_plazasMinimas;
 
    // Copia de readFilterByPrecio
    private static Func<Viaje, bool> Precio(double p_precioMinimo, double p_precioMaximo) =>
        entity => entity.Precio.Cantidad >= p_precioMinimo && entity.Precio.Cantidad <= p_precioMaximo;
 
    // Copia de readFilterByPreferenciasYEstado
    private static Func<Viaje, bool> PreferenciasYEstado(bool p_permiteMascotas, bool p_permiteEquipaje) =>
        entity => (!p_permiteMascotas || entity.PermiteMascotas)
               && (!p_permiteEquipaje || entity.PermiteEquipaje)
               && (entity.Estado == EstadoViaje.Publicado || entity.Estado == EstadoViaje.EnCurso);
 
    // ---------- Origen y destino ----------
 
    [Fact]
    public void OrigenDestino_SoloOrigen_FiltraPorOrigen() =>
        Assert.Equal(new[] { 1, 2, 5 }, Ids(OrigenDestino("Valencia", "")));
 
    [Fact]
    public void OrigenDestino_SoloDestino_FiltraPorDestino() =>
        Assert.Equal(new[] { 1, 3, 4, 5 }, Ids(OrigenDestino("", "Madrid")));
 
    [Fact]
    public void OrigenDestino_OrigenYDestino_CumpleAmbos() =>
        Assert.Equal(new[] { 1, 5 }, Ids(OrigenDestino("Valencia", "Madrid")));
 
    [Theory]
    [InlineData("valencia")]
    [InlineData("VALENCIA")]
    [InlineData("  Valencia  ")]
    public void OrigenDestino_IgnoraMayusculasYEspacios(string origen) =>
        Assert.Equal(new[] { 1, 2, 5 }, Ids(OrigenDestino(origen, "")));
 
    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    public void OrigenDestino_SinCriterios_DevuelveTodos(string origen, string destino) =>
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, Ids(OrigenDestino(origen, destino)));
 
    [Theory]
    [InlineData("Valen")]       // la coincidencia es exacta, no parcial
    [InlineData("Granada")]     // ciudad que no existe
    public void OrigenDestino_SinCoincidencias_DevuelveVacio(string origen) =>
        Assert.Empty(Ids(OrigenDestino(origen, "")));
 
    [Fact]
    public void OrigenDestino_CombinacionInexistente_DevuelveVacio() =>
        Assert.Empty(Ids(OrigenDestino("Sevilla", "Barcelona")));
 
    // ---------- Fecha y plazas ----------
 
    [Fact]
    public void FechaYPlazas_RangoDeUnDia_FiltraPorFechaYPlazas() =>
        // El viaje 3 sale justo a las 00:00 del 2 de diciembre: el limite "hasta" es exclusivo.
        Assert.Equal(new[] { 1, 2 }, Ids(FechaYPlazas(new DateTime(2026, 12, 1), new DateTime(2026, 12, 2), 1)));
 
    [Fact]
    public void FechaYPlazas_ExigeAlMenosLasPlazasIndicadas() =>
        Assert.Equal(new[] { 1 }, Ids(FechaYPlazas(new DateTime(2026, 12, 1), new DateTime(2026, 12, 2), 2)));
 
    [Fact]
    public void FechaYPlazas_LimiteDesdeEsInclusivo() =>
        Assert.Equal(new[] { 1 }, Ids(FechaYPlazas(new DateTime(2026, 12, 1, 8, 0, 0), new DateTime(2026, 12, 1, 9, 0, 0), 1)));
 
    [Fact]
    public void FechaYPlazas_LimiteHastaEsExclusivo() =>
        Assert.Empty(Ids(FechaYPlazas(new DateTime(2026, 12, 1, 7, 0, 0), new DateTime(2026, 12, 1, 8, 0, 0), 1)));
 
    [Fact]
    public void FechaYPlazas_ConCeroPlazas_IncluyeViajesSinPlazasLibres() =>
        Assert.Equal(new[] { 3, 4 }, Ids(FechaYPlazas(new DateTime(2026, 12, 2), new DateTime(2026, 12, 3), 0)));
 
    [Fact]
    public void FechaYPlazas_SinViajesEnElRango_DevuelveVacio() =>
        Assert.Empty(Ids(FechaYPlazas(new DateTime(2027, 1, 1), new DateTime(2027, 1, 2), 0)));
 
    // ---------- Precio ----------
 
    [Fact]
    public void Precio_RangoInclusivoPorAmbosLados() =>
        Assert.Equal(new[] { 1, 2, 5 }, Ids(Precio(15, 20)));
 
    [Fact]
    public void Precio_MinimoIgualAMaximo_DevuelveLosDeEsePrecio() =>
        Assert.Equal(new[] { 1, 5 }, Ids(Precio(20, 20)));
 
    [Fact]
    public void Precio_RangoAmplio_DevuelveTodos() =>
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, Ids(Precio(0, 1000)));
 
    [Theory]
    [InlineData(31, 49)]    // hueco entre precios existentes
    [InlineData(100, 200)]  // por encima de todos
    [InlineData(50, 10)]    // minimo mayor que maximo
    public void Precio_SinCoincidencias_DevuelveVacio(double minimo, double maximo) =>
        Assert.Empty(Ids(Precio(minimo, maximo)));
 
    // ---------- Preferencias y estado ----------
 
    [Fact]
    public void PreferenciasYEstado_SinExigencias_SoloPublicadosYEnCurso() =>
        // Excluye el viaje 4 (Finalizado) y el 5 (Cancelado).
        Assert.Equal(new[] { 1, 2, 3 }, Ids(PreferenciasYEstado(false, false)));
 
    [Fact]
    public void PreferenciasYEstado_ExigeMascotas() =>
        Assert.Equal(new[] { 1, 3 }, Ids(PreferenciasYEstado(true, false)));
 
    [Fact]
    public void PreferenciasYEstado_ExigeEquipaje() =>
        Assert.Equal(new[] { 1, 2 }, Ids(PreferenciasYEstado(false, true)));
 
    [Fact]
    public void PreferenciasYEstado_ExigeMascotasYEquipaje() =>
        Assert.Equal(new[] { 1 }, Ids(PreferenciasYEstado(true, true)));
 
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void PreferenciasYEstado_NuncaDevuelveViajesFinalizadosOCancelados(bool mascotas, bool equipaje)
    {
        var viajes = Datos().Where(PreferenciasYEstado(mascotas, equipaje)).ToList();
 
        Assert.DoesNotContain(viajes, v => v.Estado == EstadoViaje.Finalizado);
        Assert.DoesNotContain(viajes, v => v.Estado == EstadoViaje.Cancelado);
    }
 

    // DATOS Y HELPERS
    private static int[] Ids(Func<Viaje, bool> filtro) =>
        Datos().Where(filtro).Select(v => v.Id).OrderBy(id => id).ToArray();
 
    private static List<Viaje> Datos() => new()
    {
        Crear(1, "Valencia", "Madrid",    new DateTime(2026, 12, 1, 8, 0, 0),  plazas: 3, precio: 20, mascotas: true,  equipaje: true,  EstadoViaje.Publicado),
        Crear(2, "Valencia", "Barcelona", new DateTime(2026, 12, 1, 18, 0, 0), plazas: 1, precio: 15, mascotas: false, equipaje: true,  EstadoViaje.EnCurso),
        Crear(3, "Alicante", "Madrid",    new DateTime(2026, 12, 2, 0, 0, 0),  plazas: 4, precio: 30, mascotas: true,  equipaje: false, EstadoViaje.Publicado),
        Crear(4, "Sevilla",  "Madrid",    new DateTime(2026, 12, 2, 10, 0, 0), plazas: 0, precio: 50, mascotas: true,  equipaje: true,  EstadoViaje.Finalizado),
        Crear(5, "Valencia", "Madrid",    new DateTime(2026, 12, 3, 9, 0, 0),  plazas: 2, precio: 20, mascotas: false, equipaje: false, EstadoViaje.Cancelado),
    };
 
    private static Viaje Crear(
        int id, string origen, string destino, DateTime salida,
        int plazas, double precio, bool mascotas, bool equipaje, EstadoViaje estado) =>
        new()
        {
            Id = id,
            Origen = new Ubicacion { Ciudad = origen },
            Destino = new Ubicacion { Ciudad = destino },
            FechaHoraSalida = salida,
            NumPlazasDisponibles = plazas,
            Precio = new Dinero { Cantidad = precio },
            PermiteMascotas = mascotas,
            PermiteEquipaje = equipaje,
            Estado = estado,
        };
 

    // FAKES
    private sealed class FakeViajeRepo : IViajeRepository
    {
        public IReadOnlyList<Viaje> Resultado { get; set; } = new List<Viaje>();
        public string? UltimoMetodo { get; private set; }
        public object[]? UltimosArgumentos { get; private set; }
 
        private Task<IReadOnlyList<Viaje>> Registrar(string metodo, params object[] argumentos)
        {
            UltimoMetodo = metodo;
            UltimosArgumentos = argumentos;
            return Task.FromResult(Resultado);
        }
 
        public Task<IReadOnlyList<Viaje>> ReadFilterByOrigenDestinoAsync(string p_origen, string p_destino) =>
            Registrar(nameof(ReadFilterByOrigenDestinoAsync), p_origen, p_destino);
 
        public Task<IReadOnlyList<Viaje>> ReadFilterByFechaYPlazasAsync(DateTime p_desde, DateTime p_hasta, int p_plazasMinimas) =>
            Registrar(nameof(ReadFilterByFechaYPlazasAsync), p_desde, p_hasta, p_plazasMinimas);
 
        public Task<IReadOnlyList<Viaje>> ReadFilterByPrecioAsync(double p_precioMinimo, double p_precioMaximo) =>
            Registrar(nameof(ReadFilterByPrecioAsync), p_precioMinimo, p_precioMaximo);
 
        public Task<IReadOnlyList<Viaje>> ReadFilterByPreferenciasYEstadoAsync(bool p_permiteMascotas, bool p_permiteEquipaje) =>
            Registrar(nameof(ReadFilterByPreferenciasYEstadoAsync), p_permiteMascotas, p_permiteEquipaje);

        public Task<IReadOnlyList<Viaje>> ReadFilterViajesDisponiblesAsync(DateTime p_fechaMinima) =>
            Registrar(nameof(ReadFilterViajesDisponiblesAsync), p_fechaMinima);

        public Task<IReadOnlyList<Viaje>> ReadFilterByConductorVerificadoAsync() =>
            Registrar(nameof(ReadFilterByConductorVerificadoAsync));
 
        public Task<IReadOnlyList<Viaje>> ReadAllAsync() => throw new NotImplementedException();
        public Task<Viaje?> ReadAsync(object id) => throw new NotImplementedException();
        public Task SaveAsync(Viaje entity) => throw new NotImplementedException();
        public Task DeleteAsync(Viaje entity) => throw new NotImplementedException();
    }
 
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        private readonly FakeViajeRepo _viajes;
 
        public FakeUnitOfWork(FakeViajeRepo viajes) => _viajes = viajes;
 
        public IViajeRepository ViajeRepository => _viajes;
 
        public IUsuarioRepository UsuarioRepository => null!;
        public IVehiculoRepository VehiculoRepository => null!;
        public IReservaRepository ReservaRepository => null!;
        public IPagoRepository PagoRepository => null!;
        public IMensajeRepository MensajeRepository => null!;
        public IResenaRepository ResenaRepository => null!;
        public INotificacionRepository NotificacionRepository => null!;
        public IHariaPasswordHasher PasswordHasher => null!;
        public IHariaTokenGenerator TokenGenerator => null!;
 
        public Task BeginTransactionAsync(string? businessOperation = null) => Task.CompletedTask;
        public Task CommitAsync() => Task.CompletedTask;
        public Task RollbackAsync() => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}