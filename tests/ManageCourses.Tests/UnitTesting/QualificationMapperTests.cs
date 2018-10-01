using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    public class QualificationMapperTests
    {
        [Test]
        public void MapsPgToYes()
        {
            var includesPgce = new QualificationMapper().MapQualification("PG", false);
            includesPgce.Should().Be(IncludesPgce.Yes);
        }

        [Test]
        public void MapsBlankToNo()
        {
            var includesPgce = new QualificationMapper().MapQualification("", false);
            includesPgce.Should().Be(IncludesPgce.No);
        }

        [Test]
        public void MapsFurtherEducationToQtls()
        {
            var includesPgce = new QualificationMapper().MapQualification("", true);
            includesPgce.Should().Be(IncludesPgce.QtlsWithPgce, "this is a further education course, and we assume that these are always PGCE");
        }

        [Test]
        public void MapsPgFurtherEducationToQtlsWithPgce()
        {
            var includesPgce = new QualificationMapper().MapQualification("PG", true);
            includesPgce.Should().Be(IncludesPgce.QtlsWithPgce, "this is a 'PG' course with further education");
        }
    }
}
