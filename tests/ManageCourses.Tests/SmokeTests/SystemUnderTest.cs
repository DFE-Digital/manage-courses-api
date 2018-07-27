using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    [TestFixture]
    [Category("Smoke")]
    [Explicit]
    public class SystemUnderTest : ApiSmokeTestBase
    {
        [Test]        
        public void DataExport_WithEmptyCampus()
        {
            var dfeSignInConfig = config.GetSection("credentials").GetSection("dfesignin");
                                
            var communicator = new DfeSignInCommunicator(dfeSignInConfig["host"], dfeSignInConfig["redirect_host"], dfeSignInConfig["clientid"], dfeSignInConfig["clientsecret"]);
            var accessToken =  communicator.GetAccessTokenAsync(dfeSignInConfig["username"], dfeSignInConfig["password"]).Result;
                            

            var httpClient = new HttpClient();

            var apiKeyAccessToken = config["api:key"];


            var clientImport = new ManageCoursesApiClient(new MockApiClientConfiguration(apiKeyAccessToken), httpClient);
            clientImport.BaseUrl = localWebHost.Address;

            clientImport.Data_ImportAsync(TestData.MakeSimplePayload(dfeSignInConfig["username"])).Wait();


            var clientExport = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            clientExport.BaseUrl = localWebHost.Address;

            var export = clientExport.Data_ExportByOrganisationAsync("ABC").Result;

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

        [Test]
        public void DataExport_badAccesCode_404()
        {
            var accessToken = "badAccesCode";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = localWebHost.Address;


            Assert.That(() => client.Data_ExportByOrganisationAsync("ABC"),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));

            Assert.That(() => client.Data_ImportAsync(new Payload()),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));

        }

        [Test]
        public async Task Invite()
        {
            var accessToken = config["api:key"];

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = localWebHost.Address;

            var result = await client.Invite_IndexAsync();

            Assert.AreEqual(200, result.StatusCode);

            var client2 = new ManageCoursesApiClient(new MockApiClientConfiguration("accessToken"), httpClient);
            client2.BaseUrl = localWebHost.Address;
        }

        [Test]
        public void Invite_badAccesCode_404()
        {
            var accessToken = "badAccesCode";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = localWebHost.Address;


            Assert.That(() => client.Invite_IndexAsync(),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (404)."));
        }

        [Test]
        public void Invite_noAccesCode_401()
        {
            var accessToken = "";

            var httpClient = new HttpClient();

            var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken), httpClient);
            client.BaseUrl = localWebHost.Address;

            Assert.That(() => client.Invite_IndexAsync(),
                Throws.TypeOf<SwaggerException>()
                    .With.Message.EqualTo("The HTTP status code of the response was not expected (401)."));
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