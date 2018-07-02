using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Tests.Integration.DatabaseAccess;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class DataServiceTests : ManageCoursesDbContextIntegrationBase
    {
        public IDataService Subject = null;
        public IManageCoursesDbContext Context = null;

        private const string TestUserEmail_1 = "email_1@test-manage-courses.gov.uk";
        private const string TestUserEmail_2 = "email_2@test-manage-courses.gov.uk";
        private const string TestUserEmail_3 = "email_3@test-manage-courses.gov.uk";
        

        [SetUp]
        public void Setup()
        {
            Context = this.GetContext();
            Subject = new DataService(this.Context);
        }

        [Test]
        public void ProcessPayload()
        {
            var payload = GetPayload();

            Subject.ProcessPayload(payload);


            foreach (var item in Context.McUsers)
            {
                Assert.IsTrue(payload.Users.Any(
                    x => x.FirstName == item.FirstName &&
                    x.LastName == item.LastName &&
                    x.Email == item.Email));


            }

            GetCoursesForUser_isNull(TestUserEmail_1);
            GetCoursesForUser_isNull(TestUserEmail_2);
            GetCoursesForUser_isNull(TestUserEmail_3);
        }

        public Payload GetPayload()
        {
            var users = new List<McUser>()
            {
                new McUser
                {
                    FirstName = "FirstName_1",
                    LastName = "LastName_1",
                    Email = TestUserEmail_1
                },
                new McUser
                {
                    FirstName = "FirstName_2",
                    LastName = "LastName_2",
                    Email = TestUserEmail_2
                },
                new McUser
                {
                    FirstName = "FirstName_3",
                    LastName = "LastName_3",
                    Email = TestUserEmail_3
                }
            };
            var organisations = new List<McOrganisation> {
                new McOrganisation {
                    OrgId = "OrgId_1"
                },
                new McOrganisation {
                    OrgId = "OrgId_2"
                }

            };

            var institutions = new List<UcasInstitution>
            {
                new UcasInstitution {
                    InstCode = "InstCode_1"
                },
                new UcasInstitution {
                    InstCode = "InstCode_2"
                }
            };
            var mappers = new List<ProviderMapper> {
                new ProviderMapper () {
                    OrgId = organisations[1].OrgId,
                    UcasCode = "UcasCode_1" 
                }
            };

            var organisationInstitutions = new List<McOrganisationInstitution>
            {
                new McOrganisationInstitution {
                    InstitutionCode = institutions[1].InstCode,
                    OrgId = organisations[1].OrgId
                }
            };
            var organisationUsers = new List<McOrganisationUser>
            {
                new McOrganisationUser {
                    Email = TestUserEmail_2,
                },
                new McOrganisationUser {
                    Email = TestUserEmail_3,
                    OrgId = "OrgId_1"
                }
            };
            var result = new Payload()
            {
                Users = users,
                OrganisationInstitutions = organisationInstitutions,
                OrganisationUsers = organisationUsers,
                Organisations = organisations,
                Institutions = institutions,
                Mappers = mappers
            };

            return result;
        }

        [TestCase("nothing@nowhere.com")]
        public void GetCoursesForUser_isNull(string email)
        {
            var result = Subject.GetCoursesForUser(email);

            Assert.IsNull(result.OrganisationId);
            Assert.IsNull(result.OrganisationName);

        }
    }
}
