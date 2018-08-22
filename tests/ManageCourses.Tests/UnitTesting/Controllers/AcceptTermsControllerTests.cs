
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Controllers;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Controllers
{
    [TestFixture]
    public class AcceptTermsControllerTests
    {
        [Test]
        public void Index_UsesExemption()
        {
            var exemptionAttributes = typeof(AcceptTermsController).GetMethod("Index").CustomAttributes.Where(x => x.AttributeType == typeof(ExemptFromAcceptTermsAttribute)).ToList();

            Assert.AreEqual(1, exemptionAttributes.Count);
        }

        [Test]
        public void Index_CallsContextCorrectly()
        {
            // Arrange.... ugh

            var list = new List<McUser>{
                new McUser {
                Email = "foo@bar.com"
            }};

            var context = new Mock<IManageCoursesDbContext>();
            context.Setup(x => x.GetMcUsers("foo@bar.com")).Returns(list.AsQueryable()).Verifiable();
            context.Setup(x => x.Save()).Verifiable();
            
            var identity = new Mock<ClaimsIdentity>();
            identity.SetupGet(x => x.Name).Returns("foo@bar.com");

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.User).Returns(new ClaimsPrincipal(identity.Object));

            var controller = new AcceptTermsController(context.Object);

            controller.ControllerContext = new ControllerContext(new ActionContext(
                httpContext.Object,
                new Mock<RouteData>().Object,
                new MockControllerActionDescriptor(typeof(AcceptTermsController).GetMethod("Index")),
                new ModelStateDictionary()
            ));

            // Act

            var res = controller.Index();
            
            // Assert

            Assert.AreEqual(200, (res as StatusCodeResult).StatusCode);
            context.VerifyAll();
            Assert.IsNotNull(list[0].AcceptTermsDateUtc);

        }
        
        public class MockControllerActionDescriptor : ControllerActionDescriptor
        {
            public MockControllerActionDescriptor(MethodInfo methodInfo)
            {
                MethodInfo = methodInfo;
            }
        }
    }
}
