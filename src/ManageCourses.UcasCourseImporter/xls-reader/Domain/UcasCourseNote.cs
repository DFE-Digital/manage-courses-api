using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Xls.Domain
{
    public class UcasCourseNote
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CrseCode { get; set; }
        public string NoteNo { get; set; }
        public string NoteType { get; set; }
        public string YearCode { get; set; }
    }
}
