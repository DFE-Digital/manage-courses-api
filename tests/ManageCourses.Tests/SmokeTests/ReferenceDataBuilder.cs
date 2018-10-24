
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

            Institution institution = new Institution
            {
                InstName = "Joe's school @ UCAS",
                InstCode = "ABC"
            };
            context.Institutions.Add(institution);
            
            context.OrganisationUsers.Add(new OrganisationUser {
                    User = user,
                    Organisation = organisation
                });
            
            context.OrganisationIntitutions.Add(new OrganisationInstitution {
                    Institution = institution,
                    Organisation = organisation
                });
            
        }
    }
}