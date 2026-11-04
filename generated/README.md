# CarGoGen – Backend de CarGo

## 1. Descripción del proyecto

CarGo es una aplicación de coche compartido que permite gestionar usuarios, vehículos, viajes, reservas, pagos, mensajes, reseñas y notificaciones.

Este proyecto contiene el backend generado a partir del modelo de dominio diseñado en HARIA (TDM). Su implementación está organizada en diferentes capas para separar la lógica del dominio, los servicios de aplicación, el acceso a datos y las pruebas.

El objetivo principal es implementar las operaciones CRUD de las entidades, las operaciones personalizadas del sistema y sus correspondientes pruebas para comprobar su funcionamiento.

## 2. Estructura general del proyecto

La solución está dividida en los siguientes proyectos:

```text
generated/
│
├── CarGoGen.Domain/
│
├── CarGoGen.Application/
│
├── CarGoGen.Infrastructure/
│
├── CarGoGen.Tests/
│
└── InitializeDB/
```

### 2.1. CarGoGen.Domain

Contiene el modelo de dominio generado a partir del TDM de HARIA.

Incluye:

- **Entidades:** Usuario, Vehiculo, Viaje, Reserva, Pago, Mensaje, Resena y Notificacion.
- **Enumeraciones:** EnumMoneda, EstadoViaje, EstadoReserva, EstadoPago y EstadoVerificacion.
- **Objetos de valor:** Dinero y Ubicacion.
- **Relaciones:** asociaciones entre usuarios, vehículos, viajes, reservas, pagos y el resto de entidades.

Esta capa representa los conceptos y las relaciones principales del sistema.

### 2.2. CarGoGen.Application

Contiene la lógica de aplicación y las operaciones que ofrece el backend.

Se divide principalmente en:

- **Abstractions:** interfaces que definen los contratos de los repositorios, las unidades de trabajo y los servicios externos.
- **Services:** servicios de aplicación que implementan las operaciones CRUD y las operaciones personalizadas de tipo Custom.
- **UseCases:** casos de uso que contienen las operaciones de tipo CustomTransaction.

Los servicios de aplicación utilizan los repositorios para acceder a las entidades, mientras que los casos de uso coordinan operaciones que necesitan una gestión transaccional.

### 2.3. CarGoGen.Infrastructure

Contiene los detalles técnicos necesarios para ejecutar la aplicación.

Incluye:

- Repositorios que implementan el acceso a las entidades mediante NHibernate.
- Mapeos XML que relacionan las clases del dominio con las tablas de la base de datos.
- Implementación de la unidad de trabajo (UnitOfWork).
- Utilidades de infraestructura.
- Registro de dependencias.
- Implementaciones de servicios externos, como la pasarela de pago simulada.

Esta capa permite que Application utilice los servicios de persistencia y las implementaciones técnicas sin depender directamente de ellas.

### 2.4. CarGoGen.Tests

Contiene las pruebas automatizadas del backend mediante xUnit.

Se utilizan para comprobar el comportamiento de las operaciones personalizadas y validar que las respuestas y los estados de las entidades son los esperados.

Las pruebas pueden utilizar dependencias simuladas (*fakes*) para comprobar la lógica de aplicación sin necesitar una conexión real a la base de datos.

### 2.5. InitializeDB

Contiene los archivos de configuración y las herramientas relacionadas con la inicialización de la base de datos.

Entre ellos se encuentran:

- `appsettings.json`
- `hibernate.cfg.xml`

Estos archivos permiten configurar la conexión y el comportamiento de NHibernate.

---

## 3. Operaciones del backend

### 3.1. Operaciones CRUD

Las entidades del sistema disponen de las operaciones CRUD correspondientes:

| Operación | Descripción |
|---|---|
| New | Crea una nueva entidad. |
| Modify | Modifica los datos de una entidad existente. |
| Destroy | Elimina una entidad. |
| ReadOID | Recupera una entidad mediante su identificador. |
| ReadAll | Recupera todas las entidades de un tipo determinado. |

Estas operaciones son generadas a partir del modelo de dominio y se implementan mediante los servicios de aplicación y sus respectivos repositorios.

### 3.2. Operaciones Custom

Las operaciones Custom son funcionalidades específicas del sistema que no corresponden directamente a las operaciones CRUD.

Se implementan en los servicios de aplicación y permiten añadir reglas de negocio particulares.

Entre las operaciones definidas en el modelo se encuentran:

