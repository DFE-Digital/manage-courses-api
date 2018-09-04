using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Helpers
{
    internal static class TestHelper
    {
        private static ManageCoursesDbContext _dbContext;

        public static string OrgWithProviderEmail { get; } = "someuser@somewhere.com";

        public static string OrgWithNoProviderEmail { get; } = "someotheruser@somewhereelse.com";

        public static string UserWithMultipleOrganisationsEmail { get; } = "userwithmultipleorgs@somewhere.com";
        public static string UserWithMultipleOrganisationUcasCodesEmail { get; } = "userwithmultipleucascodess@somewhere.com";

        public static string OrgUcasCodeWithProviders { get; } = "134";

        public static string OrgUcasCodeNoProviders { get; } = "B20";

        public static ManageCoursesDbContext GetFakeData(EnumTestType testType)
        {
            SetupContext();
            switch (testType)
            {
                case EnumTestType.DataService:
                    BuildFakeDataForService();
                    break;
                case EnumTestType.DataHelper:
                    BuildFakeDataForHelper();
                    break;
            }
            return _dbContext;
        }

        private static void SetupContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ManageCoursesDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbInMemory");
            _dbContext = new ManageCoursesDbContext(optionsBuilder.Options);
        }

        private static void BuildFakeDataForHelper()
        {
            _dbContext.McUsers.AddRange(new List<McUser>
            {
                new McUser { Email = "tester1@test.com", FirstName = "Firstname1", LastName = "Lastname1" },
                new McUser { Email = "tester2@test.com", FirstName = "Firstname2", LastName = "Lastname2" },
                new McUser { Email = "tester3@test.com", FirstName = "Firstname3", LastName = "Lastname3" },
                new McUser { Email = "tester4@test.com", FirstName = "Firstname4", LastName = "Lastname4" },
                new McUser { Email = "tester5@test.com", FirstName = "Firstname5", LastName = "Lastname5" }
            });
            _dbContext.Save();
        }
        private static void BuildFakeDataForService()
        {
            _dbContext.McUsers.AddRange(new List<McUser>
            {
                new McUser { Email = OrgWithNoProviderEmail },
                new McUser { Email = OrgWithProviderEmail },
                new McUser { Email = UserWithMultipleOrganisationsEmail },
                new McUser { Email = UserWithMultipleOrganisationUcasCodesEmail }
            });
            _dbContext.Save(); // for some reason McUsers isn't populated for reading until this is run. ¯\_(ツ)_/¯

            var testDataVariations = new List<FakeDataParameters>//two types of organisations
            {
                new FakeDataParameters//organisation with providers
                {
                    Email = OrgWithProviderEmail,
                    OrgId = "1234",
                    OrgName = "St Michael's Catholic College",
                    InstitutionName = "Catholic Teaching Alliance (South East London)",
                    InstitutionCode = "134",
                    ProviderCodes = new List<string> {"S64", "K60", "U80"}
                },
                new FakeDataParameters//organisation with no providers
                {
                    Email = OrgWithNoProviderEmail,
                    OrgId = "5678",
                    OrgName = "Bath Spa University",
                    InstitutionName = "Bath Spa University",
                    InstitutionCode = "B20",
                },
                new FakeDataParameters//user with multiple organisations
                {
                    Email = UserWithMultipleOrganisationsEmail,
                    OrgId = "10915",
                    OrgName = "Attleborough Academy Norfolk NR17 2AJ",
                    InstitutionName = "Attleborougn Academy Pertnership",
                    InstitutionCode = "2GG",
                },
                new FakeDataParameters//user with multiple organisations
                {
                    Email = UserWithMultipleOrganisationsEmail,
                    OrgId = "10922",
                    OrgName = "Fakenham Academy Norfolk NR21 9QT",
                    InstitutionName = "Fakenham Academy Partnership",
                    InstitutionCode = "2G8",
                },
                new FakeDataParameters//user with multiple organisations
                {
                    Email = UserWithMultipleOrganisationsEmail,
                    OrgId = "5627",
                    OrgName = "Norfolk Teacher Training Centre",
                    InstitutionName = "Norfolk Teacher Training Centre",
                    InstitutionCode = "N43",
                },
                new FakeDataParameters//user with multiple organisations
                {
                    Email = UserWithMultipleOrganisationUcasCodesEmail,
                    OrgId = "2345",
                    OrgName = "TestOrg",
                    InstitutionName = "Test Institution",
                    InstitutionCode = "ABC",
                },
                new FakeDataParameters//user with multiple organisations
                {
                    Email = UserWithMultipleOrganisationUcasCodesEmail,
                    OrgId = "2345",
                    OrgName = "TestOrg",
                    InstitutionName = "Test Institution2",
                    InstitutionCode = "DEF",
                }
            };

            foreach (var testDataEntry in testDataVariations)
            {
                var mcUser = _dbContext.GetMcUsers(testDataEntry.Email).Single();

                if (_dbContext.McOrganisations.FirstOrDefault(o => o.OrgId == testDataEntry.OrgId) == null)
                {
                    _dbContext.AddMcOrganisation(new McOrganisation { Name = testDataEntry.OrgName, OrgId = testDataEntry.OrgId });
                }

                if (_dbContext.UcasInstitutions.FirstOrDefault(i => i.InstCode == testDataEntry.InstitutionCode) == null)
                {
                    _dbContext.AddUcasInstitution(new UcasInstitution { InstCode = testDataEntry.InstitutionCode, InstFull = testDataEntry.InstitutionName });
                }

                _dbContext.AddMcOrganisationInstitution(new McOrganisationInstitution { InstitutionCode = testDataEntry.InstitutionCode, OrgId = testDataEntry.OrgId });
                _dbContext.AddMcOrganisationUser(new McOrganisationUser { Email = testDataEntry.Email, OrgId = testDataEntry.OrgId, McUser = mcUser });

                AddProviders(testDataEntry.ProviderCodes, testDataEntry.InstitutionName);
                _dbContext.Save();
            }

            AddProviderCourses();
            AddNonProviderCourses();
            AddCampuses();

            _dbContext.Save();

        }

        private static void AddProviders(List<string> providerCodes, string institutionName)
        {
            foreach (var ucasInsCode in providerCodes)
            {
                _dbContext.AddUcasInstitution(new UcasInstitution { Addr1 = "Addr1", Addr2 = "Addr2", Addr3 = "Addr3", Addr4 = "Addr4", Postcode = "PS1 CDE", InstBig = institutionName, InstFull = institutionName, InstName = "ISCDE", SchemeMember = "Y", ContactName = "whatever", YearCode = "2019", InstCode = ucasInsCode, InstType = "O", Scitt = "N", AccreditingProvider = "Y" });
            }
        }

        private static void AddNonProviderCourses()
        {
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "37S8", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "2N22", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Drama" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "CX11", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W1X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Art And Design" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W3X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Music" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "R9X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Modern Languages" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "V6X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Religious Education" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W9X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Design And Technology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "G1X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Mathematics" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "1X99", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Computing" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X174", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary (7-11)" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X110", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary FS/KS1" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "F2X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Science With Chemistry" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "F3X2", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Science With Physics" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "Q3X1", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "English" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X100", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary And Early Years Education (5-11)" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X9C6", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Physical Education" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "336P", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Business Studies" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "345L", InstCode = "B20", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Geography" });
        }
        private static void AddProviderCourses()
        {
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "D", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "T", CrseCode = "2HCQ", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "V", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "3", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "4", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "B", CrseCode = "2HCQ", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "K", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "C", CrseCode = "2HCQ", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "M", CrseCode = "2HCQ", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "L", CrseCode = "2HCQ", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "S", CrseCode = "2YCG", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SS", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "V", CrseCode = "2HCQ", InstCode = "134", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });

            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QF", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Mathematics" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QF", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Mathematics" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QH", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "English" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QH", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QF", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QJ", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Chemistry" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QJ", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Chemistry" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QK", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QK", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QG", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "English" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QD", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Physics" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QD", InstCode = "2GG", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Physics" });

            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35Q6", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Drama" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35Q6", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Drama" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35Q8", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Design and Technology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35Q9", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35Q9", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Computer Science" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QB", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Business Studies" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QB", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Business Studies" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35QC", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Art and Design" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "C", CrseCode = "35QC", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Art and Design" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "N43", Age = "S", CampusCode = "-", CrseCode = "35Q8", InstCode = "2G8", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SD", Studymode = "F", CrseTitle = "Design and Technology" });

            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM6", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM4", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GLX", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GLY", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GLZ", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM2", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM3", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2GM5", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8N", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8P", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8Q", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8R", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8S", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2R8T", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8N", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8P", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM5", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM2", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8Q", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "29VG", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM6", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM3", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2XKF", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "29VF", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8R", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GM4", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8S", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2GLZ", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "29VH", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "M", CrseCode = "2R8T", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "2XKF", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "29VH", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "29VF", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "-", CrseCode = "29VG", InstCode = "N43", CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SC", Studymode = "F", CrseTitle = "History" });
        }
        /// <summary>
        /// adds campuses for one institution
        /// </summary>
        private static void AddCampuses()
        {
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Wobburn Road", Addr2 = "Croydon", Addr3 = "", Addr4 = "", Email = "", CampusCode = "S", CampusName = "St Mary's Catholic High School", InstCode = "134", Postcode = "CR9 2EE", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Winlaton Road", Addr2 = "Bromley", Addr3 = "", Addr4 = "", Email = "", CampusCode = "B", CampusName = "Bonus Pastor Catholic College", InstCode = "134", Postcode = "BR1 5PZ", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Belmont Grove", Addr2 = "Lewisham", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "K", CampusName = "Christ The King Sixth Form College", InstCode = "134", Postcode = "SE13 5GE", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Llewellyn Street", Addr2 = "Bermondsey", Addr3 = "", Addr4 = "London", Email = "", CampusCode = "M", CampusName = "St Michael's Catholic College", InstCode = "134", Postcode = "SE16 4UN", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Watling Street", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "C", CampusName = "St Catherine's Catholic School For Girls", InstCode = "134", Postcode = "DA6 7QJ", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Halcot Avenue", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "L", CampusName = "St Columba's Catholic Boys School", InstCode = "134", Postcode = "DA6 7QB", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Ghyllgrove", Addr2 = "Basildon", Addr3 = "", Addr4 = "Essex", Email = "", CampusCode = "D", CampusName = "De La Salle School And Language College", InstCode = "134", Postcode = "SS14 2LA", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Upper Norwood", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "V", CampusName = "Virgo Fidelis Convent Senior School", InstCode = "134", Postcode = "SW19 1RS", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Atkins Road", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "4", CampusName = "La Retraite RomanCatholic Girl's School", InstCode = "134", Postcode = "SW12 OAB", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Parkham Street", Addr2 = "Battersea", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "3", CampusName = "Saint John Bosco College", InstCode = "134", Postcode = "SW11 3DQ", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Hollydale Road", Addr2 = "Nunhead", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "T", CampusName = "St Thomas The Apostle", InstCode = "134", Postcode = "SE15 SEB", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasSubject(new UcasSubject { SubjectCode = "C1", SubjectDescription = "Biology", TitleMatch = "Biology" });
            _dbContext.AddUcasSubject(new UcasSubject { SubjectCode = "F0", SubjectDescription = "Science", TitleMatch = "Science" });
        }

        private static void AddSubjects(string institutionCode)
        {
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2Q5K" });
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2Q5K" });
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2YCG" });
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2YCG" });
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2HCQ" });
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2HCQ" });
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2H5B" });
            _dbContext.AddUcasCourseSubject(new UcasCourseSubject { YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2H5B" });
        }

        private class FakeDataParameters
        {
            public FakeDataParameters()
            {
                ProviderCodes = new List<string>();
            }

            public string Email { get; set; }
            public string OrgId { get; set; }
            public string OrgName { get; set; }
            public string InstitutionCode { get; set; }
            public string InstitutionName { get; set; }
            public List<string> ProviderCodes { get; set; }
        }
    }
}
