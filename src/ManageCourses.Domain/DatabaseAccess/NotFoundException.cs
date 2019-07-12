using System;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    /// <summary>
    /// Thrown when couldn't find requested entity in database
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message) { }
    }
}
