
using System;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public static class ReferenceDataExtensions
    {
        public static void AddTestReferenceData(this ManageCoursesDbContext context, string username)
        {
            context.McUsers.Add(new McUser{
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = username,
                    AcceptTermsDateUtc = DateTime.UtcNow
                });

            context.McOrganisations.Add(new McOrganisation {
                    Name = "Joe's school",
                    OrgId = "123"
                });
                
            context.Institutions.Add(new Institution { 
                    InstFull = "Joe's school @ UCAS",
                    InstCode = "ABC"
                });
            
            context.McOrganisationUsers.Add(new McOrganisationUser {
                    Email = username,
                    OrgId = "123"
                });
            
            context.McOrganisationIntitutions.Add(new McOrganisationInstitution {
                    InstCode = "ABC",
                    OrgId = "123"
                });
            
        }
    }
}