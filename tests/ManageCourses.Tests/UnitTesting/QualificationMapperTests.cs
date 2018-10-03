using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    public class QualificationMapperTests
    {
        private const string ProfpostFlagPg = "PG";

        [Test]
        public void MapsPgToYes()
        {
            var courseQualification = new QualificationMapper().MapQualification(ProfpostFlagPg, isFurtherEducationCourse: false, isPgde: false);
            courseQualification.Should().Be(CourseQualification.QtsWithPgce);
        }

        [Test]
        public void MapsBlankToNo()
        {
            var courseQualification = new QualificationMapper().MapQualification("", isFurtherEducationCourse: false, isPgde: false);
            courseQualification.Should().Be(CourseQualification.Qts);
        }

        [Test]
        public void MapsFurtherEducationToQtls()
        {
            var courseQualification = new QualificationMapper().MapQualification("", isFurtherEducationCourse: true, isPgde: false);
            courseQualification.Should().Be(CourseQualification.QtlsWithPgce, "this is a further education course, and we assume that these are always PGCE");
        }

        [Test]
        public void MapsPgFurtherEducationToQtlsWithPgce()
        {
            var courseQualification = new QualificationMapper().MapQualification(ProfpostFlagPg, isFurtherEducationCourse: true, isPgde: false);
            courseQualification.Should().Be(CourseQualification.QtlsWithPgce, "this is a 'PG' course with further education");
        }

        // check all variations of ProfPostFlag because they shouldn't affect the result
        [TestCase(ProfpostFlagPg)]
        [TestCase("PF")]
        [TestCase("BO")]
        [TestCase("")]
        [Test]
        public void MapsFurtherEducationToQtlsPgde(string profpostFlag)
        {
            var qualificationMapper = new QualificationMapper();
            var courseQualification = qualificationMapper.MapQualification(profpostFlag, isFurtherEducationCourse: true, isPgde: true);
            courseQualification.Should().Be(CourseQualification.QtlsWithPgde);
        }

        // check all variations of ProfPostFlag because they shouldn't affect the result
        [TestCase(ProfpostFlagPg)]
        [TestCase("PF")]
        [TestCase("BO")]
        [TestCase("")]
        [Test]
        public void MapsNonFurtherEducationToQtsPgde(string profpostFlag)
        {
            var qualificationMapper = new QualificationMapper();
            var courseQualification = qualificationMapper.MapQualification(profpostFlag, isFurtherEducationCourse: false, isPgde: true);
            courseQualification.Should().Be(CourseQualification.QtsWithPgde);
        }
    }
}
