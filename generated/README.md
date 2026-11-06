# CarGoGen – Backend de CarGo

## 1. Descripción del proyecto

CarGo es una aplicación de coche compartido (estilo BlaBlaCar) que permite gestionar usuarios, vehículos, viajes, reservas, pagos, mensajes, reseñas y notificaciones.

Este proyecto contiene el backend desarrollado a partir del modelo de dominio diseñado en HARIA (TDM). Su implementación sigue una arquitectura limpia (*Clean Architecture*) organizada en capas para desacoplar las reglas de negocio, los servicios de aplicación, la infraestructura de datos y las pruebas unitarias.

El objetivo principal es implementar la gestión CRUD de las entidades, las operaciones personalizadas de negocio (`Custom` y `CustomTransaction`), las expresiones de filtrado LINQ y su correspondiente batería de pruebas automatizadas con xUnit.

---

## 2. Estructura general del proyecto

La solución está dividida en los siguientes proyectos:

```text
generated/
│
├── CarGoGen.Domain/        # Entidades, enumeraciones y objetos de valor
├── CarGoGen.Application/   # Contratos, servicios de aplicación y UseCases
├── CarGoGen.Infrastructure/# Persistencia con NHibernate, mapeos y pasarela simulada
├── CarGoGen.Tests/         # Pruebas unitarias y de integración con xUnit y Moq
└── InitializeDB/           # Semilla y configuración inicial de la base de datos
```

### 2.1. CarGoGen.Domain
Contiene el modelo de dominio generado a partir del TDM de HARIA:
- **Entidades:** `Usuario`, `Vehiculo`, `Viaje`, `Reserva`, `Pago`, `Mensaje`, `Resena` y `Notificacion`.
- **Enumeraciones:** `EnumMoneda`, `EstadoViaje`, `EstadoReserva`, `EstadoPago`, `EstadoVerificacion` y `EstadoResena`.
- **Objetos de valor (`ValueObject`):** `Dinero` y `Ubicacion`.
- **Relaciones:** asociaciones entre conductores, pasajeros, vehículos, reservas, notificaciones y pagos.

### 2.2. CarGoGen.Application
Contiene la lógica de negocio y los servicios del sistema:
- **Abstractions:** interfaces `IRepository<T>`, `IUnitOfWork`, `IPasarelaPago` y repositorios específicos (`INotificacionRepository`, `IReservaRepository`, etc.).
- **Services:** servicios parciales (`NotificacionService`, `ViajeService`, `ResenaService`, `UsuarioService`) donde se ubican las operaciones `Custom`.
- **UseCases:** casos de uso que coordinan operaciones complejas y transaccionales (`CustomTransaction`).

### 2.3. CarGoGen.Infrastructure
Contiene los aspectos técnicos y de infraestructura:
- Implementación de repositorios con NHibernate.
- Mapeos XML para la relación objeto-relacional (ORM).
- Implementación de la unidad de trabajo (`UnitOfWork`).
- Servicios externos, como la pasarela de pagos simulada (`PasarelaPagoSimulada`).

### 2.4. CarGoGen.Tests
Proyecto de pruebas unitarias e integración con xUnit y Moq. Valida las reglas de negocio, los permisos de usuario, la consistencia de estados y el manejo de excepciones (`ModelException`).

### 2.5. InitializeDB
Herramientas para la configuración del esquema, semillas de prueba (`SeedData.cs`) e inicialización de SQL Server con NHibernate.

---

## 3. Modelo TDM y Operaciones del Backend

### 3.1. Operaciones CRUD Generadas
Todas las entidades cuentan con las operaciones CRUD estándar:

| Operación | Descripción |
|---|---|
| `New` | Crea e instancia una nueva entidad. |
| `Modify` | Actualiza los datos de una entidad existente. |
| `Destroy` | Elimina lógicamente/físicamente una entidad. |
| `ReadOID` / `ReadAsync` | Obtiene una entidad por su OID/ID. |
| `ReadAll` / `ReadAllAsync` | Recupera el listado completo de entidades. |

---

### 3.2. Operaciones Personalizadas (`Custom`)

Las operaciones personalizadas implementan lógica de negocio específica:

| Entidad | Operación | Descripción |
|---|---|---|
| **Notificacion** | `marcarComoLeida` | Permite a un usuario marcar su notificación como leída (`Leida = true`), validando previamente la pertenencia de la misma. |
| **Reserva** | `rechazarPasajero` | Permite al conductor de un viaje rechazar una solicitud de reserva pendiente (`Estado = Rechazada`). |
| **Viaje** | `gestionarPlazas` | Ajusta el número de plazas disponibles de un viaje. |
| **Viaje** | `geolocalizarTrayecto` | Actualiza en tiempo real las coordenadas (latitud/longitud) del viaje en curso. |
| **Usuario** | `verificarDni` | Valida y asigna el DNI a un usuario para su verificación. |
| **Resena** | `moderarContenido` | Permite a un administrador aceptar, ocultar o rechazar la reseña de un viaje. |

---

### 3.3. Transacciones Personalizadas (`CustomTransaction`)

