using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
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
            var includesPgce = new QualificationMapper().MapQualification(ProfpostFlagPg, isFurtherEducationCourse: false, isPgde: false);
            includesPgce.Should().Be(IncludesPgce.Yes);
        }

        [Test]
        public void MapsBlankToNo()
        {
            var includesPgce = new QualificationMapper().MapQualification("", isFurtherEducationCourse: false, isPgde: false);
            includesPgce.Should().Be(IncludesPgce.No);
        }

        [Test]
        public void MapsFurtherEducationToQtls()
        {
            var includesPgce = new QualificationMapper().MapQualification("", isFurtherEducationCourse: true, isPgde: false);
            includesPgce.Should().Be(IncludesPgce.QtlsWithPgce, "this is a further education course, and we assume that these are always PGCE");
        }

        [Test]
        public void MapsPgFurtherEducationToQtlsWithPgce()
        {
            var includesPgce = new QualificationMapper().MapQualification(ProfpostFlagPg, isFurtherEducationCourse: true, isPgde: false);
            includesPgce.Should().Be(IncludesPgce.QtlsWithPgce, "this is a 'PG' course with further education");
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
            var includesPgce = qualificationMapper.MapQualification(profpostFlag, isFurtherEducationCourse: true, isPgde: true);
            includesPgce.Should().Be(IncludesPgce.QtlsWithPgde);
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
            var includesPgce = qualificationMapper.MapQualification(profpostFlag, isFurtherEducationCourse: false, isPgde: true);
            includesPgce.Should().Be(IncludesPgce.QtsWithPgde);
        }
    }
}
