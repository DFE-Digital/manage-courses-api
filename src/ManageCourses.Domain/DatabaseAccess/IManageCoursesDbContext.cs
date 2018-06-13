using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    public interface IManageCoursesDbContext
    {
        DbSet<Course> Courses { get; set; }
        DbSet<UcasCourse> UcasCourses { get; set; }
        DbSet<UcasInstitution> UcasInstitutions { get; set; }
        DbSet<UcasSubject> UcasSubjects { get; set; }
        DbSet<UcasCampus> UcasCampuses { get; set; }
        DbSet<UcasCourseNote> UcasCourseNotes { get; set; }
        DbSet<UcasNoteText> UcasNoteTexts { get; set; }
        IList<Course> GetAll();
        IList<UcasCourse> GetAllUcasCourses();
        IList<UcasInstitution> GetAllUcasInstitutions();
        IList<UcasSubject> GetAllUcasSubjects();
        IList<UcasCampus> GetAllUcasCampuses();
        IList<UcasCourseNote> GetAllUcasCourseNotes();
        IList<UcasNoteText> GetAllUcasNoteTexts();
        void AddUcasCourse(UcasCourse course);
        void AddUcasInstitution(UcasInstitution institution);
        void AddUcasSubject(UcasSubject subject);
        void AddUcasCampus(UcasCampus campus);
        void AddUcasCourseNote(UcasCourseNote courseNote);
        void AddUcasNoteText(UcasNoteText noteText);
        void Save();
    }
}
