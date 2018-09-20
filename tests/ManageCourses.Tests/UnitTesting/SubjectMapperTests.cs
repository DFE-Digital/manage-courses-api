﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class SubjectMapperTests
    {
        [TestCase("",                       "primary, english", "Primary, Primary with English")] // an example of a primary specialisation
        [TestCase("",                       "primary, physics", "Primary, Primary with science")] // another example of a primary specialisation
        [TestCase("Early Years",            "primary, early years", "Primary")] // an example of early years (which is absorbed into primary)
        [TestCase("Physics (Welsh medium)", "physics (abridged), welsh, secondary, science", "Physics")] // an example where science should be excluded because it's used as a category
        
        // examples of how the title is considered when adding additional subjects
        [TestCase("Physics",   "physics, secondary, science, english", "Physics")]  
        [TestCase("Physics with English",   "physics, secondary, science, english", "Physics, English")] 
        [TestCase("Physics with Science",   "physics, secondary, science, english", "Physics, Science")]
        [TestCase("Physics with Science and English",   "physics, secondary, science, english", "Physics, Science, English")]

        [TestCase("Further ed",             "further education, numeracy", "Further education")] // further education examplpe
        [TestCase("MFL (Chinese)",          "secondary, languages, languages (asian), chinese", "Mandarin")] // a rename
        [TestCase("",                       "secondary, welsh", "Welsh")] // an example of welsh, which only triggers if nothing else goes
        public void MapToSearchAndCompareCourse(string courseTitle, string commaSeparatedUcasSubjects, string commaSeparatedExpectedSubjects)
        {
            var expected = commaSeparatedExpectedSubjects.Split(", ");
            var result = new SubjectMapper().GetSubjectList(courseTitle, commaSeparatedUcasSubjects.Split(", "));      

            result.Should().BeEquivalentTo(expected);      
        }

    }
}
