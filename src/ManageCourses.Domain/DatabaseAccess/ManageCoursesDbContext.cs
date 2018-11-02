using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

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

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Organisation>()
                .HasIndex(x => x.OrgId)
                .IsUnique();

            modelBuilder.Entity<OrganisationUser>()
                .HasOne(ou => ou.User)
                .WithMany(u => u.OrganisationUsers);

            modelBuilder.Entity<OrganisationUser>()
                .HasOne(ou => ou.Organisation)
                .WithMany(u => u.OrganisationUsers);

            modelBuilder.Entity<Institution>()
                .HasIndex(ui => ui.ProviderCode)
                .IsUnique();

            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.SubjectCode)
                .IsUnique();

            modelBuilder.Entity<Site>()
                .HasIndex(s => new { s.InstitutionId, s.Code })
                .IsUnique();

            modelBuilder.Entity<Site>()
                .HasOne(uc => uc.Institution)
                .WithMany(ui => ui.Sites);

            modelBuilder.Entity<OrganisationProvider>()
                .HasOne(oi => oi.Organisation)
                .WithMany(o => o.OrganisationInstitutions);

            modelBuilder.Entity<OrganisationProvider>()
                .HasOne(oi => oi.Provider)
                .WithMany(ui => ui.OrganisationProviders);

            modelBuilder.Entity<Course>()
                .HasIndex(x => new { x.InstitutionId, x.CourseCode } )
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasOne(uc => uc.Institution)
                .WithMany(ui => ui.Courses)
                .HasForeignKey(uc => uc.InstitutionId);

            modelBuilder.Entity<Course>()
                .HasOne(uc => uc.AccreditingInstitution)
                .WithMany(ui => ui.AccreditedCourses)
                .IsRequired(false);

            modelBuilder.Entity<CourseSite>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSites)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CourseSite>()
                .HasOne(cs => cs.Site)
                .WithMany(c => c.CourseSites)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseSubject>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSubjects)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CourseSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.CourseSubjects)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NctlOrganisation>()
                .HasOne(x => x.Organisation)
                .WithMany(x => x.NctlOrganisations)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccessRequest>()
                .HasOne(ar => ar.Requester)
                .WithMany(u => u.AccessRequests)
                .HasForeignKey(ar => ar.RequesterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<InstitutionEnrichment>()
                .HasIndex(x => x.InstCode);

            modelBuilder.Entity<Session>()
                .HasOne(x => x.User)
                .WithMany(u => u.Sessions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .HasIndex(x => new { x.AccessToken, x.CreatedUtc });

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
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<CourseSubject> CourseSubjects { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<NctlOrganisation> NctlOrganisations { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationProvider> OrganisationIntitutions { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AccessRequest> AccessRequests { get; set; }
        public DbSet<InstitutionEnrichment> InstitutionEnrichments { get; set; }
        public DbSet<CourseEnrichment> CourseEnrichments { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<PgdeCourse> PgdeCourses { get; set; }

        public List<Course> GetCourse(string instCode, string courseCode, string email)
        {
            var ucasCourses = Courses.FromSql(@"
                    select c.* from course c
                    join institution i on c.institution_id = i.id
                    join organisation_institution oi on oi.institution_id=i.id
                    join organisation o on o.id = oi.organisation_id 
                    join organisation_user ou on ou.organisation_id = o.id 
                    join ""user"" u on u.id = ou.user_id 
                    where lower(i.inst_code)=lower(@instCode) 
                    and lower(c.course_code)=lower(@courseCode) 
                    and lower(u.email)=lower(@email)", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("courseCode", courseCode), new NpgsqlParameter("email", email))
                .Include(x => x.Institution)
                .Include(x => x.CourseSubjects).ThenInclude(x => x.Subject)
                .Include(x => x.AccreditingInstitution)
                .Include(x => x.CourseSites).ThenInclude(x => x.Site)
                .ToList();

            return ucasCourses;
        }
        public List<Course> GetCoursesByInstCode(string instCode, string email)
        {
            var ucasCourses = Courses.FromSql(
                    $"select c.* from course c " +
                    $"join institution i on i.id=c.institution_id " +
                    $"join organisation_institution oi on oi.institution_id=i.id " +
                    $"join organisation o on o.id = oi.organisation_id " +
                    $"join organisation_user ou on ou.organisation_id = o.id " +
                    $"join \"user\" u on ou.user_id = u.id " +
                    $"where lower(i.inst_code)=lower(@instCode) " +
                    $"and lower(u.email)=lower(@email) order by c.name", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("email", email))
                .Include(x => x.Institution)
                .Include(x => x.CourseSubjects).ThenInclude(x => x.Subject)
                .Include(x => x.AccreditingInstitution)
                .Include(x => x.CourseSites).ThenInclude(x => x.Site)
                .ToList();

            return ucasCourses;
        }

        public IQueryable<OrganisationProvider> GetOrganisationInstitutions(string email)
        {
            var userOrganisations = OrganisationIntitutions.FromSql(
                $"select oi.* from organisation_institution oi " +
                $"join organisation o on o.id = oi.organisation_id " +
                $"join organisation_user ou on ou.organisation_id = o.id " +
                $"join \"user\" u on ou.user_id = u.id " +
                $"where lower(u.email) = lower(@email)",
                new NpgsqlParameter("email", email)
            ).Include(x => x.Organisation)
            .Include(x => x.Provider);

            return userOrganisations;
        }

        public OrganisationProvider GetOrganisationInstitution(string email, string instCode)
        {
            var userOrganisations = OrganisationIntitutions.FromSql(
                $"select oi.* from organisation_institution oi " +
                $"join institution i on oi.institution_id = i.id " +
                $"join organisation o on oi.organisation_id = o.id " +
                $"join organisation_user ou on ou.organisation_id = o.id " +
                $"join \"user\" u on ou.user_id = u.id " +
                $"where lower(u.email) = lower(@email) and Lower(i.inst_code) = lower(@instCode)",
                new NpgsqlParameter("email", email), new NpgsqlParameter("instCode", instCode)
            ).Include(x => x.Organisation).Include(x => x.Provider).FirstOrDefault();

            return userOrganisations;
        }

        public IQueryable<User> GetUsers(string email)
        {
            var users = Users.FromSql(
                $"select * from \"user\" " +
                $"where lower(email) = lower(@email)",
                new NpgsqlParameter("email", email)
            );

            return users;
        }

        public Institution GetInstitution(string name, string instCode)
        {
            return Institutions.FromSql(@"
                    SELECT i.* from institution i
                    JOIN organisation_institution oi on i.id = oi.institution_id
                    JOIN organisation o on oi.organisation_id = o.id
                    JOIN organisation_user ou on o.id = ou.organisation_id
                    JOIN ""user"" u on ou.user_id = u.id
                    WHERE lower(u.email) = lower(@email)
                    AND lower(i.inst_code) = lower(@instcode)",
                    new NpgsqlParameter("email", name),
                    new NpgsqlParameter("instcode", instCode))
                .FirstOrDefault();
        }

        public void Save()
        {
            SaveChanges();
        }
    }
}
