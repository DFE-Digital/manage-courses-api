using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasInstitution
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string InstName { get; set; }
        public string InstBig { get; set; }
        public string InstFull { get; set; }
        public string InstType { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public string Postcode { get; set; }
        public string ContactName { get; set; }
        public string Url { get; set; }
        public string YearCode { get; set; }
        public string Scitt { get; set; }
        public string AccreditingProvider { get; set; }
        public string SchemeMember { get; set; }

        public ICollection<McOrganisationInstitution> McOrganisationInstitutions { get; set; }
        public ICollection<UcasCourse> UcasCourses { get; set; }
        public ICollection<UcasCourse> AccreditedUcasCourses { get; set; }
        public ICollection<UcasCourseSubject> UcasCourseSubjects { get; set; }
        public ICollection<UcasCampus> UcasCampuses { get; set; }
        public ICollection<CourseCode> CourseCodes { get; set; }
    }
}
