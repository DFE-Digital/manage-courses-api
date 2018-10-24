using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Configuration;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using System;

namespace GovUk.Education.ManageCourses.UcasCourseImporter.Tests
{
    [TestFixture]
    public class UcasCourseImporterConfigurationOptionsTests
    {
        [Test]
        public void Validate_Success()
        {
            var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["azure_url"]).Returns("azure_url");

        mockConfig.Setup(c => c["azure_signature"]).Returns("azure_signature");

        mockConfig.Setup(c => c["manage_api_url"]).Returns("manage_api_url");

        mockConfig.Setup(c => c["manage_api_key"]).Returns("manage_api_key");
            var configOptions = new UcasCourseImporterConfigurationOptions(mockConfig.Object);

            try
            {
                configOptions.Validate();
                Assert.Pass();
            }
            catch (ArgumentException)
            {
                Assert.Fail("Failed");
            }
        }
    }
}
