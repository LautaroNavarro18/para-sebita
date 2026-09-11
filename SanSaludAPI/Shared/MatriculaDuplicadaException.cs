namespace SanSaludAPI.Shared
{
    public class MatriculaDuplicadaException : Exception
    {
        public MatriculaDuplicadaException(string matricula) : base($"Ya existe un médico con la matrícula: {matricula}")
        {
        }
    }
}