using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    public class ManageCoursesDbContext : DbContext, IManageCoursesDbContext
    {
        public ManageCoursesDbContext(DbContextOptions<ManageCoursesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var e in modelBuilder.Model.GetEntityTypes())
            {
                e.Relational().TableName = PascalToSnakeCase(e.DisplayName());
                foreach (var p in e.GetProperties())
                {
                    p.Relational().ColumnName = PascalToSnakeCase(p.Name);
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Converts CamelCase to underscore_separated
        /// To avoid having to quote everything when writing queries.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string PascalToSnakeCase(string value)
        {
            return Regex.Replace(value, @"([a-z\d])([A-Z])", "$1_$2").ToLower();
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<UcasCourse> UcasCourses { get; set; }
        public DbSet<UcasInstitution> UcasInstitutions { get; set; }
        public DbSet<UcasCourseSubject> UcasCourseSubjects { get; set; }
        public DbSet<UcasSubject> UcasSubjects { get; set; }
        public DbSet<UcasCampus> UcasCampuses { get; set; }
        public DbSet<UcasCourseNote> UcasCourseNotes { get; set; }
        public DbSet<UcasNoteText> UcasNoteTexts { get; set; }
        public DbSet<McOrganisation> McOrganisations { get; set; }
        public DbSet<McOrganisationInstitution> McOrganisationIntitutions { get; set; }
        public DbSet<McOrganisationUser> McOrganisationUsers { get; set; }
        public DbSet<McUser> McUsers { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderMapper> ProviderMapper { get; set; }

        public IList<Course> GetAll()
        {

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

        public IList<UcasCourseSubject> GetAllUcasCourseSubjects()
        {
            return UcasCourseSubjects.ToList();
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

        public IList<McOrganisation> GetAllMcOrganisations()
        {
            return McOrganisations.ToList();
        }

        public IList<McOrganisationInstitution> GetAllMcOrganisationsInstitutions()
        {
            return McOrganisationIntitutions.ToList();
        }

        public IList<McOrganisationUser> GetAllMcOrganisationsUsers()
        {
            return McOrganisationUsers.ToList();
        }

        public IList<McUser> GetAllMcUsers()
        {
            return McUsers.ToList();
        }

        public void AddUcasInstitution(UcasInstitution institution)
        {
            UcasInstitutions.Add(institution);
        }

        public void AddUcasSubject(UcasSubject subject)
        {
            UcasSubjects.Add(subject);
        }

        public void AddUcasCourseSubject(UcasCourseSubject courseSubject)
        {
            UcasCourseSubjects.Add(courseSubject);
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

        public void AddMcOrganisation(McOrganisation organisation)
        {
            McOrganisations.Add(organisation);
        }

        public void AddMcOrganisationInstitution(McOrganisationInstitution organisationInstitution)
        {
            McOrganisationIntitutions.Add(organisationInstitution);
        }

        public void AddMcOrganisationUser(McOrganisationUser organisationUser)
        {
            McOrganisationUsers.Add(organisationUser);
        }

        public void AddMcUser(McUser user)
        {
            McUsers.Add(user);
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
