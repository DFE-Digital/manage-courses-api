using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using GovUk.Education.ManageCourses.Xls.Domain;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public static class TestPayloadBuilder {
        public static UcasPayload MakeSimpleUcasPayload() => new UcasPayload {

                Institutions = ListOfOne(new UcasInstitution {
                    InstFull = "Joe's school @ UCAS",
                    InstCode = "ABC"
                }),
                            
                Campuses = ListOfOne(new UcasCampus {
                    InstCode = "ABC",
                    CampusCode = "", // NOTE: EMPTY STRING
                    CampusName = "Main campus site"
                }),

                Courses = ListOfOne(new UcasCourse {
                    InstCode = "ABC",
                    CampusCode = "",
                    CrseCode = "XYZ",
                    CrseTitle = "Joe's course for Primary teachers"
                })
            };
        
        
        private static List<T> ListOfOne<T> (T one) {
            return new List<T> { one };
        }
    }
}