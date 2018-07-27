using System;

namespace GovUk.Education.ManageCourses.Api.Services
{
    // todo: consider using Microsoft.Extensions.Internal.ISystemClock
    // I haven't yet because it involves switching to DateTimeOffset
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
