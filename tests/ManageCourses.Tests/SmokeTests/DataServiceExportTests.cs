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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
    
namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    [TestFixture]
    public class DataServiceExportTests
    {
        private ApiProcessLauncher launcher = new ApiProcessLauncher();

        [Test]        
        [Category("Smoke")]
        [Explicit]
        public void DataExportWithEmptyCampus()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("integration-tests.json")
                .AddUserSecrets<DataServiceExportTests>()
                .Build();

            var dfeSignInConfig = config.GetSection("credentials").GetSection("dfesignin");
            
            OrganisationCourses export = null;
            using (launcher.LaunchApiLocally(config))
            {                
                var communicator = new DfeSignInCommunicator("signin-test-oidc-as.azurewebsites.net", "localhost:44364", dfeSignInConfig["clientid"], dfeSignInConfig["clientsecret"]);
                var accessToken =  communicator.GetAccessTokenAsync(dfeSignInConfig["username"], dfeSignInConfig["password"]).Await();
                
                var client = new ManageCoursesApiClient(new MockApiClientConfiguration(accessToken));
                client.BaseUrl = "http://localhost:6001";                

                client.Data_ImportAsync(TestData.SimplePayload).Await();

                export = client.Data_ExportByOrganisationAsync("ABC").Await();
            }

            Assert.IsNotNull(export, "Test could not complete");

            Assert.AreEqual("123", export.OrganisationId);
            Assert.AreEqual("Joe's school @ UCAS", export.OrganisationName);
            Assert.AreEqual("ABC", export.UcasCode);

            Assert.AreEqual(1, export.ProviderCourses.Count);
            var course = export.ProviderCourses.Single();
            Assert.AreEqual(1, course.CourseDetails.Count);
            var details = course.CourseDetails.Single();

            Assert.AreEqual("Joe's course for Primary teachers", details.CourseTitle);
                        
            Assert.AreEqual(1, details.Variants.Count);
            var variant = details.Variants.Single();

            Assert.AreEqual("XYZ", variant.UcasCode);

            Assert.AreEqual(1, variant.Campuses.Count);
            var campus = variant.Campuses.Single();

            Assert.AreEqual("", campus.Code);                
            Assert.AreEqual("Main campus site", campus.Name); 
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
            public static Payload SimplePayload = new Payload {
                   
                Users = ListOfOne(new McUser{
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = "tim.abell+4@digital.education.gov.uk"
                }),

                Organisations = ListOfOne(new McOrganisation {
                    Name = "Joe's school",
                    OrgId = "123"
                }),

                OrganisationUsers = ListOfOne(new McOrganisationUser {
                    Email = "tim.abell+4@digital.education.gov.uk",
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