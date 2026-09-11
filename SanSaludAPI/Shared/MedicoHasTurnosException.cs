namespace SanSaludAPI.Shared
{
    public class MedicoHasTurnosException : Exception
    {
        public MedicoHasTurnosException(Guid id) : base($"No se puede eliminar el médico con ID: {id} porque tiene turnos asociados")
        {
        }
    }
}
