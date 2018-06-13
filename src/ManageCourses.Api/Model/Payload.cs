using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class Payload
    {
        public IEnumerable<UcasCourse> Courses { get; set; }
        public IEnumerable<UcasInstitution> Institutions { get; set; }
        public IEnumerable<UcasSubject> Subjects { get; set; }
        public IEnumerable<UcasCampus> Campuses { get; set; }
        public IEnumerable<UcasCourseNote> CourseNotes { get; set; }
        public IEnumerable<UcasNoteText> NoteTexts { get; set; }
    }
}
