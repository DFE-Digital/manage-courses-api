using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess {
    public class ManageCoursesDbContext : DbContext, IManageCoursesDbContext {
        public ManageCoursesDbContext (DbContextOptions<ManageCoursesDbContext> options) : base (options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<UcasCourse> UcasCourses { get; set; }
        public DbSet<UcasInstitution> UcasInstitutions { get; set; }
        public DbSet<UcasSubject> UcasSubjects { get; set; }
        public DbSet<UcasCampus> UcasCampuses { get; set; }
        public DbSet<UcasCourseNote> UcasCourseNotes { get; set; }
        public DbSet<UcasNoteText> UcasNoteTexts { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderMapper> ProviderMapper { get; set; }
       
        public IList<Course> GetAll () {

            return Courses.ToList();
        }

        public IList<UcasCourse> GetAllUcasCourses()
        {
            return UcasCourses.ToList();
        }
        public IList<UcasInstitution> GetAllUcasInstitutions()
        {
            return UcasInstitutions.ToList();
        }

        public IList<UcasSubject> GetAllUcasSubjects()
        {
            return UcasSubjects.ToList();
        }

        public IList<UcasCampus> GetAllUcasCampuses()
        {
            return UcasCampuses.ToList();
        }

        public IList<UcasCourseNote> GetAllUcasCourseNotes()
        {
            return UcasCourseNotes.ToList();
        }

        public IList<UcasNoteText> GetAllUcasNoteTexts()
        {
            return UcasNoteTexts.ToList();
        }

        public void AddUcasInstitution(UcasInstitution institution)
        {
            UcasInstitutions.Add(institution);
        }

        public void AddUcasSubject(UcasSubject subject)
        {
            UcasSubjects.Add(subject);
        }

        public void AddUcasCampus(UcasCampus campus)
        {
            UcasCampuses.Add(campus);
        }

        public void AddUcasCourseNote(UcasCourseNote courseNote)
        {
            UcasCourseNotes.Add(courseNote);
        }

        public void AddUcasNoteText(UcasNoteText noteText)
        {
            UcasNoteTexts.Add(noteText);
        }

        public void AddUcasCourse(UcasCourse course)
        {
            UcasCourses.Add(course);
        }

        public void AddCourse(Course course)
        {
            Courses.Add(course);
        }
        public void Save()
        {
            SaveChanges();
        }
    }
}
