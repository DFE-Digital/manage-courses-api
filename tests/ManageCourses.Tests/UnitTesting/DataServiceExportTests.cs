using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    class DataServiceExportTests
    {
        private readonly string _orgWithProviderEmail = "someuser@somewhere.com";
        private readonly string _orgWithNoProviderEmail = "someotheruser@somewhereelse.com";
        private ManageCoursesDbContext _dbContext;
        private DataService _sut;

        [OneTimeSetUp]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ManageCoursesDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbInMemory");
            _dbContext = new ManageCoursesDbContext(optionsBuilder.Options);
            BuildFakeData();
            _sut = new DataService(_dbContext);
        }
        [Test]
        public void GetCoursesForUser_with_email_should_return_loaded_object()
        {
            var result = _sut.GetCoursesForUser(_orgWithProviderEmail);
            
            Assert.True(result.ProviderCourses.Count == 3);
        }
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase("qwpeoiqwepoi")]
        [TestCase("idontexist@nowhere.com")]
        public void GetCoursesForUser_should_return_empty_object(string email)
        {
            var result = _sut.GetCoursesForUser(null);

            Assert.True(result.ProviderCourses.Count == 0);
        }
        [Test]
        public void GetCoursesForUser_should_return_providers()
        {
            var result = _sut.GetCoursesForUser(_orgWithProviderEmail);

            Assert.True(result.ProviderCourses.All(x => !string.IsNullOrWhiteSpace(x.AccreditingProviderId)));
        }
        [Test]
        public void GetCoursesForUser_should_return_course_details()
        {
            var result = _sut.GetCoursesForUser(_orgWithProviderEmail);

            Assert.True(result.ProviderCourses.Select(CheckCourseDetails).All(y => y));
        }
        [Test]
        public void GetCoursesForUser_should_return_course_variants()
        {
            var result = _sut.GetCoursesForUser(_orgWithProviderEmail);

            //Assert.True(result.ProviderCourses.SelectMany(x => x.CourseDetails.Select(CheckVariants)).All(y => y));
            
            //use multiple asserts rather then the flattened assert above as this is easier to debug
            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[1].CourseDetails[0]));
            Assert.True(CheckVariants(result.ProviderCourses[2].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUser_should_return_campuses()
        {
            var result = _sut.GetCoursesForUser(_orgWithProviderEmail);

            Assert.True(CheckCampuses(result.ProviderCourses[0].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[1].CourseDetails[0].Variants[0]));
            Assert.True(CheckCampuses(result.ProviderCourses[2].CourseDetails[0].Variants[0]));
        }
        [Test]
        public void GetCoursesForUser_should_not_return_providers()
        {            
            var result = _sut.GetCoursesForUser(_orgWithNoProviderEmail);

            Assert.True(result.ProviderCourses.Count == 1);
            Assert.True(string.IsNullOrWhiteSpace(result.ProviderCourses[0].AccreditingProviderId));
            Assert.True(result.ProviderCourses[0].CourseDetails.Count == 19);
        }
        [Test]
        public void GetCoursesForUser_with_no_providers_should_return_course_variants()
        {
            var result = _sut.GetCoursesForUser(_orgWithNoProviderEmail);

            Assert.True(CheckVariants(result.ProviderCourses[0].CourseDetails[0]));
        }
        [Test]
        public void GetCoursesForUser_with_no_providers_should_not_return_campuses()
        {
            var result = _sut.GetCoursesForUser(_orgWithNoProviderEmail);

            Assert.False(CheckCampuses(result.ProviderCourses[0].CourseDetails[0].Variants[0]));
        }

        #region Fake Data
        private void BuildFakeData()
        {
            var dataParameters = SetupDataParameters();

            foreach (var parameters in dataParameters)
            {
                _dbContext.AddMcOrganisationUser(new McOrganisationUser { Email = parameters.Email, OrgId = parameters.OrgId });
                _dbContext.AddMcOrganisation(new McOrganisation { Name = parameters.OrgName, OrgId = parameters.OrgId });
                _dbContext.AddMcOrganisationInstitution(new McOrganisationInstitution { InstitutionCode = parameters.InstitutionCode, OrgId = parameters.OrgId });
                _dbContext.AddUcasInstitution(new UcasInstitution {InstCode = parameters.InstitutionCode, InstFull = parameters.InstitutionName });

                AddProviders(parameters.ProviderCodes, parameters.InstitutionName);

                if (parameters.ProviderCodes.Count == 0)
                {
                    AddNonProviderCourses(parameters.InstitutionCode);
                }
                else
                {
                    AddProviderCourses(parameters.CourseTitles[0], parameters.InstitutionCode);
                    AddCampuses(parameters.InstitutionCode);
                    AddSubjects(parameters.InstitutionCode);
                }
            }

            _dbContext.Save();

        }

        private void AddProviders(List<string> providerCodes, string institutionName)
        {
            foreach (var ucasInsCode in providerCodes)
            {
                _dbContext.AddUcasInstitution(new UcasInstitution { Addr1 = "Addr1", Addr2 = "Addr2", Addr3 = "Addr3", Addr4 = "Addr4", Postcode = "PS1 CDE", InstBig = institutionName, InstFull = institutionName, InstName = "ISCDE", SchemeMember = "Y", ContactName = "whatever", YearCode = "2019", InstCode = ucasInsCode, InstType = "O", Scitt = "N", AccreditingProvider = "Y" });
            }
        }

        private void AddNonProviderCourses(string institutionCode)
        {
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "37S8", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "History" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "2N22", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Drama" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "CX11", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Biology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W1X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Art And Design" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W3X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Music" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "R9X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Modern Languages" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "V6X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Religious Education" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "W9X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Design And Technology" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "G1X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Mathematics" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "1X99", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Computing" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X174", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary (7-11)" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X110", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary FS/KS1" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "F2X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Science With Chemistry" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "F3X2", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Science With Physics" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "Q3X1", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "English" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X100", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Primary And Early Years Education (5-11)" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "X9C6", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "HE", Studymode = "F", CrseTitle = "Physical Education" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "336P", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Business Studies" });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = null, Age = "S", CampusCode = "", CrseCode = "345L", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "HE", Studymode = "F", CrseTitle = "Geography" });
        }

        private void AddProviderCourses(string courseTitle, string institutionCode)
        {
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "D", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "T", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "V", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "3", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "4", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "B", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "K", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "C", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "M", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "K60", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "U80", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "L", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "S", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SS", Studymode = "F", CrseTitle = courseTitle });
            _dbContext.AddUcasCourse(new UcasCourse { AccreditingProvider = "S64", Age = "S", CampusCode = "V", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitle });
        }

        private void AddCampuses(string institutionCode)
        {
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Wobburn Road", Addr2 = "Croydon", Addr3 = "", Addr4 = "", Email = "", CampusCode = "S", CampusName = "St Mary's Catholic High School", InstCode = institutionCode, Postcode = "CR9 2EE", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Winlaton Road", Addr2 = "Bromley", Addr3 = "", Addr4 = "", Email = "", CampusCode = "B", CampusName = "Bonus Pastor Catholic College", InstCode = institutionCode, Postcode = "BR1 5PZ", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Belmont Grove", Addr2 = "Lewisham", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "K", CampusName = "Christ The King Sixth Form College", InstCode = institutionCode, Postcode = "SE13 5GE", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Llewellyn Street", Addr2 = "Bermondsey", Addr3 = "", Addr4 = "London", Email = "", CampusCode = "M", CampusName = "St Michael's Catholic College", InstCode = institutionCode, Postcode = "SE16 4UN", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Watling Street", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "C", CampusName = "St Catherine's Catholic School For Girls", InstCode = institutionCode, Postcode = "DA6 7QJ", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Halcot Avenue", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "L", CampusName = "St Columba's Catholic Boys School", InstCode = institutionCode, Postcode = "DA6 7QB", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Ghyllgrove", Addr2 = "Basildon", Addr3 = "", Addr4 = "Essex", Email = "", CampusCode = "D", CampusName = "De La Salle School And Language College", InstCode = institutionCode, Postcode = "SS14 2LA", RegionCode = "02", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Upper Norwood", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "V", CampusName = "Virgo Fidelis Convent Senior School", InstCode = institutionCode, Postcode = "SW19 1RS", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Atkins Road", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "4", CampusName = "La Retraite RomanCatholic Girl's School", InstCode = institutionCode, Postcode = "SW12 OAB", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Parkham Street", Addr2 = "Battersea", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "3", CampusName = "Saint John Bosco College", InstCode = institutionCode, Postcode = "SW11 3DQ", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasCampus(new UcasCampus { Addr1 = "Hollydale Road", Addr2 = "Nunhead", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "T", CampusName = "St Thomas The Apostle", InstCode = institutionCode, Postcode = "SE15 SEB", RegionCode = "01", TelNo = "" });
            _dbContext.AddUcasSubject(new UcasSubject { SubjectCode = "C1", SubjectDescription = "Biology", TitleMatch = "Biology" });
            _dbContext.AddUcasSubject(new UcasSubject { SubjectCode = "F0", SubjectDescription = "Science", TitleMatch = "Science" });
        }

        private void AddSubjects(string institutionCode)
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
        /// <summary>
        /// Sets up data parameters representing organisation with and without providers
        /// </summary>
        /// <returns></returns>
        private List<FakeDataParameters> SetupDataParameters()
        {
            var dataParameters = new List<FakeDataParameters>//two types of organisations
            {
                new FakeDataParameters//organisation with providers
                {
                    Email = _orgWithProviderEmail,
                    OrgId = "10970",
                    OrgName = "St Michael's Catholic College",
                    InstitutionName = "Catholic Teaching Alliance (South East London)",
                    InstitutionCode = "134",
                    CourseTitles = new List<string> {"Biology"},
                    ProviderCodes = new List<string> {"S64", "K60", "U80"}
                },
                new FakeDataParameters//organisation with no providers
                {
                    Email = _orgWithNoProviderEmail,
                    OrgId = "1502",
                    OrgName = "Bath Spa University",
                    InstitutionName = "Bath Spa University",
                    InstitutionCode = "B20",
                    CourseTitles = new List<string>(),
                    ProviderCodes = new List<string>()
                }
            };
            return dataParameters;
        }
        private struct FakeDataParameters
        {
            public string Email { get; set; }
            public string OrgId { get; set; }
            public string OrgName { get; set; }
            public string InstitutionCode { get; set; }
            public string InstitutionName { get; set; }
            public List<string> CourseTitles { get; set; }
            public List<string> ProviderCodes { get; set; }
        }
        #endregion

        #region Data Checks
        private bool CheckCourseDetails(ProviderCourse course)
        {
            var returnBool = course.CourseDetails.Any(x => (! string.IsNullOrWhiteSpace(x.CourseTitle)) && (! string.IsNullOrWhiteSpace(x.AgeRange)));

            return returnBool;
        }
        private bool CheckVariants(CourseDetail courseDetail)
        {
            var returnBool = courseDetail.Variants.Any(x =>
                (!string.IsNullOrWhiteSpace(x.UcasCode)) && (!string.IsNullOrWhiteSpace(x.CourseCode)) &&
                (!string.IsNullOrWhiteSpace(x.Name)));

            return returnBool;
        }
        private bool CheckCampuses(CourseVariant variant)
        {
            var returnBool = variant.Campuses.Any(x =>
                (!string.IsNullOrWhiteSpace(x.Name)) && (!string.IsNullOrWhiteSpace(x.Code)));

            return returnBool;
        }

        #endregion
    }
}
