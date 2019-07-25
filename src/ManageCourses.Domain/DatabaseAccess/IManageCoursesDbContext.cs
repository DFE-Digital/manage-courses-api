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
        DbSet<Provider> Providers { get; set; }
        DbSet<CourseSubject> CourseSubjects { get; set; }
        DbSet<Subject> Subjects { get; set; }
        DbSet<Site> Sites { get; set; }
        DbSet<Organisation> Organisations { get; set; }
        DbSet<OrganisationProvider> OrganisationProviders { get; set; }
        DbSet<OrganisationUser> OrganisationUsers { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<AccessRequest> AccessRequests { get; set; }
        DbSet<ProviderEnrichment> ProviderEnrichments { get; set; }
        DbSet<CourseEnrichment> CourseEnrichments { get; set; }
        DbSet<Session> Sessions { get; set; }

        void RunInRetryableTransaction (Action action);

        List<Course> GetCourse(string providerCode, string courseCode, string email);
        List<Course> GetCoursesByProviderCode(string providerCode, string email);
        IQueryable<OrganisationProvider> GetOrganisationProviders(string email);
        OrganisationProvider GetOrganisationProvider(string email, string providerCode);
        TResult GetOrganisationProvider<TResult>(string email, string providerCode, Func<OrganisationProvider, string, string, TResult> mapping);
        Provider GetProvider(string name, string providerCode);

        IQueryable<User> GetUsers(string email);
        void Save();
    }
}
