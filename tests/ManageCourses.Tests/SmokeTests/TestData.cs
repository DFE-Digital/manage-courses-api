using System.Collections.ObjectModel;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Tests.SmokeTests
{
    public static class TestData {
            
        public static ReferenceDataPayload MakeReferenceDataPayload(string username) => new ReferenceDataPayload {
            
                Users = ListOfOne(new McUser{
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = username
                }),

                Organisations = ListOfOne(new McOrganisation {
                    Name = "Joe's school",
                    OrgId = "123"
                }),

                OrganisationUsers = ListOfOne(new McOrganisationUser {
                    Email = username,
                    OrgId = "123"
                }),

                OrganisationInstitutions = ListOfOne(new McOrganisationInstitution {
                    InstitutionCode = "ABC",
                    OrgId = "123"
                }),
                
                Institutions = ListOfOne(new UcasInstitution { 
                    InstFull = "Joe's school @ UCAS",
                    InstCode = "ABC"
                })
            };
        

        public static UcasPayload MakeSimpleUcasPayload() => new UcasPayload {
                            
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
        
        
        private static ObservableCollection<T> ListOfOne<T> (T one) {
            return new ObservableCollection<T> { one };
        }
    }
}