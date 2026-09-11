# Actividad: Eliminar médico sin móvil de turnos asociados

Trabajo commiteado en `caf490b` (rama `main`).

## Objetivo

Implementar el borrado lógico de un médico con restricción de negocio: **no se puede
eliminar un médico que tenga turnos asociados**. Además se mejora el controlador
(`MedicosController`) con anotaciones de respuesta y constructor explícito.

## Archivos modificados

| Archivo | Cambio |
| --- | --- |
| `SanSaludAPI/Shared/MedicoHasTurnosException.cs` | **(nuevo)** Excepción tipada de negocio para cuando el médico tiene turnos. |
| `SanSaludAPI/bruno/Medicos/Del Medico by Id.bru` | **(nuevo)** Request DELETE para la colección de Bruno. |
| `SanSaludAPI/DataAccess/IMedicoRepository.cs` | Nuevos métodos `HasTurnosAsync` y `DeleteAsync`. |
| `SanSaludAPI/DataAccess/MedicoRepository.cs` | Implementación: verifica turnos con `AnyAsync` y elimina el médico si existe. |
| `SanSaludAPI/BusinessLogic/IMedicoService.cs` | Nuevo método `DeleteMedicoAsync`. |
| `SanSaludAPI/BusinessLogic/MedicoService.cs` | Regla de negocio: no existe → `MedicoNotFoundException`; tiene turnos → `MedicoHasTurnosException`; si no, elimina. |
| `SanSaludAPI/API/MedicosController.cs` | Endpoint `DELETE /api/Medicos/{id}` (204/404/409), mapeo try/catch de excepciones, `ProducesResponseType` y constructor clásico por inyección. |
| `SanSaludAPI.Tests/MedicoServiceTests.cs` | 3 tests unitarios nuevos. |

## Regla de negocio

1. `MedicoNotFoundException` → **404 NotFound** si el médico no existe.
2. `MedicoHasTurnosException` → **409 Conflict** si el médico tiene turnos.
3. Éxito → **204 NoContent**.

## Códigos HTTP usados en detalle

El endpoint `DELETE /api/Medicos/{id}` documenta y devuelve tres códigos HTTP:

| Código | Nombre | Cuándo | Body de respuesta |
| --- | --- | --- | --- |
| `204 No Content` | Éxito | El médico existe y no tiene turnos asociados → se elimina | Sin body (por especificación, un 204 no lleva contenido) |
| `404 Not Found` | No existe | `MedicoNotFoundException` → el ID no corresponde a ningún médico | `{ "message": "No se encontró el médico..." }` |
| `409 Conflict` | Conflicto de estado | `MedicoHasTurnosException` → el médico tiene turnos asociados y no se puede eliminar | `{ "message": "No se puede eliminar el médico... tiene turnos asociados" }` |

### Por qué es importante detallarlos

1. **Contrato de la API explícito y autocontenido**: `[ProducesResponseType]` documenta en Swagger/Scalar (OpenAPI) qué respuestas puede devolver cada endpoint. Quien consume la API sabe de antemano qué esperar, sin leer la implementación.
2. **Separación de responsabilidades HTTP vs. negocio**: el service sólo lanza excepciones tipadas (`MedicoNotFoundException`, `MedicoHasTurnosException`); el controller se encarga únicamente de traducir cada excepción a su código HTTP. Esto mantiene el flujo N-Tier (Controller → Service → Repository) sin acoplar reglas de negocio al protocolo HTTP.
3. **Semántica correcta de cada código**:
   - `204` comunica "operación completada sin contenido que devolver" (correcto para DELETE exitoso).
   - `404` comunica "el recurso no existe" (no es un error del cliente ni excepción técnica).
   - `409` comunica "el estado actual del recurso impide la operación" (el médico existe pero está en conflicto), en lugar de usar un 400 genérico o un 500.
4. **Facilita el consumo y las pruebas**: con los códigos bien definidos, un cliente (o las colecciones de Bruno) puede decidir la lógica según la respuesta; y los tests de integración pueden verificar rigorosamente que cada caso produce el status esperado.
5. **Buenas prácticas REST**: usar los códigos estándar y específicos (y no sólo 200/500) hace la API más expresiva, consistente y fácil de depurar.

## Tests nuevos (xUnit + Moq)

- `DeleteMedicoAsync_WhenExistsAndHasNoTurnos_DeletesMedico` → verifica que se llama a `DeleteAsync`.
- `DeleteMedicoAsync_WhenDoesNotExist_ThrowsMedicoNotFoundException` → verifica que NO se elimina.
- `DeleteMedicoAsync_WhenHasTurnos_ThrowsMedicoHasTurnosException` → verifica que NO se elimina.

## Nota: ¿hace falta `ModelState.IsValid` en `DeleteMedico`?

**No hace falta.** El método `DeleteMedico` del `MedicosController.cs` **no** incluye el check
`if (!ModelState.IsValid) { return BadRequest(ModelState); }` a propósito, por dos motivos:

1. **`[ApiController]` ya maneja la validación automáticamente**: al tener el atributo
   `[ApiController]` en la clase, ASP.NET Core devuelve `400 Bad Request` solo cuando
   `ModelState` es inválido. El check explícito que sí aparece en `CreateMedico` también es
   redundante por esto.
2. **`id` es un parámetro simple del route**: `DeleteMedico(Guid id)` toma el `Guid` de la URL
   (sin `[FromBody]` ni data annotations). Un ID ausente o mal formado ya genera `400`
   automático durante el binding, antes de entrar al método.

El check de `ModelState` sólo aportaría si el endpoint recibiera un cuerpo con `[FromBody]`
o validaciones manuales (p.ej. `[BindRequired]`), que no es el caso acá.
