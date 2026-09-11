# Actividad: ¿Qué se hizo hasta ahora? (guía para el alumno)

Resumen didáctico de todo el trabajo realizado sobre la API **Clínica San Salud** hasta la
fecha, ordenado de lo ya commiteado a lo pendiente, con las decisiones técnicas explicadas
para que puedas **entender la lógica y repetir el proceso en tus propios proyectos**.

> Estado a la fecha: rama `main`. Todo el trabajo descrito ya está commiteado.

---

## 1. Resumen general

El proyecto es una API RESTful educativa en **.NET 10** con arquitectura **N-Tier**
(Controller → Service → Repository → DbContext). Los trabajos recientes agregaron tres cosas:

1. **Endpoint `DELETE /api/medicos/{id}`** con regla de negocio: no se puede borrar un médico
   con turnos asociados (commiteado en `caf490b`).
2. **Reglas de negocio para el alta de médicos**: matrícula única (**RN-04**) y normalización
   de campos con `Trim()` (**RN-05**) (commiteado en `4b8d5db`).
3. **Documentación técnica**: una guía de especificación de casos de uso + el caso de uso
   **CU-02 (Alta de Médico)** con su **matriz de trazabilidad** hacia los tests, y la
   sincronización de la documentación general (`README.md`, `AGENTS.md`) con la realidad del
   código.

En paralelo, la suite de tests creció de **19 a 28 pruebas** (6 nuevas por RN-04/RN-05 y una
por la auditoría final de cobertura).

---

## 2. Documentación técnica: casos de uso

Se creó `docs/analisis/casos-de-usos/` con:

| Archivo | Contenido |
| --- | --- |
| `GUIA-Especificacion-Casos-de-Uso.md` | Explica cómo escribir un caso de uso: plantilla (sección 3), códigos HTTP por capa, diferencias entre validación y verificación, trazabilidad CU → Test y un checklist de entrega. |
| `CU-02 Alta de Medico.md` | Caso de uso real: flujo principal (201), alternativos (400/400/400/400/409/500), postcondiciones y **matriz de trazabilidad** que liga cada flujo con su test unitario y de integración. |

Lo más importante para vos: la **matriz de trazabilidad**. Cada paso del caso de uso apunta al
test que lo comprueba. Así, la especificación no es un documento "de adorno": se puede
verificar automáticamente con `dotnet test`.

---

## 3. Auditoría final: docs vs. código

Como paso de cierre se releyó **todo** (código + tests + docs) buscando inconsistencias, y se
corrigieron estas en `README.md`:

1. Médicos se describían "con email y teléfono" — la entidad no los tiene. Corregido.
2. Se decía "16 pruebas automatizadas" — ahora son 28. Actualizado el número y la salida esperada.
3. El listado de tests unitarios/integración estaba desactualizado (4→11 y 3→8). Actualizado.
4. La colección de Bruno describía menos requests de los reales (faltaba `DELETE Del Medico by
   Id`, `POST Create Medico Duplicado` y `GET Get Turno by Id`). Agregados.
5. La matriz del CU-02 mapeaba los flujos 2a/2b al test de whitespace (que es del flujo 2c) y
   el flujo 1a (JSON inválido) no tenía test. Se corrigió la matriz y se **agregaron los tests
   faltantes** para respetar la regla de oro: *cada flujo del caso de uso debe tener al menos
   un test*.

Lección: la documentación "vive" y se desactualiza; conviene terminar cada feature con una
pasada de consistencia **docs ↔ código ↔ tests**.

---

## 4. Cómo aplicar este proceso en tu proyecto

Receta resumida de lo que se hizo, paso a paso:

1. **Elegí una regla de negocio nueva** (ej. "la matrícula debe ser única").
2. **Escribí el caso de uso primero** (guía, sección 3): define precondiciones, flujos feliz y
   tristes, y qué código HTTP responde cada uno.
3. **Implementá en el orden N-Tier**: repositorio (consulta) → service (regla + excepción
   tipada) → controller (try/catch que traduce a status code).
4. **Creá la excepción tipada** siguiendo el patrón de `Shared/`.
5. **Cubrí cada flujo con tests**: unitarios (Moq sobre el repositorio) para la lógica de
   negocio e integración (HTTP) para el código de respuesta.
6. **Agregá un request en la colección de Bruno** por cada caso relevante (feliz y de error).
7. **Actualizá la documentación**: matriz de trazabilidad del CU, `README.md` (lista de tests,
   colección de Bruno) y `AGENTS.md` (reglas nuevas, conteo de tests).
8. **Cerrá con una auditoría**: `dotnet test` en verde y releer docs vs. código buscando
   contradicciones.

---

