using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasCourseSubject
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CrseCode { get; set; }
        public string SubjectCode { get; set; }
        public string YearCode { get; set; }
    }
}
