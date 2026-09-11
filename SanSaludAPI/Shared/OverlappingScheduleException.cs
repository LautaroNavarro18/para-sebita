using System;

namespace SanSaludAPI.Shared
{
    public class OverlappingScheduleException : Exception
    {
        public OverlappingScheduleException(string message) : base(message)
        {
        }
    }
}