- `Usuario.verificarDni`
- `Viaje.gestionarPlazas`
- `Viaje.geolocalizarTrayecto`
- `Reserva.aceptarPasajero`
- `Resena.moderarContenido`

### 3.3. Operaciones CustomTransaction

Las operaciones CustomTransaction se implementan en la capa de UseCases.

Se utilizan cuando una operación necesita coordinar varias acciones dentro de una transacción de negocio.

En la entidad Pago se definen:

- `realizarPago`
- `reembolsarPago`

Estas operaciones comprueban el estado del pago, interactúan con la pasarela y actualizan la entidad de acuerdo con el resultado.

---

## 4. Implementación de pagos

Esta sección describe las operaciones desarrolladas para la gestión de pagos.

### 4.1. Componentes utilizados

La implementación utiliza los siguientes componentes:

| Componente | Proyecto | Responsabilidad |
|---|---|---|
| `IPasarelaPago` | Application | Define las operaciones disponibles de la pasarela. |
| `PasarelaPagoSimulada` | Infrastructure | Implementa la simulación de pagos y reembolsos. |
| `ExternalUtilRegistration` | Infrastructure | Registra la implementación de la pasarela. |
| `ExternalUtilsServiceCollectionExtensions` | Infrastructure | Registra las utilidades externas y `IUnitOfWorkUtils`. |
| `PagoUseCase` | Application | Coordina las operaciones transaccionales de Pago. |
| `Pago` | Domain | Representa la entidad que almacena la información del pago. |

### 4.2. Interfaz IPasarelaPago

La interfaz `IPasarelaPago`, situada en `CarGoGen.Application/Abstractions`, define dos operaciones:

- `RealizarPagoAsync`: recibe el identificador del pago y su importe, y devuelve un identificador de transacción.
- `ReembolsarPagoAsync`: recibe el identificador de transacción y ejecuta su reembolso.

De esta manera, la capa Application depende de una abstracción y no de una implementación concreta de la pasarela.

### 4.3. Pasarela de pago simulada

La clase `PasarelaPagoSimulada` se encuentra en:

```text
CarGoGen.Infrastructure/
└── Services/
    └── PasarelaPagoSimulada.cs
```

Esta clase implementa `IPasarelaPago` y simula el comportamiento de una pasarela externa.

Al realizar un pago:

1. Comprueba que el identificador sea válido.
2. Comprueba que el importe sea positivo y válido.
3. Genera un identificador único para la transacción.
4. Registra la transacción como reembolsable.
5. Devuelve el identificador generado.

Al realizar un reembolso:

1. Comprueba que el identificador de transacción sea válido.
2. Comprueba que la transacción exista.
3. Comprueba que todavía no haya sido reembolsada.
4. Marca la transacción como no reembolsable.

Las transacciones se almacenan mediante un `ConcurrentDictionary`, evitando que dos solicitudes concurrentes puedan reembolsar la misma transacción.

La pasarela se registra como Singleton para mantener la misma instancia y conservar las transacciones mientras el proceso esté en ejecución.

**Limitación:** se trata de una simulación local. No se realizan cargos ni devoluciones de dinero reales. Los datos de las transacciones se almacenan en memoria y se pierden al reiniciar el proceso.

### 4.4. Registro de dependencias

La clase `ExternalUtilRegistration` registra la implementación de la pasarela:

```csharp
services.AddSingleton<IPasarelaPago, PasarelaPagoSimulada>();
```

Por su parte, `ExternalUtilsServiceCollectionExtensions` registra `IUnitOfWorkUtils` y ejecuta el registro manual de los servicios externos.

El caso de uso recibe `IUnitOfWorkUtils` mediante su constructor para poder resolver la implementación de `IPasarelaPago` sin depender directamente de Infrastructure.

### 4.5. Operación realizarPago

La operación `RealizarPagoAsync(int p_oid)` permite completar un pago pendiente.

Su funcionamiento es el siguiente:

1. Inicia una transacción mediante `BeginTransactionAsync`.
2. Recupera el pago mediante su identificador.
3. Comprueba que el pago existe.
4. Comprueba que su estado sea `Pendiente`.
5. Obtiene la implementación de `IPasarelaPago`.
6. Ejecuta el pago utilizando el identificador y el importe de la entidad.
7. Recibe el identificador de transacción.
8. Guarda dicho identificador en `IdTransaccionPasarela`.
9. Cambia el estado del pago a `Completado`.
10. Guarda los cambios y confirma la transacción mediante `CommitAsync`.

Si se produce una excepción, se ejecuta `RollbackAsync` y se lanza una `ModelException`.

