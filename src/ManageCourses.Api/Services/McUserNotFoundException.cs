using System;

namespace GovUk.Education.ManageCourses.Api.Services
{
    /// <summary>
    /// Thrown when we receive a sign-in from an email address we don't know about.
    /// Expected to be caught and handled.
    /// </summary>
    public class McUserNotFoundException : Exception
    {
    }
}
