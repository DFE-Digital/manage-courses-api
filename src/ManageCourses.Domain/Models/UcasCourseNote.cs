using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasCourseNote
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CourseCode { get; set; }
        public string NoteNo { get; set; }
        public string NoteType { get; set; }
    }
}
