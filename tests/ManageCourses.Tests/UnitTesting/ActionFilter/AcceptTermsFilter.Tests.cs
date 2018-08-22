
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.ActionFilter
{
    [TestFixture]
    public class AcceptTermsFilterTests    
    {
        [Test]
        public void NoConsent()
        {
            var actionExecutingContext = GetActionExecutingContext("foo@example.com", "Index");
            var acceptTermsFilter = GetAcceptTermsFilter(GetUsers(null, 1));

            acceptTermsFilter.OnActionExecuting(actionExecutingContext);

            actionExecutingContext.Result.Should().BeOfType<StatusCodeResult>();
            (actionExecutingContext.Result as StatusCodeResult).StatusCode.Should().Be(451);
        }


        [Test]
        public void WithConsent()
        {
            var actionExecutingContext = GetActionExecutingContext("foo@example.com", "Index");
            var acceptTermsFilter = GetAcceptTermsFilter(GetUsers(DateTime.UtcNow, 1));

            acceptTermsFilter.OnActionExecuting(actionExecutingContext);

            actionExecutingContext.Result.Should().BeNull();
        }

        [Test]
        public void NoConsent_ButExempt()
        {
            var actionExecutingContext = GetActionExecutingContext("foo@example.com", "Exempt");
            var acceptTermsFilter = GetAcceptTermsFilter(GetUsers(null, 1));

            acceptTermsFilter.OnActionExecuting(actionExecutingContext);

            actionExecutingContext.Result.Should().BeNull();
        }

        [Test]
        public void Unauth_Throws()
        {
            var actionExecutingContext = GetActionExecutingContext(null, "Index");
            var acceptTermsFilter = GetAcceptTermsFilter(GetUsers(null, 1));

            Action action = () => acceptTermsFilter.OnActionExecuting(actionExecutingContext);
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Unauth_ButExempt_DoesntChangeResult()
        {
            var actionExecutingContext = GetActionExecutingContext(null, "Exempt");
            var acceptTermsFilter = GetAcceptTermsFilter(GetUsers(null, 1));

            acceptTermsFilter.OnActionExecuting(actionExecutingContext);
            
            actionExecutingContext.Result.Should().BeNull();
        }

        [Test]
        public void UnknownUser_Throws()
        {
            var actionExecutingContext = GetActionExecutingContext("foo@example.com", "Index");
            var acceptTermsFilter = GetAcceptTermsFilter(GetUsers(DateTime.Now, 0));

            Action action = () => acceptTermsFilter.OnActionExecuting(actionExecutingContext);
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void MultiUser_Throws()
        {
            var actionExecutingContext = GetActionExecutingContext("foo@example.com", "Index");            
            var acceptTermsFilter = GetAcceptTermsFilter(GetUsers(DateTime.Now, 2));

            Action action = () => acceptTermsFilter.OnActionExecuting(actionExecutingContext);
            action.Should().Throw<InvalidOperationException>();
        }
        
        private static AcceptTermsFilter GetAcceptTermsFilter(IQueryable<McUser> users)
        {
            var mockedContext = new Mock<IManageCoursesDbContext>();
            mockedContext.Setup(x => x.GetMcUsers("foo@example.com")).Returns(users);
            var acceptTermsFilter = new AcceptTermsFilter(mockedContext.Object);
            return acceptTermsFilter;
        }

        private IQueryable<McUser> GetUsers(DateTime? consentDate, int multiple)
        {
            var list = new List<McUser>();
            for (var i = 0; i < multiple; i++) list.Add(new McUser {
                Email = "foo@example.com",
                AcceptTermsDateUtc = consentDate
            });
            
            return list.AsQueryable();
        }

        private static ActionExecutingContext GetActionExecutingContext(string userEmail, string methodName)
        {
            var identity = new Mock<ClaimsIdentity>();
            identity.SetupGet(x => x.Name).Returns(userEmail);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.User).Returns(new ClaimsPrincipal(identity.Object));


            var actionContext = new ActionContext(
                httpContext.Object,
                new Mock<RouteData>().Object,
                new MockControllerActionDescriptor(typeof(MockController).GetMethod(methodName)),
                new ModelStateDictionary()
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );

            return actionExecutingContext;
        }

        public class MockController : Controller
        {
            public IActionResult Index()
            {
                return Ok();
            }

            [ExemptFromAcceptTerms]
            public IActionResult Exempt()
            {
                return Ok();
            }
        }

        public class MockControllerActionDescriptor : ControllerActionDescriptor
        {
            public MockControllerActionDescriptor(MethodInfo methodInfo)
            {
                MethodInfo = methodInfo;
            }
        }

        public class MockHttpContext : HttpContext
        {
            public override IFeatureCollection Features => throw new NotImplementedException();

            public override HttpRequest Request => throw new NotImplementedException();

            public override HttpResponse Response => throw new NotImplementedException();

            public override ConnectionInfo Connection => throw new NotImplementedException();

            public override WebSocketManager WebSockets => throw new NotImplementedException();

            public override AuthenticationManager Authentication => throw new NotImplementedException();

            public override ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override IServiceProvider RequestServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override void Abort()
            {
                throw new NotImplementedException();
            }
        }


    }

}