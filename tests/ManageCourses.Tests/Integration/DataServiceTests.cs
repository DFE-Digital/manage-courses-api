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
    public class DataServiceTests : DbIntegrationTestBase
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
            Context.UcasCourses.RemoveRange(Context.UcasCourses);
            Context.CourseCodes.RemoveRange(Context.CourseCodes);
            Context.UcasSubjects.RemoveRange(Context.UcasSubjects);
            Context.UcasCourseSubjects.RemoveRange(Context.UcasCourseSubjects);
            Context.UcasCampuses.RemoveRange(Context.UcasCampuses);
            Context.UcasCourseNotes.RemoveRange(Context.UcasCourseNotes);
            Context.UcasNoteTexts.RemoveRange(Context.UcasNoteTexts);         
            Context.McOrganisationIntitutions.RemoveRange(Context.McOrganisationIntitutions);
            Context.UcasInstitutions.RemoveRange(Context.UcasInstitutions);
            Context.McOrganisations.RemoveRange(Context.McOrganisations);
            Context.McOrganisationUsers.RemoveRange(Context.McOrganisationUsers);
            Context.Save();

            Subject = new DataService(this.Context, new UserDataHelper(), new Mock<ILogger<DataService>>().Object);
        }

        [Test]
        public void ProcessPayload()
        {            
            var userPayload = GetUserPayload();
            Subject.ProcessReferencePayload(userPayload);
            
            var payload = GetUcasPayload();
            Subject.ProcessUcasPayload(payload);


            foreach (var item in Context.McUsers)
            {
                var payloadUser = userPayload.Users.FirstOrDefault(
                    x => x.FirstName == item.FirstName &&
                    x.LastName == item.LastName &&
                    x.Email == item.Email);

                Assert.IsNotNull(payloadUser);

                var payloadOrgUser = userPayload.OrganisationUsers.FirstOrDefault(x => x.Email == payloadUser.Email);

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

        [Test]
        public void RepeatImportsArePossible()
        {            
            var payload = GetUcasPayload();            
            var userPayload = GetUserPayload();

            Subject.ProcessReferencePayload(userPayload);
            Subject.ProcessUcasPayload(payload);
                        
            Subject.ProcessReferencePayload(userPayload);
            Subject.ProcessUcasPayload(payload);

            Subject.ProcessReferencePayload(userPayload);
            Subject.ProcessReferencePayload(userPayload);
            
            Subject.ProcessUcasPayload(payload);
            Subject.ProcessUcasPayload(payload);


            foreach(var expected in userPayload.Users) 
            {
                var storedUser = Context.McUsers.ByEmail(expected.Email);

                Assert.AreEqual(1, storedUser.Count());
                Assert.AreEqual(expected.FirstName, storedUser.Single().FirstName);
                Assert.AreEqual(expected.LastName, storedUser.Single().LastName);
            }

            foreach(var expected in userPayload.Organisations) 
            {
                var storedOrg = Context.McOrganisations.Where(o => o.OrgId == expected.OrgId);

                Assert.AreEqual(1, storedOrg.Count());
            }

            foreach(var expected in userPayload.Institutions) 
            {
                var storedOrg = Context.UcasInstitutions.Where(o => o.InstCode == expected.InstCode);

                Assert.AreEqual(1, storedOrg.Count());
            }

            foreach(var expected in userPayload.OrganisationUsers) 
            {
                var count = Context.McOrganisationUsers
                    .Count(o => o.OrgId == expected.OrgId && o.Email == expected.Email);

                Assert.AreEqual(1, count);
            }

            foreach(var expected in userPayload.OrganisationInstitutions) 
            {
                var count = Context.McOrganisationIntitutions
                    .Count(o => o.OrgId == expected.OrgId && o.InstitutionCode == expected.InstitutionCode);

                Assert.AreEqual(1, count);
            }

            foreach(var expected in payload.Courses) 
            {
                var count = Context.UcasCourses
                    .Count(o => o.CrseCode == expected.CrseCode);

                Assert.AreEqual(1, count);
            }
        }



        public ReferenceDataPayload GetUserPayload()
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
            var result = new ReferenceDataPayload()
            {
                Users = users,
                OrganisationInstitutions = organisationInstitutions,
                OrganisationUsers = organisationUsers,
                Organisations = organisations,
                Institutions = institutions
            };

            return result;
        }

        public UcasPayload GetUcasPayload()
        {
            return new UcasPayload()
            {
                Courses = new List<UcasCourse>{
                    new UcasCourse()
                    {
                        InstCode = "InstCode_1",
                        CrseCode = "CourseCode_1"
                    }
                }
            };
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
