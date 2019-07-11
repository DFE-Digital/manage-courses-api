using System.Collections.Generic;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using GovUk.Education.ManageCourses.Xls.Domain;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public static class TestPayloadBuilder {
        public static UcasPayload MakeSimpleUcasPayload()
        {
            return new UcasPayload
            {
                Institutions = new List<UcasInstitution>
                {
                    new UcasInstitution
                    {
                        InstFull = "Joe's school @ UCAS",
                        InstCode = "ABC"
                    },
                },
                Campuses = new List<UcasCampus>{
                    new UcasCampus
                    {
                        InstCode = "ABC",
                        CampusCode = "",
                        CampusName = "Main campus site"
                    }
                },
                Courses = new List<UcasCourse>{
                    new UcasCourse
                    {
                        InstCode = "ABC",
                        CampusCode = "",
                        CrseCode = "XYZ",
                        CrseTitle = "Joe's course for Primary teachers"
                    },
                    new UcasCourse
                    {
                        InstCode = "ABC",
                        CampusCode = "",
                        CrseCode = "CC101",
                        CrseTitle = "Chelsea Tractor"
                    },
                },
            };
        }
    }
}
