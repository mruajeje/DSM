# CarGoGen – Backend de CarGo

## 1. Descripción del proyecto

CarGo es una aplicación de coche compartido (estilo BlaBlaCar) que permite gestionar usuarios, vehículos, viajes, reservas, pagos, mensajes, reseñas y notificaciones.

Este proyecto contiene el backend desarrollado a partir del modelo de dominio diseñado en HARIA (TDM). Su implementación sigue una arquitectura limpia (*Clean Architecture*) organizada en capas para desacoplar las reglas de negocio, los servicios de aplicación, la infraestructura de datos y las pruebas unitarias.

---

## 2. Guía de Preparación e Inicialización (Pasos para Integración / Frontend)

Para poder consumir la API o conectar el Frontend con una base de datos local funcional, es necesario seguir este flujo de inicialización previa:

### Paso 1: Generar e Inicializar la Base de Datos Local (SQLite)
Antes de arrancar la aplicación o probar endpoints, debemos instanciar y poblar el esquema local con datos iniciales:

1. Acreditar que los archivos `hibernate.cfg.xml` y `appsettings.json` en `generated/InitializeDB` apuntan al archivo SQLite `CarGo.db`.
2. Ejecutar la compilación y semilla desde la terminal o mediante depuración en VS Code:
   ```powershell
   cd generated/InitializeDB
   dotnet run
   ```
3. Verificar que se ha creado correctamente el archivo de base de datos `CarGoGenNHibernate.db` en la carpeta `InitializeDB`.

### Paso 2: Compilación de la Solución Backend
Asegurarse de que todos los proyectos compilen correctamente sin errores de dependencias:
```powershell
dotnet restore
dotnet build
```

### Paso 3: Ejecución de la Suite de Pruebas
Comprobar que todas las reglas de negocio y restricciones transaccionales son válidas antes de conectar capas externas:
```powershell
dotnet test
```

---

## 3. Estructura general del proyecto

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

### 3.1. CarGoGen.Domain
Contiene el modelo de dominio generado a partir del TDM de HARIA:
- **Entidades:** `Usuario`, `Vehiculo`, `Viaje`, `Reserva`, `Pago`, `Mensaje`, `Resena` y `Notificacion`.
- **Enumeraciones:** `EnumMoneda`, `EstadoViaje`, `EstadoReserva`, `EstadoPago`, `EstadoVerificacion` y `EstadoResena`.
- **Objetos de valor (`ValueObject`):** `Dinero` y `Ubicacion`.
- **Relaciones:** asociaciones entre conductores, pasajeros, vehículos, reservas, notificaciones y pagos.

### 3.2. CarGoGen.Application
Contiene la lógica de negocio y los servicios del sistema:
- **Abstractions:** interfaces `IRepository<T>`, `IUnitOfWork`, `IPasarelaPago` y repositorios específicos.
- **Services:** servicios parciales (`NotificacionService`, `ViajeService`, `ResenaService`, `UsuarioService`) con operaciones `Custom`.
- **UseCases:** casos de uso que coordinan operaciones transaccionales (`CustomTransaction`).

### 3.3. CarGoGen.Infrastructure
- Implementación de repositorios con NHibernate y mapeos XML.
- Servicios externos simulados (`PasarelaPagoSimulada`).

### 3.4. CarGoGen.Tests
Pruebas unitarias e integración con xUnit y Moq.

### 3.5. InitializeDB
Inicialización de esquemas, configuración del ORM y carga de semillas de datos de prueba (`SeedData.cs`).

---

## 4. Problemas Encontrados y Soluciones Tecnológicas

Durante el desarrollo e inicialización del proyecto surgieron diversas incidencias técnicas que se resolvieron del siguiente modo:

### 4.1. Error de Autenticación en SQL Server (Error 18456)
* **Problema:** Al intentar ejecutar `InitializeDB` contra SQL Server Express, la consola de depuración devolvía un error de inicio de sesión del usuario `nhibernateUser` (Error 18456), impidiendo crear las tablas. Además, la base de datos se creaba en el servidor SQL Server y no como archivo en el proyecto.
* **Solución:** Se configuró la cadena de conexión para utilizar Autenticación de Windows (`Integrated Security=True`) tanto en `appsettings.json` como en `hibernate.cfg.xml`:
  ```text
  Server=(local)\sqlexpress;Database=CarGoGenNHibernate;TrustServerCertificate=True;Integrated Security=True
  ```

