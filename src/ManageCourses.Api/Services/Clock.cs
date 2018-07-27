using System;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class Clock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
