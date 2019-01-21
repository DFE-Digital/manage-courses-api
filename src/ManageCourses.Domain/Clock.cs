using System;

namespace GovUk.Education.ManageCourses.Domain
{
    public class Clock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
