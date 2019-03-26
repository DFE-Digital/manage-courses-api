using System;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests
{
    public class TestConfigReader
    {
        const string SignInPrefix = "credentials:dfesignin:";

        private readonly IConfiguration _configuration;

        public TestConfigReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiKey => GetRequired("api:key");
        public string BackendApiKey => GetRequired("manage_courses_backend:key");

        public string SignInHost => GetRequired(SignInPrefix + "host");
        public string SignInRedirectHost => GetRequired(SignInPrefix + "redirect_host");
        public string SignInUsername => GetRequired(SignInPrefix + "username");
        public string SignInClientId => GetRequired(SignInPrefix + "clientid");
        public string SignInClientSecret => GetRequired(SignInPrefix + "clientsecret");
        public string SignInPassword => GetRequired(SignInPrefix + "password");

        private string GetRequired(string key)
        {
            var value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception($"Config value missing: '{key}'");
            }
            return value;
        }
    }
}
