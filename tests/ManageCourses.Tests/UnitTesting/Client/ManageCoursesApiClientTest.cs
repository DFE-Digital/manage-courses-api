using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Domain.Models;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Client
{
    [TestFixture]
    public class ManageCoursesApiClientTest
    {

        private ManageCoursesApiClient manageCoursesApiClient;
        private Mock<HttpClientWrapper> mockHttp;
        private string baseurl = "http://fake.baseurl.com.fakefortesting";

        [SetUp]
        public void SetUp()
        {
            mockHttp = new Mock<HttpClientWrapper>(MockBehavior.Strict, new object[1]);

            manageCoursesApiClient = new ManageCoursesApiClient(baseurl, mockHttp.Object);
        }

        [Test]
        public void AcceptTerms_IndexAsync()
        {
             var controller = "acceptterms";
             var leaf = "/accept";

             SetupPostUrlVerification($"{baseurl}/api/{controller}{leaf}");

             manageCoursesApiClient.AcceptTerms_IndexAsync().Wait();
             mockHttp.VerifyAll();
        }

        [Test]
        public void AccessRequest_IndexAsync()
        {
            var request  = new Api.Model.AccessRequest();
            var controller = "accessrequest";
            var leaf = "";

            SetupPostUrlVerification($"{baseurl}/api/{controller}{leaf}");

            manageCoursesApiClient.AccessRequest_IndexAsync(request).Wait();

            mockHttp.VerifyAll();

        }

        private void SetupPostUrlVerification(string url)
        {
            var setup = mockHttp.Setup(x => x.PostAsync(It.Is<Uri>(y => y.AbsoluteUri == url), It.IsAny<StringContent>()));
            setup.ReturnsAsync(
                new HttpResponseMessage() {
                    StatusCode = HttpStatusCode.OK
                })
            .Verifiable();
        }

        private void SetupPostUrlVerification<T>(string url) where T : new()
        {
            var payloadJson = JsonConvert.SerializeObject(new T());

            var payloadStringContent = new StringContent(payloadJson, Encoding.UTF8, "application/json" );

            var setup = mockHttp.Setup(x => x.PostAsync(It.Is<Uri>(y => y.AbsoluteUri == url), It.IsAny<StringContent>()));
            setup.ReturnsAsync(
                new HttpResponseMessage() {
                    StatusCode = HttpStatusCode.OK,
                    Content = payloadStringContent
                })
            .Verifiable();
        }

        private void SetupGetUrlVerification<T>(string url) where T : new()
        {
            var payloadJson = JsonConvert.SerializeObject(new T());

            var payloadStringContent = new StringContent(payloadJson, Encoding.UTF8, "application/json" );

            var setup = mockHttp.Setup(x => x.GetAsync(It.Is<Uri>(y => y.AbsoluteUri == url)));
            setup.ReturnsAsync(
                new HttpResponseMessage() {
                    StatusCode = HttpStatusCode.OK,
                    Content = payloadStringContent
                })
            .Verifiable();
        }

        [Test]
        public void Courses_GetAsync()
        {
            var instCode ="instCode";
            var ucasCode = "ucasCode";

            var controller = "courses";
            var leaf = $"/{instCode}/course/{ucasCode}";
            SetupGetUrlVerification<Course>($"{baseurl}/api/{controller}{leaf}");

            var result = manageCoursesApiClient.Courses_GetAsync(instCode, ucasCode).Result;

            result.Should().BeOfType<Course>();
            mockHttp.VerifyAll();
        }

        [Test]
        public void Courses_GetAllAsync()
        {
            var instCode ="instCode";

            var controller = "courses";
            var leaf = $"/{instCode}";
            SetupGetUrlVerification<List<Domain.Models.Course>>($"{baseurl}/api/{controller}{leaf}");

            var result = manageCoursesApiClient.Courses_GetAllAsync(instCode).Result;

            result.Should().BeOfType<List<Domain.Models.Course>>();
            mockHttp.VerifyAll();
        }

        [Test]
        public void Organisations_GetAsync()
        {
            var instCode ="instCode";

            var controller = "organisations";
            var leaf = $"/{instCode}";
            SetupGetUrlVerification<ProviderSummary>($"{baseurl}/api/{controller}{leaf}");

            var result = manageCoursesApiClient.Organisations_GetAsync(instCode).Result;

            result.Should().BeOfType<ProviderSummary>();
            mockHttp.VerifyAll();
        }
        [Test]
        public void Organisations_GetAllAsync()
        {
            var controller = "organisations";
            var leaf = $"";
            SetupGetUrlVerification<List<ProviderSummary>>($"{baseurl}/api/{controller}{leaf}");

            var result = manageCoursesApiClient.Organisations_GetAllAsync().Result;

            result.Should().AllBeAssignableTo<IEnumerable<ProviderSummary>>();
            mockHttp.VerifyAll();
        }

        public void Publish_PublishCourseToSearchAndCompareAsync()
        {
            var instCode = "instCode";
            var courseCode = "courseCode";
            var controller = "publish";
            var leaf = $"/course/{instCode}/{courseCode}";

            SetupPostUrlVerification<bool>($"{baseurl}/api/{controller}{leaf}");

            var result = manageCoursesApiClient.Publish_PublishCourseToSearchAndCompareAsync(instCode, courseCode).Result;

            result.Should().BeFalse();
            mockHttp.VerifyAll();
        }
    }
}
