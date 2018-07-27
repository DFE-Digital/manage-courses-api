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
using Microsoft.Extensions.Logging;
using Moq;

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
            Subject = new DataService(this.Context, new UserDataHelper(), new Mock<ILogger<DataService>>().Object);
        }

        [Test]
        public void ProcessPayload()
        {
            var payload = GetPayload();

            Subject.ProcessPayload(payload);


            foreach (var item in Context.McUsers)
            {
                var payloadUser = payload.Users.FirstOrDefault(
                    x => x.FirstName == item.FirstName &&
                    x.LastName == item.LastName &&
                    x.Email == item.Email);

                Assert.IsNotNull(payloadUser);

                var payloadOrgUser = payload.OrganisationUsers.FirstOrDefault(x => x.Email == payloadUser.Email);

                if (payloadOrgUser != null)
                {
                    if (!string.IsNullOrEmpty(payloadOrgUser.OrgId))
                    {
                        Assert.AreEqual(payloadUser.Email, TestUserEmail_3);
                        Assert.AreEqual(item.McOrganisationUsers.First().McOrganisation.OrgId, payloadOrgUser.OrgId);
                    }
                    else
                    {
                        Assert.AreEqual(payloadUser.Email, TestUserEmail_2);
                    }
                }
                else
                {
                    Assert.AreEqual(payloadUser.Email, TestUserEmail_1);
                }
            }

            GetCoursesForUser_isNull(TestUserEmail_1, null);
            GetCoursesForUser_isNull(TestUserEmail_2, null);
            GetCoursesForUser_isNull(TestUserEmail_3, "OrgId_1"); 
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
            };

            return result;
        }

        [TestCase("nothing@nowhere.com", null)]
        public void GetCoursesForUser_isNull(string email, string orgId)
        {
            var result = Subject.GetCoursesForUserOrganisation(email, orgId);

            Assert.IsNull(result.OrganisationId);
            Assert.IsNull(result.OrganisationName);

        }
    }
}
