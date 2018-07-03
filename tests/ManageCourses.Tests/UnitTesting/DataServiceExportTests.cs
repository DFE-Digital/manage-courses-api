using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using NUnit.Framework;


namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    class DataServiceExportTests
    {
        [Test]
        public void GetCoursesForUser_with_email_should_return_loaded_object()
        {
            var email = "someuser@somewhere.com";

            var dbContext = BuildFakeData(email, "10970", "St Michael's Catholic College", "134", "Catholic Teaching Alliance (South East London)", new List<string>{"Biology"});
            var sut = new DataService(dbContext);
            var result = sut.GetCoursesForUser(email);

            Assert.NotNull(result);
            Assert.True(result.ProviderCourses.Count == 3);
        }
        [Test]
        public void GetCoursesForUser_with_null_email_should_return_empty_object()
        {
            var email = "someuser@somewhere.com";

            var dbContext = BuildFakeData(email, "10970", "St Michael's Catholic College", "134", "Catholic Teaching Alliance (South East London)", new List<string> { "Biology" });
            var sut = new DataService(dbContext);
            var result = sut.GetCoursesForUser(null);

            Assert.NotNull(result);
            Assert.True(result.ProviderCourses.Count == 0);
        }
        private ManageCoursesDbContext BuildFakeData(string email, string orgId, string orgName, string institutionCode, string institutionName, List<string> courseTitles)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ManageCoursesDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbInMemory");
            var dbContext = new ManageCoursesDbContext(optionsBuilder.Options);
            dbContext.AddMcOrganisationUser(new McOrganisationUser { Id = 1, Email = email, OrgId = orgId });
            dbContext.AddMcOrganisation(new McOrganisation { Id = 1, Name = orgName, OrgId = orgId });
            dbContext.AddMcOrganisationInstitution(new McOrganisationInstitution { Id = 1, InstitutionCode = institutionCode, OrgId = orgId });
            dbContext.AddUcasInstitution(new UcasInstitution { InstCode = institutionCode, InstFull = institutionName });
            dbContext.AddUcasInstitution(new UcasInstitution { Id = 982, Addr1 = "Waldgrave Road", Addr2 = "Strawberry, Twickenham", Addr3 = "Richmond", Addr4 = "London", Postcode = "TW1 4SX", InstBig = "StMarysUnitTwickenham", InstFull = "St Mary's University, Twickenham", InstName = "SMARY", SchemeMember = "Y", ContactName = "Postgraduate Admissions Office", YearCode = "2019", InstCode = "S64", InstType = "O", Scitt = "N", AccreditingProvider = "Y" });
            dbContext.AddUcasInstitution(new UcasInstitution { Id = 335, Addr1 = "King's Admission Office", Addr2 = "James Clark Maxwell Building", Addr3 = "", Addr4 = "London", Postcode = "SE1 8WA", InstBig = "KingsCollege", InstFull = "King's College London (University Of London)", InstName = "KCL", SchemeMember = "Y", ContactName = "King's College London (University Of London)", YearCode = "2019", InstCode = "K60", InstType = "O", Scitt = "N", AccreditingProvider = "Y" });
            dbContext.AddUcasInstitution(new UcasInstitution { Id = 77, Addr1 = "Student And Registry Service", Addr2 = "UCL", Addr3 = "Gower Street", Addr4 = "London", Postcode = "WC1E 6BT", InstBig = "UniCollLondon", InstFull = "UCL University College London (University Of Lodon)", InstName = "UCL", SchemeMember = "Y", ContactName = "Sarah West", YearCode = "2019", InstCode = "U80", InstType = "O", Scitt = "N", AccreditingProvider = "Y" });
            dbContext.AddUcasCourse(new UcasCourse { Id = 1650, AccreditingProvider = "U80", Age = "S", CampusCode = "D", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 1651, AccreditingProvider = "S64", Age = "S", CampusCode = "T", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 2727, AccreditingProvider = "K60", Age = "S", CampusCode = "V", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 2728, AccreditingProvider = "K60", Age = "S", CampusCode = "3", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 5524, AccreditingProvider = "U80", Age = "S", CampusCode = "4", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 7227, AccreditingProvider = "S64", Age = "S", CampusCode = "B", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 12245, AccreditingProvider = "K60", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 12246, AccreditingProvider = "K60", Age = "S", CampusCode = "K", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 12401, AccreditingProvider = "S64", Age = "S", CampusCode = "C", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 12402, AccreditingProvider = "S64", Age = "S", CampusCode = "M", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 14908, AccreditingProvider = "K60", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 15047, AccreditingProvider = "U80", Age = "S", CampusCode = "L", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 15048, AccreditingProvider = "U80", Age = "S", CampusCode = "M", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 19910, AccreditingProvider = "S64", Age = "S", CampusCode = "L", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 19922, AccreditingProvider = "S64", Age = "S", CampusCode = "S", CrseCode = "2YCG", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "", ProgramType = "SS", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCourse(new UcasCourse { Id = 19934, AccreditingProvider = "S64", Age = "S", CampusCode = "V", CrseCode = "2HCQ", InstCode = institutionCode, CrseOpenDate = "2018-10-16 00:00:00", ProfpostFlag = "BO", ProgramType = "SD", Studymode = "F", CrseTitle = courseTitles[0] });
            dbContext.AddUcasCampus(new UcasCampus { Id = 1160, Addr1 = "Wobburn Road", Addr2 = "Croydon", Addr3 = "", Addr4 = "", Email = "", CampusCode = "S", CampusName = "St Mary's Catholic High School", InstCode = institutionCode, Postcode = "CR9 2EE", RegionCode = "01", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 1525, Addr1 = "Winlaton Road", Addr2 = "Bromley", Addr3 = "", Addr4 = "", Email = "", CampusCode = "B", CampusName = "Bonus Pastor Catholic College", InstCode = institutionCode, Postcode = "BR1 5PZ", RegionCode = "02", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 1526, Addr1 = "Belmont Grove", Addr2 = "Lewisham", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "K", CampusName = "Christ The King Sixth Form College", InstCode = institutionCode, Postcode = "SE13 5GE", RegionCode = "01", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 1527, Addr1 = "Llewellyn Street", Addr2 = "Bermondsey", Addr3 = "", Addr4 = "London", Email = "", CampusCode = "M", CampusName = "St Michael's Catholic College", InstCode = institutionCode, Postcode = "SE16 4UN", RegionCode = "01", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 1536, Addr1 = "Watling Street", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "C", CampusName = "St Catherine's Catholic School For Girls", InstCode = institutionCode, Postcode = "DA6 7QJ", RegionCode = "02", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 1573, Addr1 = "Halcot Avenue", Addr2 = "Bexleyheath", Addr3 = "Kent", Addr4 = "", Email = "", CampusCode = "L", CampusName = "St Columba's Catholic Boys School", InstCode = institutionCode, Postcode = "DA6 7QB", RegionCode = "02", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 1624, Addr1 = "Ghyllgrove", Addr2 = "Basildon", Addr3 = "", Addr4 = "Essex", Email = "", CampusCode = "D", CampusName = "De La Salle School And Language College", InstCode = institutionCode, Postcode = "SS14 2LA", RegionCode = "02", TelNo = "" });            
            dbContext.AddUcasCampus(new UcasCampus { Id = 4379, Addr1 = "Upper Norwood", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "V", CampusName = "Virgo Fidelis Convent Senior School", InstCode = institutionCode, Postcode = "SW19 1RS", RegionCode = "01", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 5046, Addr1 = "Atkins Road", Addr2 = "London", Addr3 = "", Addr4 = "", Email = "", CampusCode = "4", CampusName = "La Retraite RomanCatholic Girl's School", InstCode = institutionCode, Postcode = "SW12 OAB", RegionCode = "01", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 5290, Addr1 = "Parkham Street", Addr2 = "Battersea", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "3", CampusName = "Saint John Bosco College", InstCode = institutionCode, Postcode = "SW11 3DQ", RegionCode = "01", TelNo = "" });
            dbContext.AddUcasCampus(new UcasCampus { Id = 5389, Addr1 = "Hollydale Road", Addr2 = "Nunhead", Addr3 = "London", Addr4 = "", Email = "", CampusCode = "T", CampusName = "St Thomas The Apostle", InstCode = institutionCode, Postcode = "SE15 SEB", RegionCode = "01", TelNo = "" });
            dbContext.AddUcasSubject(new UcasSubject { Id = 28, SubjectCode = "C1", SubjectDescription = "Biology", TitleMatch = "Biology" });
            dbContext.AddUcasSubject(new UcasSubject { Id = 44, SubjectCode = "F0", SubjectDescription = "Science", TitleMatch = "Science" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 24349, YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2Q5K" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 24353, YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2Q5K" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 32147, YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2YCG" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 32164, YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2YCG" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 32330, YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2HCQ" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 32331, YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2HCQ" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 32379, YearCode = "2019", InstCode = institutionCode, SubjectCode = "F0", CrseCode = "2H5B" });
            dbContext.AddUcasCourseSubject(new UcasCourseSubject { Id = 32380, YearCode = "2019", InstCode = institutionCode, SubjectCode = "C1", CrseCode = "2H5B" });

            dbContext.Save();
            return dbContext;
        }
    }
}
