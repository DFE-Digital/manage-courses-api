using System.Collections.Generic;
using GovUk.Education.ManageCourses;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    public class UcasPayload
    {
        public UcasPayload()
        {
            this.Institutions = new List<UcasInstitution>();
            this.Courses = new List<UcasCourse>();
            this.Subjects = new List<UcasSubject>();
            this.CourseSubjects = new List<UcasCourseSubject>();
            this.Campuses = new List<UcasCampus>();
            this.CourseNotes = new List<UcasCourseNote>();
            this.NoteTexts = new List<UcasNoteText>();
        }

        public IEnumerable<UcasInstitution> Institutions { get; set; }
        public IEnumerable<UcasCourse> Courses { get; set; }
        public IEnumerable<UcasSubject> Subjects { get; set; }
        public IEnumerable<UcasCourseSubject> CourseSubjects { get; set; }
        public IEnumerable<UcasCampus> Campuses { get; set; }
        public IEnumerable<UcasCourseNote> CourseNotes { get; set; }
        public IEnumerable<UcasNoteText> NoteTexts { get; set; }
    }
}
