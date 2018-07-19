using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
    
namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    [TestFixture]
    public class DataServiceExportTests
    {
        private ApiLocalWebHost localWebHost = null;

        private IConfiguration config = null;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("integration-tests.json")
                .AddUserSecrets<DataServiceExportTests>()
                .AddEnvironmentVariables()
                .Build();

            localWebHost = new ApiLocalWebHost(config).Launch();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (localWebHost != null)
            {
                localWebHost.Stop();
            }
        }


        [Test]        
        [Category("Smoke")]
        [Explicit]
        public void DataExportWithEmptyCampus()
        {
            var dfeSignInConfig = config.GetSection("credentials").GetSection("dfesignin");
                                
            var communicator = new DfeSignInCommunicator(dfeSignInConfig["host"], dfeSignInConfig["redirect_host"], dfeSignInConfig["clientid"], dfeSignInConfig["clientsecret"]);
            var accessToken =  communicator.GetAccessTokenAsync(dfeSignInConfig["username"], dfeSignInConfig["password"]).Await();
                            
            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken));
            client.BaseUrl = localWebHost.Address;             

            client.Data_ImportAsync(TestData.MakeSimplePayload(dfeSignInConfig["username"])).Await();
            var export = client.Data_ExportByOrganisationAsync("ABC").Await();

            Assert.AreEqual("123", export.OrganisationId, "OrganisationId should be retrieved");
            Assert.AreEqual("Joe's school @ UCAS", export.OrganisationName, "OrganisationName should be retrieved");
            Assert.AreEqual("ABC", export.UcasCode, "UcasCode should be retrieved");

            Assert.AreEqual(1, export.ProviderCourses.Count, "Expecting exactly one in ProviderCourses");
            var course = export.ProviderCourses.Single();
            Assert.AreEqual(1, course.CourseDetails.Count, "Expecting exactly one in ProviderCourses.CourseDetails");
            var details = course.CourseDetails.Single();

            Assert.AreEqual("Joe's course for Primary teachers", details.CourseTitle, "ProviderCourses.CourseDetails.CourseTitle should be retrieved");
                        
            Assert.AreEqual(1, details.Variants.Count, "Expecting exactly one in ProviderCourses.CourseDetails.Variants");
            var variant = details.Variants.Single();

            Assert.AreEqual("XYZ", variant.UcasCode, "ProviderCourses.CourseDetails.Variants.UcasCode should be retrieved");

            Assert.AreEqual(1, variant.Campuses.Count, "Expecting exactly one in ProviderCourses.CourseDetails.Variants.Campuses");
            var campus = variant.Campuses.Single();

            Assert.AreEqual("", campus.Code, "ProviderCourses.CourseDetails.Variants.Campuses.Code should be retrieved");                
            Assert.AreEqual("Main campus site", campus.Name, "ProviderCourses.CourseDetails.Variants.Campuses.Name should be retrieved");
        }

        private class MockApiClientConfiguration : IManageCoursesApiClientConfiguration
        {
            private string accessToken;

            public MockApiClientConfiguration(string accessToken)
            {
                this.accessToken = accessToken;
            }

            public Task<string> GetAccessTokenAsync()
            {
                return Task.FromResult(accessToken);
            }
        }
        private static class TestData {
            public static Payload MakeSimplePayload(string username) => new Payload {
                   
                Users = ListOfOne(new McUser{
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = username
                }),

                Organisations = ListOfOne(new McOrganisation {
                    Name = "Joe's school",
                    OrgId = "123"
                }),

                OrganisationUsers = ListOfOne(new McOrganisationUser {
                    Email = username,
                    OrgId = "123"
                }),

                Institutions = ListOfOne(new UcasInstitution { 
                    InstFull = "Joe's school @ UCAS",
                    InstCode = "ABC"
                }),

                OrganisationInstitutions = ListOfOne(new McOrganisationInstitution {
                    InstitutionCode = "ABC",
                    OrgId = "123"
                }),
                
                Campuses = ListOfOne(new UcasCampus {
                    InstCode = "ABC",
                    CampusCode = "", // NOTE: EMPTY STRING
                    CampusName = "Main campus site"
                }),

                Courses = ListOfOne(new UcasCourse {
                    InstCode = "ABC",
                    CampusCode = "",
                    CrseCode = "XYZ",
                    CrseTitle = "Joe's course for Primary teachers"
                })

            };
        }
        
        private static ObservableCollection<T> ListOfOne<T> (T one) {
            return new ObservableCollection<T> { one };
        }

    }
}