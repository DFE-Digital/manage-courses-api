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
        DbSet<Organisation> Organisations { get; set; }
        DbSet<OrganisationInstitution> OrganisationIntitutions { get; set; }
        DbSet<OrganisationUser> OrganisationUsers { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<AccessRequest> AccessRequests { get; set; }
        DbSet<InstitutionEnrichment> InstitutionEnrichments { get; set; }
        DbSet<CourseEnrichment> CourseEnrichments { get; set; }
        DbSet<Session> Sessions { get;  set; }
        DbSet<PgdeCourse> PgdeCourses { get; set; }
        
        List<Course> GetUcasCourseRecordsByUcasCode(string instCode, string courseCode, string email);
        List<Course> GetUcasCourseRecordsByInstCode(string instCode, string email);
        IQueryable<OrganisationInstitution> GetUserOrganisations(string email);
        OrganisationInstitution GetUserOrganisation(string email, string instCode);

        Institution GetInstitution(string name, string instCode);

        IQueryable<User> GetMcUsers(string email);
        void Save();
    }
}
