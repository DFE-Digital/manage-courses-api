using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Controllers;
using GovUk.Education.ManageCourses.Api.Exceptions;
using GovUk.Education.ManageCourses.Api.Services.Invites;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void Index()
        {
            var controller = new HomeController();
            var res = controller.Index() as RedirectResult;

            Assert.NotNull(res);
            Assert.AreEqual("/swagger", res.Url);
        }
    }
}
