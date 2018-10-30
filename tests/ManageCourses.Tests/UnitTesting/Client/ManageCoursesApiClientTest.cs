using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;
using Institution = GovUk.Education.ManageCourses.Domain.Models.Institution;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Client
{
    [TestFixture]
    public class ManageCoursesApiClientTest
    {

        private ManageCoursesApiClient sut;
        private Mock<HttpClientWrapper> mockHttp;
        private string accessToken = "accessToken";
        private string baseurl = "http://baseurl.com";

        [SetUp]
        public void SetUp()
        {
            mockHttp = new Mock<HttpClientWrapper>(MockBehavior.Strict, new object[1]);
            // mockHttp.Setup(x => x.GetAccessToken()).Returns(accessToken).Verifiable("Did not call GetAccessToken");

            sut = new ManageCoursesApiClient(baseurl, mockHttp.Object);
        }

        [Test]
        public async Task AcceptTerms_IndexAsync()
        {
            var controller = "acceptterms";
             mockHttp.Setup(x => x.PostAsync(It.Is<Uri>(y => y.AbsoluteUri == $"{baseurl}/api/{controller}/accept"), It.IsAny<StringContent>())).ReturnsAsync(
                new HttpResponseMessage() {
                    StatusCode = HttpStatusCode.OK
                }
            ).Verifiable();

            sut.AcceptTerms_IndexAsync().Wait();

            mockHttp.VerifyAll();
        }
        /*
         public async Task AcceptTerms_IndexAsync()
        {
            await PostObjects($"acceptterms/accept", null);
        }
        public async Task AccessRequest_IndexAsync(AccessRequest request)
        {
            await PostObjects($"accessrequest", request);
        }

        public async Task<Domain.Models.Course> Courses_GetAsync(string instCode, string ucasCode)
        {
            return await GetObjects<Domain.Models.Course>("$courses/{instCode}/course/{ucasCode}");
        }
        public async Task<InstitutionCourses> Courses_GetAllAsync(string instCode)
        {
            return await GetObjects<InstitutionCourses>("$courses/{instCode}");
        }

        public async Task<UserOrganisation> Organisations_GetAsync(string instCode)
        {
            return await GetObjects<UserOrganisation>("$organisation/{instCode}");
        }

        public async Task<IEnumerable<UserOrganisation>> Organisations_GetAllAsync()
        {
            return await GetObjects<IEnumerable<UserOrganisation>>("$organisation/getall");
        }
        public async Task<bool> Publish_PublishCourseToSearchAndCompareAsync(string instCode, string courseCode)
        {
            return await PostObjects<bool>("$publish/course/{instCode}/{courseCode}", null);;
        }
         */
    }
}