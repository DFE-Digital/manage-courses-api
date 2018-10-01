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
            var includesPgce = new QualificationMapper().MapQualification("PG");
            includesPgce.Should().Be(IncludesPgce.Yes);
        }

        [Test]
        public void MapsBlankToNo()
        {
            var includesPgce = new QualificationMapper().MapQualification("");
            includesPgce.Should().Be(IncludesPgce.No);
        }
    }
}
