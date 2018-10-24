
using System;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public static class ReferenceDataExtensions
    {
        public static void AddTestReferenceData(this ManageCoursesDbContext context, string username)
        {
            McUser user = new McUser
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Email = username,
                AcceptTermsDateUtc = DateTime.UtcNow
            };
            context.McUsers.Add(user);

            McOrganisation organisation = new McOrganisation
            {
                Name = "Joe's school",
                OrgId = "123"
            };
            context.McOrganisations.Add(organisation);

            Institution institution = new Institution
            {
                InstFull = "Joe's school @ UCAS",
                InstCode = "ABC"
            };
            context.Institutions.Add(institution);
            
            context.McOrganisationUsers.Add(new McOrganisationUser {
                    McUser = user,
                    McOrganisation = organisation
                });
            
            context.McOrganisationIntitutions.Add(new McOrganisationInstitution {
                    Institution = institution,
                    McOrganisation = organisation
                });
            
        }
    }
}