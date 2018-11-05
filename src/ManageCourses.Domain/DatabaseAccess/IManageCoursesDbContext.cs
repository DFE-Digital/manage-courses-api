using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

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
        DbSet<Session> Sessions { get; set; }
        DbSet<PgdeCourse> PgdeCourses { get; set; }

        List<Course> GetCourse(string instCode, string courseCode, string email);
        List<Course> GetCoursesByInstCode(string instCode, string email);
        IQueryable<OrganisationInstitution> GetOrganisationInstitutions(string email);
        OrganisationInstitution GetOrganisationInstitution(string email, string instCode);
        TResult GetOrganisationInstitution<TResult>(string email, string instCode, Func<OrganisationInstitution, string, string, TResult> mapping);
        Institution GetInstitution(string name, string instCode);

        IQueryable<User> GetUsers(string email);
        void Save();
    }
}
