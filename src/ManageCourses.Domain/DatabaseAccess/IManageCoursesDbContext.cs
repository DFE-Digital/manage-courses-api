using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    public interface IManageCoursesDbContext
    {
        DbSet<Course> Courses { get; set; }
        DbSet<Institution> Institutions { get; set; }
        DbSet<CourseSubject> CourseSubjects { get; set; }
        DbSet<Subject> Subjects { get; set; }
        DbSet<Site> Sites { get; set; }
        DbSet<McOrganisation> McOrganisations { get; set; }
        DbSet<McOrganisationInstitution> McOrganisationIntitutions { get; set; }
        DbSet<McOrganisationUser> McOrganisationUsers { get; set; }
        DbSet<McUser> McUsers { get; set; }
        DbSet<AccessRequest> AccessRequests { get; set; }
        DbSet<InstitutionEnrichment> InstitutionEnrichments { get; set; }
        DbSet<CourseEnrichment> CourseEnrichments { get; set; }
        DbSet<McSession> McSessions { get;  set; }
        DbSet<PgdeCourse> PgdeCourses { get; set; }
        
        List<Course> GetUcasCourseRecordsByUcasCode(string instCode, string courseCode, string email);
        List<Course> GetUcasCourseRecordsByInstCode(string instCode, string email);
        IQueryable<McOrganisationInstitution> GetUserOrganisations(string email);
        McOrganisationInstitution GetUserOrganisation(string email, string instCode);

        Institution GetInstitution(string name, string instCode);

        IQueryable<McUser> GetMcUsers(string email);
        void Save();
    }
}
