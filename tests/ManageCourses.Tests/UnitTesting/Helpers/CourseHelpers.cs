using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting.Helpers
{
    [TestFixture]
    public class CourseHelpersTests
    {   
        [Test]
        [TestCase(CourseQualification.Qts, "f", "ss", "QTS full time with salary")]
        [TestCase(CourseQualification.Qts, "F", "SS", "QTS full time with salary")]
        [TestCase(CourseQualification.Qts, "f", "ss", "QTS full time with salary")]
        [TestCase(CourseQualification.Qts, "F", "SS", "QTS full time with salary")]
        [TestCase(CourseQualification.QtsWithPgce, "f", "ss", "PGCE with QTS full time with salary")]
        [TestCase(CourseQualification.QtsWithPgce, "F", "SS", "PGCE with QTS full time with salary")]
        [TestCase(CourseQualification.QtsWithPgce, "p", "ss", "PGCE with QTS part time with salary")]
        [TestCase(CourseQualification.QtsWithPgce, "P", "SS", "PGCE with QTS part time with salary")]
        [TestCase(CourseQualification.Qts, "f", "sd", "QTS full time")]
        [TestCase(CourseQualification.Qts, "F", "SD", "QTS full time")]
        [TestCase(CourseQualification.Qts, "f", "sd", "QTS full time")]
        [TestCase(CourseQualification.Qts, "F", "SD", "QTS full time")]
        [TestCase(CourseQualification.QtsWithPgce, "f", "sd", "PGCE with QTS full time")]
        [TestCase(CourseQualification.QtsWithPgce, "F", "SD", "PGCE with QTS full time")]
        [TestCase(CourseQualification.QtsWithPgce, "p", "sd", "PGCE with QTS part time")]
        [TestCase(CourseQualification.QtsWithPgce, "P", "SD", "PGCE with QTS part time")]
        [TestCase(CourseQualification.QtsWithPgce, "B", "SD", "PGCE with QTS, full time or part time")]
        [TestCase(CourseQualification.QtsWithPgce, "b", "sd", "PGCE with QTS, full time or part time")]
        [TestCase(CourseQualification.Qts, "f", "ta", "QTS full time teaching apprenticeship")]
        public void TestGetCourseVariantType(CourseQualification qualification, string studyMode, string programType, string expectedResult)
        {
            var course = new Course
            {
                Qualification = qualification,
                StudyMode = studyMode,
                ProgramType = programType
            };
            var result = course.TypeDescription;
            result.Should().Be(expectedResult);
        }
    }
}