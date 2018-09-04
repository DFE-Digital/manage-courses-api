using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.EqualityComparers;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Helpers
{
    internal class TestHelper
    {
        public ManageCoursesDbContext DbContext { get; private set; }

        public const string EmailWithProvider = "someuser@somewhere.com";
        public const string EmailNoProvider = "someotheruser@somewhereelse.com";
        public const string EmailMultiOrg = "userwithmultipleorgs@somewhere.com";
        public const string EmailMultiInst = "userwithmultipleucascodess@somewhere.com";
        public const string UcasInstCodeWithProviders = "134";
        public const string UcasInstCodeNoProviders = "B20";
        private const string StMichaelSCatholicCollege = "St Michael's Catholic College";
        private const string MultiOrgInstCode1 = "2GG";
        private const string MultiOrgInstCode2 = "2G8";
        private const string MultiOrgInstCode3 = "N43";

        public TestHelper()
        {
            SetupInMemoryContext();
        }

        public TestHelper(ManageCoursesDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private void SetupInMemoryContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ManageCoursesDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbInMemory");
            DbContext = new ManageCoursesDbContext(optionsBuilder.Options);
        }

        public void BuildFakeDataForHelper()
        {
            DbContext.McUsers.AddRange(new List<McUser>
            {
                new McUser { Email = "tester1@test.com", FirstName = "Firstname1", LastName = "Lastname1" },
                new McUser { Email = "tester2@test.com", FirstName = "Firstname2", LastName = "Lastname2" },
                new McUser { Email = "tester3@test.com", FirstName = "Firstname3", LastName = "Lastname3" },
                new McUser { Email = "tester4@test.com", FirstName = "Firstname4", LastName = "Lastname4" },
                new McUser { Email = "tester5@test.com", FirstName = "Firstname5", LastName = "Lastname5" }
            });
            DbContext.Save();
        }

        public void BuildFakeDataForService()
        {
            // Creates the following variations:
            //
            // user - org - inst (has provider)
            //
            // user - org - inst (no provider)
            //
            // user - org - inst
            //      - org - inst
            //      - org - inst
            //
            // user - org - inst
            //            - inst

            const string catholicTeachingAllianceSouthEastLondon = "Catholic Teaching Alliance (South East London)";
            var mcUserHasProvider = new McUser { Email = EmailWithProvider };
            DbContext.McUsers.Add(mcUserHasProvider);
            var orgHasProvider = new McOrganisation { OrgId = "1234", Name = StMichaelSCatholicCollege };
            DbContext.McOrganisations.Add(orgHasProvider);
            DbContext.McOrganisationUsers.Add(new McOrganisationUser { OrgId = orgHasProvider.OrgId, Email = mcUserHasProvider.Email });
            var ucasInstitutionHasProvider = new UcasInstitution { InstCode = UcasInstCodeWithProviders, InstFull = catholicTeachingAllianceSouthEastLondon };
            DbContext.UcasInstitutions.Add(ucasInstitutionHasProvider);
            DbContext.McOrganisationIntitutions.Add(new McOrganisationInstitution { InstitutionCode = ucasInstitutionHasProvider.InstCode, OrgId = orgHasProvider.OrgId });
            AddProviders(new List<string> { "S64", "K60", "U80" }, catholicTeachingAllianceSouthEastLondon);

            const string bathSpaUniversity = "Bath Spa University";
            var mcUserNoProvider = new McUser { Email = EmailNoProvider };
            DbContext.McUsers.Add(mcUserNoProvider);
            var orgNoProvider = new McOrganisation { OrgId = "5678", Name = bathSpaUniversity };
            DbContext.McOrganisations.Add(orgNoProvider);
            DbContext.McOrganisationUsers.Add(new McOrganisationUser { OrgId = orgNoProvider.OrgId, Email = mcUserNoProvider.Email });
            var ucasInstitutionNoProvider = new UcasInstitution { InstCode = UcasInstCodeNoProviders, InstFull = bathSpaUniversity };
            DbContext.UcasInstitutions.Add(ucasInstitutionNoProvider);
            DbContext.McOrganisationIntitutions.Add(new McOrganisationInstitution { InstitutionCode = ucasInstitutionNoProvider.InstCode, OrgId = orgNoProvider.OrgId });

            var mcUserMulitOrg = new McUser { Email = EmailMultiOrg };
            DbContext.McUsers.Add(mcUserMulitOrg);
            //org1
            var multiOrg1 = new McOrganisation { OrgId = "10915", Name = "Attleborough Academy Norfolk NR17 2AJ" };
            DbContext.McOrganisations.Add(multiOrg1);
            DbContext.McOrganisationUsers.Add(new McOrganisationUser { OrgId = multiOrg1.OrgId, Email = mcUserMulitOrg.Email });
            var multiOrgInst1 = new UcasInstitution { InstCode = MultiOrgInstCode1, InstFull = "Attleborougn Academy Pertnership" };
            DbContext.UcasInstitutions.Add(multiOrgInst1);
            DbContext.McOrganisationIntitutions.Add(new McOrganisationInstitution { InstitutionCode = multiOrgInst1.InstCode, OrgId = multiOrg1.OrgId });
            //org2
            var multiOrg2 = new McOrganisation { OrgId = "10922", Name = "Fakenham Academy Norfolk NR21 9QT" };
            DbContext.McOrganisations.Add(multiOrg2);
            DbContext.McOrganisationUsers.Add(new McOrganisationUser { OrgId = multiOrg2.OrgId, Email = mcUserMulitOrg.Email });
            var multiOrgInst2 = new UcasInstitution { InstCode = MultiOrgInstCode2, InstFull = "Fakenham Academy Partnership" };
            DbContext.UcasInstitutions.Add(multiOrgInst2);
            DbContext.McOrganisationIntitutions.Add(new McOrganisationInstitution { InstitutionCode = multiOrgInst2.InstCode, OrgId = multiOrg2.OrgId });
            //org3
            var multiOrg3 = new McOrganisation { OrgId = "5627", Name = "Norfolk Teacher Training Centre" };
            DbContext.McOrganisations.Add(multiOrg3);
            DbContext.McOrganisationUsers.Add(new McOrganisationUser { OrgId = multiOrg3.OrgId, Email = mcUserMulitOrg.Email });
            var multiOrgInst3 = new UcasInstitution { InstCode = MultiOrgInstCode3, InstFull = "Norfolk Teacher Training Centre" };
            DbContext.UcasInstitutions.Add(multiOrgInst3);
            DbContext.McOrganisationIntitutions.Add(new McOrganisationInstitution { InstitutionCode = multiOrgInst3.InstCode, OrgId = multiOrg3.OrgId });

            var mcUserMultiInst = new McUser { Email = EmailMultiInst };
            DbContext.McUsers.Add(mcUserMultiInst);
            var multiInstOrg = new McOrganisation { OrgId = "2345", Name = "TestOrg" };
            DbContext.McOrganisations.Add(multiInstOrg);
            DbContext.McOrganisationUsers.Add(new McOrganisationUser { OrgId = multiInstOrg.OrgId, Email = mcUserMultiInst.Email });
            //inst1
            var multiInst1 = new UcasInstitution { InstCode = "ABC", InstFull = "Test Institution" };
            DbContext.UcasInstitutions.Add(multiInst1);
            DbContext.McOrganisationIntitutions.Add(new McOrganisationInstitution { InstitutionCode = multiInst1.InstCode, OrgId = multiInstOrg.OrgId });
            //inst2
            var multiInst2 = new UcasInstitution { InstCode = "DEF", InstFull = "Test Institution2" };
            DbContext.UcasInstitutions.Add(multiInst2);
            DbContext.McOrganisationIntitutions.Add(new McOrganisationInstitution { InstitutionCode = multiInst2.InstCode, OrgId = multiInstOrg.OrgId });

            AddCampuses();
            AddSubjects();
            AddProviderCourses();
            AddNonProviderCourses();
            DbContext.Save();
        }

        private void AddProviders(List<string> providerCodes, string institutionName)
        {
            foreach (var ucasInsCode in providerCodes)
            {
                DbContext.AddUcasInstitution(new UcasInstitution { Addr1 = "Addr1", Addr2 = "Addr2", Addr3 = "Addr3", Addr4 = "Addr4", Postcode = "PS1 CDE", InstBig = institutionName, InstFull = institutionName, InstName = "ISCDE", SchemeMember = "Y", ContactName = "whatever", YearCode = "2019", InstCode = ucasInsCode, InstType = "O", Scitt = "N", AccreditingProvider = "Y" });
            }
        }

        private void AddNonProviderCourses()
        {
            var toAdd = new List<UcasCourse>
            {
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "37S8", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "2N22", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Drama" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "CX11", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W1X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Art And Design" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W3X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Music" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "R9X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Modern Languages" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "V6X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Religious Education" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W9X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Design And Technology" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "G1X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Mathematics" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "1X99", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Computing" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X174", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary (7-11)" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X110", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary FS/KS1" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "F2X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Science With Chemistry" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "F3X2", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Science With Physics" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "Q3X1", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "English" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X100", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary And Early Years Education (5-11)" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X9C6", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Physical Education" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "336P", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Business Studies" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "345L", InstCode = UcasInstCodeNoProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Geography" },
            };
            DbContext.CourseCodes.AddRange(toAdd.Select(x => new CourseCode { InstCode = x.InstCode, CrseCode = x.CrseCode }));
            DbContext.UcasCourses.AddRange(toAdd);
        }

        private void AddProviderCourses()
        {
            var toAdd = new List<UcasCourse>
            {
                new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "D", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "T", CrseCode = "2HCQ", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "V", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "3", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "4", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "B", CrseCode = "2HCQ", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "K", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "C", CrseCode = "2HCQ", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "M", CrseCode = "2HCQ", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                //new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                //new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "L", CrseCode = "2HCQ", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "S", CrseCode = "2YCG", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SS", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "V", CrseCode = "2HCQ", InstCode = UcasInstCodeWithProviders, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },

                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QF", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Mathematics" },
                //new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QF", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Mathematics" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QH", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "English" },
                //new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QH", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" },
                //new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QF", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QJ", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Chemistry" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QJ", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Chemistry" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QK", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QK", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QG", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "English" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QD", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Physics" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QD", InstCode = MultiOrgInstCode1, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Physics" },

                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35Q6", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Drama" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35Q6", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Drama" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35Q8", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Design and Technology" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35Q9", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35Q9", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QB", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Business Studies" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QB", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Business Studies" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35QC", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Art and Design" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "C", CrseCode = "35QC", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Art and Design" },
                new UcasCourse { AccreditingProvider = MultiOrgInstCode3, Age = "S", CampusCode = "-", CrseCode = "35Q8", InstCode = MultiOrgInstCode2, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Design and Technology" },

                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM6", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM4", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GLX", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GLY", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GLZ", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM2", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM3", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM5", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8N", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8P", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8Q", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8R", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8S", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8T", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8N", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8P", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM5", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM2", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8Q", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "29VG", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM6", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM3", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2XKF", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "29VF", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8R", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM4", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8S", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GLZ", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "29VH", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8T", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2XKF", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "29VH", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "29VF", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
                new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "29VG", InstCode = MultiOrgInstCode3, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" },
            };
            DbContext.CourseCodes.AddRange(toAdd.Select(x => new CourseCode { InstCode = x.InstCode, CrseCode = x.CrseCode }).Distinct(new CourseCodeEquivalencyComparer()));
            DbContext.UcasCourses.AddRange(toAdd);
        }

        /// <summary>
        /// adds campuses for one institution
        /// </summary>
        private void AddCampuses()
        {
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "a1", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "", CampusName = "main no-provider campus", InstCode = UcasInstCodeNoProviders, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "a2", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "S", CampusName = "somewhere", InstCode = UcasInstCodeNoProviders, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });

            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "b1", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "-", CampusName = "somewhere", InstCode = MultiOrgInstCode1, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "b2", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "C", CampusName = "somewhere", InstCode = MultiOrgInstCode1, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });

            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "b1", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "-", CampusName = "somewhere", InstCode = MultiOrgInstCode2, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "b2", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "C", CampusName = "somewhere", InstCode = MultiOrgInstCode2, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });

            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "b1", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "-", CampusName = "somewhere", InstCode = MultiOrgInstCode3, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "b2", Addr2 = "", Addr3 = "", Addr4 = "", Email = "", CampusCode = "M", CampusName = "somewhere", InstCode = MultiOrgInstCode3, Postcode = "aa1 1aa", RegionCode = "01", TelNo = "" });

            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Wobburn Road", Addr2 = "Croydon", Addr3 = "", Addr4 = "", Email = "", CampusCode = "S", CampusName = "St Mary's Catholic High School", InstCode = UcasInstCodeWithProviders, Postcode = "CR9 2EE", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Winlaton Road", Addr2 = "Bromley", Addr3 = "", Addr4 = "", Email = "", CampusCode = "B", CampusName = "Bonus Pastor Catholic College", InstCode = UcasInstCodeWithProviders, Postcode = "BR1 5PZ", RegionCode = "02", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Belmont Grove", Addr2 = "Lewisham", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "K", CampusName = "Christ The King Sixth Form College", InstCode = UcasInstCodeWithProviders, Postcode = "SE13 5GE", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Llewellyn Street", Addr2 = "Bermondsey", Addr3 = "", Addr4 = "London", Email = "", CampusCode = "M", CampusName = StMichaelSCatholicCollege, InstCode = UcasInstCodeWithProviders, Postcode = "SE16 4UN", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Watling Street", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "C", CampusName = "St Catherine's Catholic School For Girls", InstCode = UcasInstCodeWithProviders, Postcode = "DA6 7QJ", RegionCode = "02", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Halcot Avenue", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "L", CampusName = "St Columba's Catholic Boys School", InstCode = UcasInstCodeWithProviders, Postcode = "DA6 7QB", RegionCode = "02", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Ghyllgrove", Addr2 = "Basildon", Addr3 = "", Addr4 = "Essex", Email = "", CampusCode = "D", CampusName = "De La Salle School And Language College", InstCode = UcasInstCodeWithProviders, Postcode = "SS14 2LA", RegionCode = "02", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Upper Norwood", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "V", CampusName = "Virgo Fidelis Convent Senior School", InstCode = UcasInstCodeWithProviders, Postcode = "SW19 1RS", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Atkins Road", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "4", CampusName = "La Retraite RomanCatholic Girl's School", InstCode = UcasInstCodeWithProviders, Postcode = "SW12 OAB", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Parkham Street", Addr2 = "Battersea", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "3", CampusName = "Saint John Bosco College", InstCode = UcasInstCodeWithProviders, Postcode = "SW11 3DQ", RegionCode = "01", TelNo = "" });
            DbContext.AddUcasCampus(new UcasCampus { Addr1 = "Hollydale Road", Addr2 = "Nunhead", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "T", CampusName = "St Thomas The Apostle", InstCode = UcasInstCodeWithProviders, Postcode = "SE15 SEB", RegionCode = "01", TelNo = "" });

        }

        private void AddSubjects()
        {
            DbContext.AddUcasSubject(new UcasSubject { SubjectCode = "C1", SubjectDescription = "Biology", TitleMatch = "Biology" });
            DbContext.AddUcasSubject(new UcasSubject { SubjectCode = "F0", SubjectDescription = "Science", TitleMatch = "Science" });
        }

        private void AddSubjects(string institutionCode)
        {
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2Q5K" });
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2Q5K" });
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2YCG" });
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2YCG" });
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2HCQ" });
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2HCQ" });
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2H5B" });
            DbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2H5B" });
        }
    }
}
