using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.UcasCourseImporter.Mapping;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.Mapping
{
    [TestFixture]
    public class UcasStringParserTests
    {
        [Test]
        [TestCase("")]
        [TestCase("       ")]
        [TestCase("invalid date")]
        [TestCase(null)]
        public void TestGetDateFromStringReturnsNull(string dateToConvert)
        {
            var result = UcasStringParser.GetDateTimeFromString(dateToConvert);
            result.Should().BeNull();
        }
        [Test]
        [TestCase("2018-10-16 00:00:00")]
        [TestCase("2018-10-16")]
        [TestCase("2018 10 16")]
        [TestCase("2018/10/16")]
        public void TestGetDateFromStringReturnsDate(string dateToConvert)
        {
            var result = UcasStringParser.GetDateTimeFromString(dateToConvert);
            //assert
            result.Should().NotBeNull();
            var date = (DateTime) result;
            date.Day.Should().Be(16);
            date.Month.Should().Be(10);
            date.Year.Should().Be(2018);
        }
    }
}
