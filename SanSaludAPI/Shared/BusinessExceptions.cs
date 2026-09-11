using System;

namespace SanSaludAPI.Shared
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class MedicoNotFoundException : NotFoundException
    {
        public MedicoNotFoundException(Guid id) : base($"No se encontró el médico con ID: {id}") { }
    }

    public class PacienteNotFoundException : NotFoundException
    {
        public PacienteNotFoundException(Guid id) : base($"No se encontró el paciente con ID: {id}") { }
    }

    public class TurnoNotFoundException : NotFoundException
    {
        public TurnoNotFoundException(Guid id) : base($"No se encontró el turno con ID: {id}") { }
    }

    public class InvalidTurnoDateException : ValidationException
    {
        public InvalidTurnoDateException() : base("La fecha del turno debe ser futura") { }
    }
}
