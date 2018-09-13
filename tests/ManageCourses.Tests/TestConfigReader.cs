using System;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Tests
{
    public class TestConfigReader
    {
        private readonly IConfiguration _configuration;

        public TestConfigReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiKey => GetRequired("api:key");

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
