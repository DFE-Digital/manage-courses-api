using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasNoteText
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string NoteNo { get; set; }
        public string NoteType { get; set; }
        public string LineText { get; set; }
    }
}
