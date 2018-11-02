using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    public interface IManageCoursesDbContext
    {
        DbSet<Course> Courses { get; set; }
        DbSet<Provider> Institutions { get; set; }
        DbSet<CourseSubject> CourseSubjects { get; set; }
        DbSet<Subject> Subjects { get; set; }
        DbSet<Site> Sites { get; set; }
        DbSet<Organisation> Organisations { get; set; }
        DbSet<OrganisationProvider> OrganisationIntitutions { get; set; }
        DbSet<OrganisationUser> OrganisationUsers { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<AccessRequest> AccessRequests { get; set; }
        DbSet<ProviderEnrichment> InstitutionEnrichments { get; set; }
        DbSet<CourseEnrichment> CourseEnrichments { get; set; }
        DbSet<Session> Sessions { get; set; }
        DbSet<PgdeCourse> PgdeCourses { get; set; }

        List<Course> GetCourse(string instCode, string courseCode, string email);
        List<Course> GetCoursesByInstCode(string instCode, string email);
        IQueryable<OrganisationProvider> GetOrganisationInstitutions(string email);
        OrganisationProvider GetOrganisationInstitution(string email, string instCode);

        Provider GetInstitution(string name, string instCode);

        IQueryable<User> GetUsers(string email);
        void Save();
    }
}
