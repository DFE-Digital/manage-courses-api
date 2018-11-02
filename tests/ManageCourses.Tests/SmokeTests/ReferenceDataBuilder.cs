
using System;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public static class ReferenceDataExtensions
    {
        public static void AddTestReferenceData(this ManageCoursesDbContext context, string username)
        {
            User user = new User
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Email = username,
                AcceptTermsDateUtc = DateTime.UtcNow
            };
            context.Users.Add(user);

            Organisation organisation = new Organisation
            {
                Name = "Joe's school",
                OrgId = "123"
            };
            context.Organisations.Add(organisation);

            Provider institution = new Provider
            {
                ProviderName = "Joe's school @ UCAS",
                ProviderCode = "ABC"
            };
            context.Providers.Add(institution);
            
            context.OrganisationUsers.Add(new OrganisationUser {
                    User = user,
                    Organisation = organisation
                });
            
            context.OrganisationProviders.Add(new OrganisationProvider {
                    Provider = institution,
                    Organisation = organisation
                });
            
        }
    }
}