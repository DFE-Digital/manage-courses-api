using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasSubject
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectDescription { get; set; }
        public string TitleMatch { get; set; }
    }
}
