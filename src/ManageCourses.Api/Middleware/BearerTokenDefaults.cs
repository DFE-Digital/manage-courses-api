using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenDefaults
    {
        public const string AuthenticationScheme = "BearerTokenScheme";
        public const string AuthenticationDisplayName = "BearerTokenDisplayName";
    }
}