Coordinan múltiples cambios en el dominio o interacciones externas dentro de un contexto transaccional:

| Entidad / UseCase | Transacción | Descripción |
|---|---|---|
| **Reserva** | `cancelarReserva` | Permite al pasajero o conductor cancelar una reserva. Si la reserva estaba previamente `Aceptada`, la transacción devuelve automáticamente las plazas reservadas al `Viaje` asociado. |
| **Pago** | `realizarPago` | Procesa un pago en estado `Pendiente` a través de la pasarela de pago, almacena el `IdTransaccionPasarela` y actualiza el estado a `Completado`. |
| **Pago** | `reembolsarPago` | Procesa la devolución de un pago `Completado` llamando a la pasarela y actualizando el estado a `Reembolsado`. |

---

### 3.4. Filtros LINQ Implementados

El modelo TDM define filtros LINQ optimizados para consultas específicas:

* **`Viaje.readFilterByOrigenDestino`**: Filtra viajes por ciudad de origen y/o destino.
* **`Viaje.readFilterByFechaYPlazas`**: Busca viajes entre un rango de fechas con un número mínimo de plazas disponibles.
* **`Viaje.readFilterByPrecio`**: Filtra por un rango de precio (mínimo/máximo).
* **`Viaje.readFilterByPreferenciasYEstado`**: Filtra viajes publicados/en curso por preferencias de equipaje y mascotas.
* **`Viaje.readFilterViajesDisponibles`**: Obtiene viajes en estado `Publicado` con plazas vacantes a partir de una fecha dada.
* **`Viaje.readFilterByConductorVerificado`**: Devuelve viajes cuyo conductor tenga el estado de verificación `Verificado`.
* **`Reserva.readFilterByEstadoYFecha`**: Filtra reservas según su estado (`Pendiente`, `Aceptada`, etc.) en un período de tiempo.
* **`Usuario.readFilterByEmail`** y **`Usuario.readFilterByDni`**: Búsquedas exactas para autenticación e identificación.

---

## 4. Servicio e Integración de Pagos

La gestión de pagos sigue el patrón de inversión de dependencias:

* **`IPasarelaPago`** *(Application)*: Abstracción que define `RealizarPagoAsync` y `ReembolsarPagoAsync`.
* **`PasarelaPagoSimulada`** *(Infrastructure)*: Implementación en memoria mediante `ConcurrentDictionary` que genera códigos de transacción únicos y controla reembolsos duplicados.
* **`PagoUseCase`** *(Application)*: Maneja las transacciones complejas garantizando rollback en caso de fallo en la pasarela o inconsistencia en la entidad.

---

## 5. Pruebas Automatizadas (Suite xUnit)

El proyecto `CarGoGen.Tests` contiene **102 pruebas en total**:

* **101 Pruebas pasadas (100% éxito):**
  * `NotificationServiceTests`: Pruebas de marcaje como leída (éxito, notificación inexistente y usuario no autorizado).
  * `ViajeServiceReservaTests`: Pruebas para `rechazarPasajero` y `cancelarReserva` (incluyendo la devolución de plazas al viaje).
  * `PagoUseCaseTests`: Pruebas de integración simulada para pagos, reembolsos, errores de pasarela e importes.
  * Pruebas de Geolocalización, Moderación de Reseñas y Contratos del modelo TDM.
* **1 Prueba omitida (`[Fact(Skip = "...")]`)**:
  * `Generated_Crud_And_Associations_Should_Work_Against_Database`: Omitida intencionadamente en ejecuciones locales para no requerir un servidor SQL Server activo durante la ejecución de los tests unitarios.

### 5.1. Ejecución de las Pruebas

Para ejecutar la suite completa de pruebas unitarias:

```powershell
dotnet test
```

Para filtrar por una clase de prueba específica (ej. Reservas o Notificaciones):

```powershell
dotnet test --filter "FullyQualifiedName~ViajeServiceReservaTests"
dotnet test --filter "FullyQualifiedName~NotificationServiceTests"
dotnet test --filter "FullyQualifiedName~PagoUseCaseTests"
```

---

## 6. Compilación y Construcción

Para restaurar dependencias y compilar la solución completa:

```powershell
dotnet restore
dotnet build
```

Compilación por proyectos individuales:

```powershell
dotnet build .\CarGoGen.Domain\CarGoGen.Domain.csproj
dotnet build .\CarGoGen.Application\CarGoGen.Application.csproj
dotnet build .\CarGoGen.Infrastructure\CarGoGen.Infrastructure.csproj
dotnet build .\CarGoGen.Tests\CarGoGen.Tests.csproj
```

---

## 7. Consideraciones Finales

1. **Invariantes de Dominio**: Todas las operaciones personalizadas comprueban exhaustivamente los permisos de usuario y el estado previo de las entidades antes de realizar modificaciones. En caso de invalidez, se lanza una `ModelException`.
2. **Consistencia Transaccional**: La cancelación de reservas aceptadas restituye las plazas de forma atómica en la misma operación.
3. **Mantenimiento**: Los bloques protegidos (`PROTECTED REGION`) de HARIA garantizan que el código manual no se sobreescriba al revalidar el modelo TDM.