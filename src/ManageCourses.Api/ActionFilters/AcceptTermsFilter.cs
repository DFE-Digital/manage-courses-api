using System;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GovUk.Education.ManageCourses.Api.ActionFilters
{
    public class AcceptTermsFilter : IActionFilter
    {
        private readonly IManageCoursesDbContext dbContext;

        public AcceptTermsFilter(IManageCoursesDbContext context)
        {
            this.dbContext = context;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do nothing; this is needed for the interface
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var exemption = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ExemptFromAcceptTermsAttribute));

            if (exemption != null)
            {
                // leave out the AcceptTerms POST
                return;
            }
            
            var userEmail = context.HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new InvalidOperationException("AcceptTermsFilter invoked without a user being authenticated");
            }

            var users = dbContext.GetMcUsers(userEmail).ToList();
            if (users.Count > 1)
            {
                throw new InvalidOperationException($"multiple users found for {userEmail}");
            }
            if (users.Count == 0)
            {
                throw new InvalidOperationException($"user not found: {userEmail}");
            }
            
            if(users[0].AcceptTermsDateUtc == null) 
            {     
                // consent hasn't been given, return a status code in order to supress Action execution.             
                context.Result = new StatusCodeResult(451);
            }
            else
            {
                // consent has been given, just return to proceed to the Action
                return;
            }
        }
    }
}