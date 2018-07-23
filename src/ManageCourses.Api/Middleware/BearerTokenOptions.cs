using System;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class BearerTokenOptions : AuthenticationSchemeOptions    
    {
        public string UserinfoEndpoint { get; set; }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.UserinfoEndpoint))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The '{0}' option must be provided.", nameof(this.UserinfoEndpoint)), nameof(this.UserinfoEndpoint));

            }

            base.Validate();
        }
    }
}