### 4.2. Migración y Ubicación del Archivo SQLite (`.db`)
* **Problema 1 (Ubicación del archivo):** Al cambiar a SQLite, el ejecutable generaba el archivo `CarGo.db` dentro de la carpeta interna de compilación (`bin/Debug/net9.0/`) y no en la carpeta visible del proyecto `InitializeDB`.
* **Problema 2 (Sobrescritura por el código generado):** Al ejecutar el debug, la llamada a `CreateDB.Create()` volvía a escribir la configuración por defecto de SQL Server sobreescribiendo los cambios de SQLite.
* **Solución aplicada:**
  1. Se instaló el paquete NuGet en `InitializeDB`:
     ```powershell
     dotnet add package System.Data.SQLite.Core
     ```
  2. En `hibernate.cfg.xml` y `appsettings.json` se estableció el driver y dialecto de SQLite:
     ```xml
    <property name="dialect">NHibernate.Dialect.SQLiteDialect</property>
    <property name="connection.driver_class">NHibernate.Driver.SQLite20Driver</property>
    <property name="connection.connection_string">Data Source=../../../CarGo.db</property>
     ```
  3. En `Program.cs` (dentro de la región protegida `initializeDatabase`), se modificó la obtención de la cadena para leer de la configuración sin ejecutar la recreación del XML:
     ```csharp
     var cs = configuration["ConnectionStrings:DbConnection"]!;
     ```
  4. En `.vscode/launch.json` se fijó el directorio de trabajo actual (`cwd`) para asegurar que el archivo `.db` se ubique en la raíz de `InitializeDB`:
     ```json
     "cwd": "${workspaceFolder}/generated/InitializeDB"
     ```

---

## 5. Modelo TDM y Operaciones del Backend

### 5.1. Operaciones Personalizadas (`Custom`) y Transacciones (`CustomTransaction`)

| Entidad / UseCase | Operación | Tipo | Descripción |
|---|---|---|---|
| **Notificacion** | `marcarComoLeida` | Custom | Marca como leída una notificación propia. |
| **Reserva** | `rechazarPasajero` | Custom | Cancela/rechaza solicitudes de reserva pendientes. |
| **Viaje** | `gestionarPlazas` | Custom | Reajusta la disponibilidad de plazas. |
| **Viaje** | `geolocalizarTrayecto` | Custom | Actualiza coordenadas GPS del viaje en curso. |
| **Reserva** | `cancelarReserva` | CustomTransaction | Devuelve automáticamente las plazas al viaje si la reserva aceptada se cancela. |
| **Pago** | `realizarPago` | CustomTransaction | Procesa pagos pendientes a través de la pasarela y actualiza estados. |
| **Pago** | `reembolsarPago` | CustomTransaction | Procesa devoluciones de pagos completados. |

---

## 6. Pruebas Automatizadas (Suite xUnit)

El proyecto `CarGoGen.Tests` cuenta con **102 pruebas en total**:
- **101 Pruebas pasadas con éxito.**
- **1 Prueba omitida intencionadamente** (`Generated_Crud_And_Associations_Should_Work_Against_Database`), reservada para entornos con base de datos de integración.

Ejecución de filtros de prueba:
```powershell
dotnet test --filter "FullyQualifiedName~ViajeServiceReservaTests"
dotnet test --filter "FullyQualifiedName~PagoUseCaseTests"
```

---

## 7. Consideraciones Finales

1. **Invariantes de Dominio**: Todas las operaciones comprueban permisos y estados antes de alterar datos (`ModelException`).
2. **Protección HARIA**: Las regiones `PROTECTED REGION` conservan las modificaciones hechas en `Program.cs` e infraestructura ante posibles regeneraciones del modelo TDM.




## Arrancar la aplicación web (CarGo.Web)

### Requisitos
- .NET 9 SDK instalado (`dotnet --version` debe mostrar 9.x).
- Backend compilado: `dotnet build generated\CarGo.sln` (desde la raíz del proyecto, donde está `carGo.tdm`).

### Paso 1: crear la base de datos con datos de demo
Desde `generated\InitializeDB`:

```powershell
dotnet run -- --reset
```

- Crea `generated\InitializeDB\CarGo.db` y la rellena con datos de demo: 4 usuarios, 2 vehículos, 6 viajes, 2 reservas y 1 notificación.
- `--reset` **recrea la base y borra los datos anteriores**. Antes hace una copia en `database\backups`.
- Sin `--reset`, si la base ya existe se conservan los datos y no se vuelve a sembrar.

### Paso 2: arrancar la web
Desde la raíz del proyecto:

```powershell
dotnet run --project generated\CarGo.Web\CarGo.Web.csproj
```

La consola muestra la URL (`Now listening on: ...`). Ábrela en el navegador y para parar usa `Ctrl+C`.
La web lee la base desde `ConnectionStrings:DbConnection` en `generated\CarGo.Web\appsettings.Haria.json`, que debe apuntar a `../InitializeDB/CarGo.db`.

### Cuentas de demo
| Usuario | Contraseña | Notas |
|---|---|---|
| admin@cargo.com | Admin123! | Administrador |
| ana@cargo.com | Ana12345! | Conductora verificada |
| luis@cargo.com | Luis12345! | Conductor verificado |
| marta@cargo.com | Marta12345! | Pasajera, verificación pendiente |

### Si regeneras desde HARIA
- La configuración de SQLite y el nombre del proyecto (`CarGo`) están en `haria.config.json`, en la raíz. **No lo borres y no edites a mano `appsettings.json` ni `hibernate.cfg.xml`**: HARIA los sobrescribe en cada generación.
- El código manual vive en las carpetas `manual` y en las regiones `PROTECTED REGION` (por ejemplo, en `Program.cs` y `SeedData.cs` de `InitializeDB`), que HARIA conserva.
- Antes de un `--reset` o de cambiar el modelo, haz una copia: `powershell -ExecutionPolicy Bypass -File .\scripts\backup-db.ps1`.