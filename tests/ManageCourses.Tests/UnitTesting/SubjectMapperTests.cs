using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
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
        [TestCase("Physics with Science",   "physics, secondary, science, english", "Physics, Balanced science")]
        [TestCase("Physics with Science and English",   "physics, secondary, science, english", "Physics, Balanced science, English")]

        [TestCase("Further ed",             "further education, numeracy", "Further education")] // further education examplpe
        [TestCase("MFL (Chinese)",          "secondary, languages, languages (asian), chinese", "Mandarin")] // a rename
        [TestCase("",                       "secondary, welsh", "Welsh")] // an example of welsh, which only triggers if nothing else goes

        [TestCase("Computer science", "computer studies, science", "Computing")] // here science is used as a category
        [TestCase("Computer science with Science", "computer studies, science", "Computing, Balanced science")] // here, it is explicit

        [TestCase("Primary with Mathematics", "primary, mathematics", "Primary, Primary with mathematics")] // bug fix test: accidentally included maths in the list of sciences
        
        [TestCase("Mfl", "languages", "Modern languages (other)")]
        [TestCase("Latin", "latin", "Classics")] // latin and classics have been merged
        [TestCase("Primary (geo)", "primary, geography", "Primary, Primary with history and geography")] // Primary with hist/geo have beeen merged
        [TestCase("Primary (history)", "primary, history", "Primary, Primary with history and geography")] // Primary with hist/geo have beeen merged

        [TestCase("Computing", "secondary, computer studies, information communication technology", "Computing")] // no ICT

        [TestCase("Mandarin and ESOL", "mandarin, english as a second or other language", "Mandarin, English as a second or other language")] // secondary ESOL
        [TestCase("PCET ESOL", "further education, english as a second or other language", "Further education")] // secondary ESOL

        public void MapToSearchAndCompareCourse(string courseTitle, string commaSeparatedUcasSubjects, string commaSeparatedExpectedSubjects)
        {
            var expected = commaSeparatedExpectedSubjects.Split(", ");
            var result = new SubjectMapper().GetSubjectList(courseTitle, commaSeparatedUcasSubjects.Split(", "));      

            result.Should().BeEquivalentTo(expected);      
        }

    }
}
