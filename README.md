# 🏥 Clínica San Salud - API RESTful (.NET 10)

Bienvenido al repositorio del proyecto **Clínica San Salud API**. Esta es una aplicación de referencia desarrollada en **.NET 10 (C#)** pensada con fines educativos y profesionales para estudiantes de **Prácticas Profesionalizantes I** y materias afines a la Ingeniería de Software.

El sistema implementa una **API RESTful** para la gestión integral de turnos médicos, médicos y pacientes, aplicando buenas prácticas de arquitectura, desacoplamiento y diseño de software.

---

## 📑 Tabla de Contenidos
1. [¿Qué hace la aplicación?](#-1-qué-hace-la-aplicación)
2. [Arquitectura y Estructura del Proyecto](#-2-arquitectura-y-estructura-del-proyecto)
3. [Requisitos Previos y Cómo Ejecutar](#-3-requisitos-previos-y-cómo-ejecutar)
4. [Documentación de API y Contratos (Scalar & OpenAPI)](#-4-documentación-de-api-y-contratos-scalar--openapi)
5. [Colección de Bruno para Pruebas (QA / Estudiantes)](#-5-colección-de-bruno-para-pruebas-qa--estudiantes)
6. [Conceptos Clave para el Aprendizaje](#-6-conceptos-clave-para-el-aprendizaje)
7. [Pruebas Unitarias Automatizadas (xUnit & Moq)](#-7-pruebas-unitarias-automatizadas-xunit--moq)
8. [IA Asistida: Memoria de Contexto e Indexación Semántica (AGENTS.md + Serena)](#-8-ia-asistida-memoria-de-contexto-e-indexación-semántica-agentsmd--serena)

---

## 🩺 1. ¿Qué hace la aplicación?

La **API de Clínica San Salud** permite realizar operaciones CRUD (Crear, Leer, Actualizar y Eliminar) sobre tres recursos clave del dominio clínico:

- **Médicos**: Registro de profesionales de la salud con su nombre, especialidad y matrícula (esta última debe ser única).
- **Pacientes**: Registro de pacientes con su nombre, DNI, obra social/cobertura y fecha de nacimiento.
- **Turnos Médicos**: Reserva y gestión de turnos asignados a un paciente y a un médico específico en una fecha/hora dada, validando reglas de negocio complejas (como evitar solapamientos de horarios).

---

## 🏗️ 2. Arquitectura y Estructura del Proyecto

El proyecto está organizado siguiendo una **Arquitectura en Capas (N-Tier Architecture)** para mantener una estricta separación de responsabilidades:

```text
Clinica-San-Salud/
├── Clinica-San-Salud.slnx           # Archivo de solución de la aplicación (.NET)
└── SanSaludAPI/                     # Proyecto principal Web API
    ├── API/                         # 🟢 Capa de Presentación (Controladores HTTP)
    │   ├── MedicosController.cs
    │   ├── PacientesController.cs
    │   └── TurnosController.cs
    ├── BusinessLogic/               # 🔵 Capa de Lógica de Negocio (Servicios y Reglas)
    │   ├── IMedicoService.cs / MedicoService.cs
    │   ├── IPacienteService.cs / PacienteService.cs
    │   └── ITurnoService.cs / TurnoService.cs
    ├── DataAccess/                  # 🔴 Capa de Acceso a Datos (EF Core DbContext & Repositorios)
    │   ├── SanSaludDbContext.cs
    │   ├── DbInitializer.cs         # Crea la BD al iniciar y carga datos de ejemplo
    │   ├── Medico.cs / Paciente.cs / Turno.cs
    │   └── Repositories (IMedicoRepository, IPacienteRepository, ITurnoRepository)
    ├── Shared/                      # 🟡 DTOs y Excepciones Personalizadas
    │   ├── DTOs (MedicoCreateDTO, TurnoResponseDTO, etc.)
    │   └── Exceptions (OverlappingScheduleException, MedicoHasTurnosException, MatriculaDuplicadaException, BusinessExceptions, etc.)
    ├── Migrations/                  # Migraciones de Entity Framework Core
    └── bruno/                       # 🧪 Colección de peticiones HTTP para el cliente Bruno
```

### Descripción de cada capa:
1. **API Layer (`API/`)**: Expone los endpoints REST. No contiene lógica de negocio ni consultas directas a la base de datos; delega el trabajo a la capa de servicios y transforma los resultados en respuestas HTTP (`200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`, `409 Conflict`).
2. **Business Logic Layer (`BusinessLogic/`)**: Aplica las reglas del negocio (ejemplo: comprobar que la fecha del turno sea futura, verificar que el médico exista y que no tenga otro turno que se solape en ese horario).
3. **Data Access Layer (`DataAccess/`)**: Gestiona la interacción con la base de datos a través de **Entity Framework Core (SQLite)** mediante el patrón Repositorio.
4. **Shared Layer (`Shared/`)**: Contiene los **DTOs (Data Transfer Objects)** para que las entidades de base de datos nunca se expongan directamente al cliente, y las **Excepciones de Negocio** para comunicar errores específicos.

---

## 🚀 3. Requisitos Previos y Cómo Ejecutar

### Requisitos:
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) o superior.
- Un editor de código como **VS Code**, **Visual Studio 2022+** o **JetBrains Rider**.
- (Opcional) Cliente de API **Bruno** o navegador web.

### Pasos para iniciar la aplicación:

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/PracticasProfesionalizantes-I-2026/demo-repository.git
   cd demo-repository
   ```

2. **Restaurar dependencias y ejecutar la API:**
   ```bash
   cd SanSaludAPI
   dotnet restore
   dotnet run
   ```

3. La aplicación se ejecutará en las siguientes direcciones por defecto:
   - **HTTP**: `http://localhost:5175`
   - **HTTPS**: `https://localhost:7088`

4. **Base de Datos (SQLite):**
   - La base de datos `SanSalud.db` **no se incluye en el repositorio** (está ignorada por Git). No te preocupes: al iniciar la aplicación, la clase `DbInitializer` detecta si no existe y la crea automáticamente aplicando las migraciones de Entity Framework Core, además de cargar datos de ejemplo (ver sección [6.E](#-e-inicializador-automático-de-base-de-datos-dbinitializer)). Si deseas aplicar las migraciones manualmente:
     ```bash
     dotnet ef database update
     ```

---

## 📖 4. Documentación de API y Contratos (Scalar & OpenAPI)

Esta aplicación utiliza **OpenAPI 3.0** y **Scalar API Reference** para generar la documentación interactiva de la API de forma automática.

### ¿Cómo ver los contratos de la API?
Cuando la aplicación se ejecuta en ambiente de desarrollo (`ASPNETCORE_ENVIRONMENT=Development`), ingresa a las siguientes URLs desde tu navegador:

- **Scalar API Reference (Interfaz interactiva recomendada):**
  [http://localhost:5175/scalar/v1](http://localhost:5175/scalar/v1)

- **Especificación OpenAPI en formato JSON:**
  [http://localhost:5175/openapi/v1.json](http://localhost:5175/openapi/v1.json)

Desde la interfaz de Scalar podrás:
- Inspeccionar todos los **Endpoints disponibles** (`GET`, `POST`, `PUT`, `DELETE`).
- Ver los esquemas de **solicitud (Request Body)** y **respuesta (Response Body)**.
- Probar llamadas directamente desde el navegador (*Try it out*).

---

## 🧪 5. Colección de Bruno para Pruebas (QA / Estudiantes)

En la carpeta `SanSaludAPI/bruno` se incluye una colección completa de peticiones HTTP para el cliente de API **[Bruno](https://www.usebruno.com/)** (una alternativa liviana, de código abierto y sin almacenamiento en la nube a Postman/Insomnia).

### Estructura de la Colección de Bruno:
- `Medicos/`
  - `GET GetAll Medicos`: Obtener el listado de médicos.
  - `GET Get Medico by Id`: Buscar un médico por su UUID.
  - `POST Create Medico`: Dar de alta un nuevo médico.
  - `POST Create Medico Duplicado`: Reproducir el error `409 Conflict` por matrícula duplicada.
  - `DELETE Del Medico by Id`: Eliminar un médico (solo si no tiene turnos asociados).
- `Pacientes/`
  - `GET GetAll Pacientes`: Lista de pacientes.
  - `GET Get Paciente by Id`: Buscar un paciente.
  - `POST Create Paciente`: Registrar un nuevo paciente.
- `Turnos/`
  - `GET GetAll Turnos`: Listar todos los turnos.
  - `GET Get Turnos by Medico`: Filtrar turnos por ID de médico.
  - `GET Get Turnos by Paciente`: Filtrar turnos por ID de paciente.
  - `GET Get Turno by Id`: Buscar un turno por su UUID.
  - `POST Create Turno`: Crear un turno (con verificación de horario).
  - `PUT Update Turno`: Modificar fecha/hora o integrantes del turno.
  - `DELETE Delete Turno`: Cancelar/Eliminar un turno.

### Pasos para usar Bruno:
1. Descarga e instala **Bruno** desde [usebruno.com](https://www.usebruno.com/).
2. Abre Bruno y selecciona **Open Collection**.
3. Selecciona la carpeta `SanSaludAPI/bruno` que está dentro de este proyecto.
4. ¡Listo! Tendrás todos los endpoints organizados y listos para ejecutar contra `http://localhost:5175`.

---

## 🎓 6. Conceptos Clave para el Aprendizaje

Esta aplicación fue construida como un ejemplo pedagógico. A continuación se resumen los principales conceptos y patrones que los estudiantes pueden aprender analizando el código fuente:

### 💡 A. Inyección de Dependencias (DI)
En `Program.cs` se configuran las dependencias del contenedor con el tiempo de vida `Scoped` (una instancia por solicitud HTTP):
```csharp
// Registrar Repositorios
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();

// Registrar Servicios de Negocio
builder.Services.AddScoped<ITurnoService, TurnoService>();
```
*Aprendizaje:* Esto permite desacoplar los componentes y facilita la creación de pruebas unitarias mediante Mocks.

### 💡 B. Data Transfer Objects (DTOs)
Las entidades de base de datos como `Medico`, `Paciente` y `Turno` no se envían directamente al cliente REST. Se utilizan DTOs como `MedicoCreateDTO` y `TurnoResponseDTO`.
*Aprendizaje:* Previene problemas de sobreexposición de datos (*Over-posting*), evita referencias circulares en JSON y protege el modelo interno de la base de datos.

### 💡 C. Validación de Reglas de Negocio Complejas
En `TurnoService.cs` se implementa la lógica para detectar solapamiento de horarios entre turnos del mismo médico:
```csharp
if (isOverlapping)
{
    throw new OverlappingScheduleException("El turno solicitado se solapa en horario con otro existente para el mismo médico.");
}
```
*Aprendizaje:* Separar la lógica de validación en la capa de servicios evita ensuciar los controladores y garantiza la integridad de los datos.

### 💡 D. Cambio Transparente de Motor de Base de Datos
Entity Framework Core abstrae las consultas. Actualmente se utiliza SQLite:
```csharp
builder.Services.AddDbContext<SanSaludDbContext>(options =>
    options.UseSqlite(connectionString));
```
*Aprendizaje:* Para migrar a **PostgreSQL** o **SQL Server**, solo se debe instalar el paquete NuGet correspondiente y cambiar el proveedor en `Program.cs` sin modificar una sola línea de lógica SQL manual.

### 💡 E. Inicializador Automático de Base de Datos (`DbInitializer`)
El archivo `SanSalud.db` **no está versionado en el repositorio** (Git lo ignora porque es un binario que cambia con cada ejecución). Por eso, quien clona este proyecto por primera vez arranca **sin base de datos**, y sin esquema ni datos la API devolvería errores en todos los endpoints.

Para resolverlo, la clase `DataAccess/DbInitializer.cs` se invoca automáticamente desde `Program.cs` en cada inicio de la aplicación:
```csharp
// Program.cs — se ejecuta una vez por inicio, antes de atender requests
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SanSaludDbContext>();
    await DbInitializer.InitializeAsync(db);
}
```

**¿Qué hace exactamente?**
1. **Verifica si la base de datos existe** mediante `IRelationalDatabaseCreator.ExistsAsync()`.
2. **Si no existe**: aplica las migraciones pendientes con `Database.MigrateAsync()`, creando el archivo `SanSalud.db` con su esquema completo (tablas, claves foráneas e índices).
3. **Si las tablas están vacías**: carga **datos de ejemplo** (3 médicos, 3 pacientes y 3 turnos) para poder probar la API inmediatamente desde Scalar o Bruno.

**¿Por qué está diseñado así?**
- ✅ **Experiencia "clonar y ejecutar"**: no hay que crear nada a mano; al abrir la API ya hay datos para probar.
- ✅ **Seed idempotente**: solo inserta si las tablas están vacías, por lo que nunca duplica registros ni pisa los datos que vos cargues mientras practicas.
- ✅ **Usa `MigrateAsync()` y no `EnsureCreated()`**: respeta el historial de migraciones de la carpeta `Migrations/`, así que las migraciones futuras se aplicarán solas en cada arranque.
- ✅ **No interfiere con los tests de integración**: esos usan una base SQLite *en memoria* que ya "existe" y es creada por `CustomWebApplicationFactory`, por lo que el inicializador solo hace su seed idempotente.
- ✅ **GUIDs fijos en el seed**: los turnos de ejemplo referencian médicos y pacientes por identificadores predeterminados, garantizando la integridad referencial entre los datos.

> 💡 *Aprendizaje:* El patrón "initializer" es una alternativa simple a las *seed migrations* (`HasData` en `OnModelCreating`) cuando los datos de ejemplo dependen del tiempo (ej.: turnos siempre a fecha futura) o querés mantenerlos fuera del historial de migraciones. Fijate cómo `DbInitializer` vive en la capa `DataAccess`: es responsabilidad de infraestructura, no de negocio.

---

## 🧪 7. Pruebas Unitarias y de Integración Automatizadas (xUnit, Moq & WebApplicationFactory)

El proyecto incluye una suite completa de **28 pruebas automatizadas** en la carpeta `SanSaludAPI.Tests/`, divididas en **Pruebas Unitarias** y **Pruebas de Integración**.

### 🔹 A. Pruebas Unitarias (Unit Testing)
Aíslan la lógica de negocio simulando la base de datos y repositorios mediante **Moq**.

- **Librería de Mocking:** `Moq` (4.20.72)
- **Archivos de prueba:**
  - `TurnoServiceTests.cs`:
    - `CreateTurnoAsync_WithValidData_ReturnsCreatedTurnoResponseDTO`: Creación exitosa de turno.
    - `CreateTurnoAsync_WithPastDate_ThrowsInvalidTurnoDateException`: Rechazo de fechas en el pasado.
    - `CreateTurnoAsync_WithNonExistentMedico_ThrowsMedicoNotFoundException`: Validación de médico inexistente.
    - `CreateTurnoAsync_WithNonExistentPaciente_ThrowsPacienteNotFoundException`: Validación de paciente inexistente.
    - `CreateTurnoAsync_WithOverlappingSchedule_ThrowsOverlappingScheduleException`: Validación de solapamiento de horarios (regla de negocio de 2 horas).
    - `DeleteTurnoAsync_WithNonExistentId_ThrowsTurnoNotFoundException`: Eliminación de turno inexistente.
  - `MedicoServiceTests.cs`:
    - `GetAllMedicosAsync_ReturnsAllMedicosAsDTOs`: Listado de médicos.
    - `GetMedicoByIdAsync_WhenExists_ReturnsMedicoResponseDTO`: Búsqueda por ID.
    - `GetMedicoByIdAsync_WhenDoesNotExist_ReturnsNull`: Manejo de id no encontrado.
    - `CreateMedicoAsync_SavesAndReturnsCreatedMedico`: Alta de médico.
    - `CreateMedicoAsync_WithValidData_TrimsAndSavesMedico`: RN-05 normaliza los valores con `Trim()`.
    - `CreateMedicoAsync_WhenMatriculaAlreadyExists_ThrowsMatriculaDuplicadaException`: RN-04 matrícula única.
    - `CreateMedicoAsync_WithWhitespaceOnlyField_ThrowsValidationException`: Campo con solo espacios (RN-05).
    - `CreateMedicoAsync_WithEmptyRequiredField_ThrowsValidationException`: Campo con cadena vacía.
    - `DeleteMedicoAsync_WhenExistsAndHasNoTurnos_DeletesMedico`: Baja de médico sin turnos.
    - `DeleteMedicoAsync_WhenDoesNotExist_ThrowsMedicoNotFoundException`: Médico inexistente.
    - `DeleteMedicoAsync_WhenHasTurnos_ThrowsMedicoHasTurnosException`: Bloqueo de baja con turnos asociados.

---

### 🔹 B. Pruebas de Integración (Integration Testing)
Prueban el flujo HTTP completo del servidor ASP.NET Core desde la solicitud HTTP hasta la base de datos de prueba en memoria (**SQLite in-memory**).

- **Servidores de prueba:** `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`)
- **Base de datos de prueba:** `SQLite in-memory` (creada dinámicamente con `EnsureCreated()`).
- **Archivos de prueba:**
  - `MedicosControllerIntegrationTests.cs`:
    - `GetMedicos_ReturnsSuccessAndJsonArray`: Petición HTTP `GET /api/medicos` devuelve `200 OK`.
    - `CreateMedico_ReturnsSuccessAndCreatedMedico`: Petición HTTP `POST /api/medicos` registra el médico y responde con `201 Created`.
    - `CreateMedico_WithInvalidJson_Returns400BadRequest`: JSON roto responde `400 Bad Request`.
    - `CreateMedico_WithMissingRequiredField_Returns400BadRequest`: Campo obligatorio faltante → `400`.
    - `CreateMedico_WithWhitespaceOnlyField_Returns400BadRequest`: Campo con solo espacios → `400`.
    - `CreateMedico_WithOversizedField_Returns400BadRequest`: Longitud excedida (`MaxLength`) → `400`.
    - `CreateMedico_WhenDuplicateMatricula_Returns409Conflict`: Matrícula duplicada → `409 Conflict`.
    - `GetMedicoById_WhenNonExistent_ReturnsNotFound`: Petición HTTP `GET /api/medicos/{id}` responde `404 Not Found`.
  - `TurnosControllerIntegrationTests.cs`:
    - `CreateTurno_WithValidData_Returns201Created`: Petición HTTP `POST /api/turnos` registra un turno en el sistema.
    - `CreateTurno_WhenOverlappingSchedule_Returns409Conflict`: Petición HTTP `POST /api/turnos` responde `409 Conflict` cuando se intenta agendar un turno solapado.
    - `CreateTurno_WithPastDate_Returns400BadRequest`: Petición HTTP `POST /api/turnos` responde `400 Bad Request` ante fechas pasadas.

---

### 🚀 ¿Cómo ejecutar la suite completa de pruebas?
Desde la terminal en la raíz del repositorio, ejecuta:

```bash
dotnet test
```

Salida esperada:
```text
Serie de pruebas para SanSaludAPI.Tests.dll (.NETCoreApp,Version=v10.0)
Correctas! - Con error: 0, Superado: 28, Omitido: 0, Total: 28
```

---

## 🤖 8. IA Asistida: Memoria de Contexto e Indexación Semántica (AGENTS.md + Serena)

Este repositorio está preparado para desarrollarse **con ayuda de agentes de código con IA**
(opencode, Claude Code, Codex CLI, Cursor, etc.). Los asistentes de IA son muy útiles, pero
tienen dos limitaciones que este proyecto resuelve de forma didáctica:

1. **No recuerdan nada entre sesiones**: cada vez que abrís el agente, empieza "de cero" y
   debería releer decenas de archivos para entender la arquitectura (lo que consume tiempo,
   tokens y genera errores).
2. **Leen código como texto plano**: para encontrar una clase o método hacen búsquedas de texto
   (`grep`), sin entender realmente la estructura del programa.

Las herramientas que configuramos atacan cada problema:

| Problema | Solución | Archivo/Herramienta |
|---|---|---|
| Falta de memoria entre sesiones | Documento de contexto que el agente lee automáticamente | `AGENTS.md` |
| Lectura de código "sin entender" | Indexación semántica vía Language Server | `Serena` + `opencode.json` |

---

### 🔹 A. La memoria del proyecto: `AGENTS.md`

#### ¿Qué es y para qué existe?
`AGENTS.md` es un archivo **Markdown convencional** que vive en la raíz del repositorio.
Los agentes de código lo leen automáticamente al iniciar cada sesión, antes de hacer nada más.
Funciona como la "memoria a largo plazo" del proyecto: documentación escrita *pensada para una
IA*, no para humanos.

En este repositorio, `AGENTS.md` documenta:
- **Qué es el proyecto** y su dominio (turnos médicos).
- **Comandos esenciales**: cómo compilar, correr tests y levantar la API.
- **La arquitectura en capas** y las reglas obligatorias del flujo Controller → Service → Repository.
- **Convenciones de código** (DTOs manuales, excepciones tipadas, `AsNoTracking()`, comentarios en español...).
- **Reglas de negocio clave** (duración de turnos, validación de solapamientos) y pendientes conocidos.

#### Beneficios concretos
- ✅ El agente **respeta las convenciones** desde la primera línea de código que escribe.
- ✅ Se ahorran miles de tokens: no relee toda la estructura en cada sesión.
- ✅ Cualquier compañero que use otro agente (Claude Code, Codex...) obtiene el mismo contexto.

> 💡 **Aprendizaje:** Un buen `AGENTS.md` describe *reglas y decisiones* (el "porqué"),
> no cosas obvias que el agente puede deducir leyendo el código. Si tu proyecto tiene una
> regla de negocio o una convención de equipo, va ahí.

#### Cómo crear uno en tu propio proyecto
1. Creá un archivo llamado exactamente `AGENTS.md` en la raíz del repo.
2. Escribí, con tus palabras, las secciones mínimas:
   ```markdown
   # AGENTS.md — [Nombre del proyecto]
   ## Qué es          → 2-3 líneas sobre el propósito
   ## Comandos        → compilar, testear, ejecutar
   ## Arquitectura    → capas/carpetas y reglas de dependencia
   ## Convenciones    → estilo, nombres, idioma de comentarios
   ## Reglas de negocio → validaciones y restricciones importantes
   ```
3. Commitealo al repositorio para que todo el equipo se beneficie.

---

### 🔹 B. Indexación semántica con Serena

#### ¿Qué es Serena y para qué existe?
[Serena](https://github.com/oraios/serena) es un **servidor MCP de código abierto** (licencia MIT)
que le da a cualquier agente de IA capacidades propias de un IDE: entiende el código a nivel de
*símbolos* (clases, métodos, propiedades), no como texto plano.

- **MCP** (*Model Context Protocol*) es un estándar abierto que permite a los LLMs conectarse a
  herramientas externas, como si fueran plugins.
- Serena usa por debajo **Language Servers** (el mismo protocolo LSP que usa VS Code), por lo que
  soporta más de 40 lenguajes, incluyendo **C#/.NET**, Java, Python, TypeScript, Go, etc.

Sin Serena, un agente busca así:
```text
grep "OverlappingScheduleException"   ← encuentra texto, no significado
```

Con Serena, el agente puede preguntar por conceptos:
```text
find_symbol("TurnoService")                    ← encuentra LA clase, donde sea que esté
find_referencing_symbols("ValidateNoOverlappingTurnos")  ← quién usa este método
rename_symbol(...)                             ← renombrado seguro en todo el proyecto
get_diagnostics_for_file(...)                  ← errores de compilación en vivo
```

Esto significa navegación y refactorizaciones **más rápidas, confiables y baratas en tokens**.

#### Instalación (una sola vez por máquina)

Serena necesita [`uv`](https://docs.astral.sh/uv/) (gestor de Python). En macOS/Linux:

```bash
# 1. Instalar uv (queda en ~/.local/bin, no requiere sudo)
curl -LsSf https://astral.sh/uv/install.sh | sh

# 2. Instalar Serena
uv tool install -p 3.13 serena-agent

# 3. Verificar
serena --version
```

#### Configuración por proyecto

**Paso 1 — Registrar e indexar el proyecto** (crea `.serena/project.yml`):

```bash
cd ~/ruta/del/proyecto
serena project create . --ls csharp --index
```
- `--ls csharp` indica el lenguaje (para web: `typescript`; Python: `python`, etc.).
- `--index` pre-analiza todos los archivos y guarda el índice en caché (más rápido en cada sesión).

**Paso 2 — Conectar Serena al agente** mediante un archivo `opencode.json` en la raíz del repo:

```json
{
  "$schema": "https://opencode.ai/config.json",
  "mcp": {
    "serena": {
      "type": "local",
      "command": [
        "~/.local/bin/serena", "start-mcp-server",
        "--context", "ide",
        "--project", "~/ruta/del/proyecto"
      ],
      "enabled": true
    }
  }
}
```

Al abrir opencode dentro de esa carpeta, el servidor arranca solo, carga el índice y expone
sus herramientas al modelo. Para otros clientes (Claude Code, Codex...) hay instrucciones
específicas en la [documentación oficial](https://oraios.github.io/serena/02-usage/030_clients.html).

#### ¿Qué herramientas gana el agente?

| Herramienta | Para qué sirve |
|---|---|
| `find_symbol` | Encontrar clases/métodos por nombre, sin grep |
| `find_referencing_symbols` | Ver todas las llamadas/usos de un símbolo |
| `get_symbols_overview` | Ver el "esqueleto" de un archivo (como el outline del IDE) |
| `replace_symbol_body` | Reemplazar un método completo de forma segura |
| `rename_symbol` / `safe_delete_symbol` | Refactorizaciones atómicas |
| `write_memory` / `read_memory` | Notas persistentes del proyecto entre sesiones |

> 💡 **Aprendizaje:** Serena también incluye su propio sistema de **memorias**: notas que el
> agente puede escribir y releer en sesiones futuras (ej.: "cómo se agregó la validación de
> solapamientos"). Complementa muy bien a `AGENTS.md`: el primero es escrito *por el equipo*
> (decisiones), las memorias las escribe *el propio agente* (hallazgos).

#### Aplicarlo en tus propios proyectos
1. Elegí el lenguaje correcto en `--ls` (ver lista completa en la doc).
2. Agregá carpetas generadas a `ignored_paths` en `.serena/project.yml` (en este repo ignoramos
   `notebooklm/`, `bin/` y `obj/`) para indexar solo código real.
3. Commiteá `.serena/project.yml` y `opencode.json` junto con `AGENTS.md`: todo el equipo
   comparte la misma configuración.

---

### 🔹 C. Resumen: ¿con qué archivos trabaja la IA en este repo?

```text
Clinica-San-Salud/
├── AGENTS.md            # 📝 Memoria del proyecto: lo escribe el equipo, lo lee el agente
├── opencode.json        # 🔌 Registro del servidor MCP (Serena) para opencode
└── .serena/
    ├── project.yml      # ⚙️ Configuración de indexación (lenguaje, rutas ignoradas)
    └── *.cache          # 🧠 Índice semántico generado por el language server
```

¡Esperamos que este proyecto sirva como una guía práctica y clara para dominar el desarrollo de Web APIs modernas en .NET! 🚀