### 4.6. Operación reembolsarPago

La operación `ReembolsarPagoAsync(int p_oid)` permite reembolsar un pago que ya se ha completado.

Su funcionamiento es el siguiente:

1. Inicia una transacción.
2. Recupera el pago mediante su identificador.
3. Comprueba que el pago existe.
4. Comprueba que su estado sea `Completado`.
5. Comprueba que tenga un identificador de transacción asociado.
6. Obtiene la implementación de `IPasarelaPago`.
7. Ejecuta el reembolso utilizando el identificador de transacción almacenado.
8. Cambia el estado del pago a `Reembolsado`.
9. Guarda los cambios y confirma la transacción.

Si el pago no existe, no está completado, no tiene una transacción asociada o la pasarela rechaza el reembolso, se ejecuta el rollback y se lanza una `ModelException`.

---

## 5. Pruebas de las operaciones de pago

Las pruebas de las operaciones de Pago se encuentran en:

```text
CarGoGen.Tests/
└── UseCases/
    └── PagoUseCaseTests.cs
```

Se utiliza xUnit para verificar el comportamiento de los casos de uso.

Para evitar depender de una conexión real a SQL Server, se han creado implementaciones simuladas de las dependencias:

- `FakePagoRepository`: simula la lectura y el almacenamiento de pagos.
- `FakeUnitOfWork`: simula el inicio, la confirmación y la reversión de transacciones.
- `FakeUnitOfWorkUtils`: permite resolver la pasarela simulada durante las pruebas.
- `PasarelaQueFalla`: simula un error durante la realización de un pago.
- `PasarelaCapturadora`: permite comprobar los datos enviados a la pasarela.

### 5.1. Casos de prueba implementados

| Nº | Prueba | Resultado esperado |
|---:|---|---|
| 1 | Realizar un pago pendiente | El pago pasa a Completado y recibe un identificador de transacción. |
| 2 | Realizar un pago inexistente | Se produce una excepción. |
| 3 | Realizar un pago ya completado | Se impide repetir el pago. |
| 4 | Reembolsar un pago completado | El pago pasa a Reembolsado. |
| 5 | Reembolsar un pago pendiente | Se produce una excepción. |
| 6 | Reembolsar un pago sin transacción | Se produce una excepción. |
| 7 | Reembolsar dos veces la misma transacción | Se impide el segundo reembolso. |
| 8 | Error de la pasarela durante el pago | Se ejecuta el rollback y no se guarda el pago. |
| 9 | Comprobar el importe enviado a la pasarela | Se envían el identificador y el importe correctos. |

**Resultado obtenido:** 9 pruebas correctas, 0 fallidas y 0 omitidas.

Estas pruebas validan la lógica de los casos de uso y las llamadas a las operaciones transaccionales. Al utilizar repositorios simulados, no verifican la persistencia real mediante NHibernate ni la ejecución de transacciones en SQL Server.

### 5.2. Ejecución de las pruebas

Desde la carpeta `generated`, ejecutar:

```powershell
dotnet test .\CarGoGen.Tests\CarGoGen.Tests.csproj --filter "FullyQualifiedName~PagoUseCaseTests"
```

Para ejecutar todas las pruebas del proyecto:

```powershell
dotnet test .\CarGoGen.Tests\CarGoGen.Tests.csproj
```

---

## 6. Compilación del proyecto

Para restaurar las dependencias:

```powershell
dotnet restore
```

Para compilar la solución:

```powershell
dotnet build
```

También se pueden compilar individualmente los proyectos:

```powershell
dotnet build .\CarGoGen.Domain\CarGoGen.Domain.csproj

dotnet build .\CarGoGen.Application\CarGoGen.Application.csproj

dotnet build .\CarGoGen.Infrastructure\CarGoGen.Infrastructure.csproj

dotnet build .\CarGoGen.Tests\CarGoGen.Tests.csproj
```

## 7. Consideraciones finales

El backend separa el modelo de dominio, la lógica de aplicación, la infraestructura y las pruebas.

La implementación de pagos utiliza una interfaz para desacoplar los casos de uso de la pasarela concreta, permitiendo sustituir la simulación por una integración real en el futuro.

Las pruebas unitarias de Pago permiten comprobar las reglas de negocio y los escenarios de error sin necesitar una base de datos.

Para comprobar el comportamiento de persistencia y las operaciones CRUD contra SQL Server será necesario ejecutar las correspondientes pruebas de integración con una base de datos correctamente configurada.