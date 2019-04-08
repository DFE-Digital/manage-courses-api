using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using System;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders
{
    internal enum CourseType
    {
        NewUnpublished,
        NewPublished,
        SuspensedUnpublished,
        SuspensedPublished,
        RunningUnpublished,
        RunningPublished,
        DiscontinuedUnpublished,
        DiscontinuedPublished,
    }
}
