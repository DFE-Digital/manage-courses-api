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

            modelBuilder.Entity<McOrganisationUser>()
                .HasIndex(ou => new { ou.Email, ou.OrgId })
                .IsUnique();
            modelBuilder.Entity<McOrganisationUser>()
                .HasOne(ou => ou.McUser)
                .WithMany(u => u.McOrganisationUsers)
                .HasForeignKey(ou => ou.Email)
                .HasPrincipalKey(u => u.Email);
            modelBuilder.Entity<McOrganisationUser>()
                .HasOne(ou => ou.McOrganisation)
                .WithMany(u => u.McOrganisationUsers)
                .HasForeignKey(ou => ou.OrgId)
                .HasPrincipalKey(u => u.OrgId);

            modelBuilder.Entity<Institution>()
                .HasIndex(ui => ui.InstCode)
                .IsUnique();

            modelBuilder.Entity<Site>()
                .HasOne(uc => uc.Institution)
                .WithMany(ui => ui.Sites)
                .HasForeignKey(uc => uc.InstCode)
                .HasPrincipalKey(ui => ui.InstCode);

            modelBuilder.Entity<McOrganisationInstitution>()
                .HasIndex(oi => new { oi.OrgId, oi.InstCode })
                .IsUnique();
            modelBuilder.Entity<McOrganisationInstitution>()
                .HasOne(oi => oi.McOrganisation)
                .WithMany(o => o.McOrganisationInstitutions)
                .HasForeignKey(oi => oi.OrgId)
                .HasPrincipalKey(o => o.OrgId);
            modelBuilder.Entity<McOrganisationInstitution>()
                .HasOne(oi => oi.Institution)
                .WithMany(ui => ui.McOrganisationInstitutions)
                .HasForeignKey(oi => oi.InstCode)
                .HasPrincipalKey(ui => ui.InstCode);

            modelBuilder.Entity<Course>()
                .HasIndex(oi => new { oi.InstCode, oi.CourseCode })
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasOne(uc => uc.Institution)
                .WithMany(ui => ui.Courses)
                .HasForeignKey(uc => uc.InstCode)
                .HasPrincipalKey(ui => ui.InstCode);
            modelBuilder.Entity<Course>()
                .HasOne(uc => uc.AccreditingInstitution)
                .WithMany(ui => ui.AccreditedCourses)
                .HasForeignKey(uc => uc.AccreditingInstCode)
                .HasPrincipalKey(ui => ui.InstCode);

            modelBuilder.Entity<CourseSite>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSites)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CourseSite>()
                .HasOne(cs => cs.Site)
                .WithMany(c => c.CourseSites)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Site>()
                .HasOne(s => s.Institution)
                .WithMany(i => i.Sites)
                .HasForeignKey(s => s.InstCode)
                .HasPrincipalKey(i => i.InstCode);

            modelBuilder.Entity<CourseSubject>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSubjects)
                .OnDelete(DeleteBehavior.Cascade);                
            modelBuilder.Entity<CourseSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.CourseSubjects)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NctlOrganisation>()
                .HasOne(x => x.McOrganisation)                
                .WithMany(x => x.NctlOrganisations)
                .HasForeignKey(x => x.OrgId)
                .HasPrincipalKey(x => x.OrgId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccessRequest>()
                .HasOne(ar => ar.Requester)
                .WithMany(u => u.AccessRequests)
                .HasForeignKey(ar => ar.RequesterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<InstitutionEnrichment>()
                .HasIndex(x => x.InstCode);
                
            modelBuilder.Entity<McSession>()
                .HasOne(x => x.McUser)
                .WithMany(u => u.Sessions)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<McSession>()
                .HasIndex(x => new {x.AccessToken, x.CreatedUtc});


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
        public DbSet<McOrganisation> McOrganisations { get; set; }
        public DbSet<McOrganisationInstitution> McOrganisationIntitutions { get; set; }
        public DbSet<McOrganisationUser> McOrganisationUsers { get; set; }
        public DbSet<McUser> McUsers { get; set; }
        public DbSet<AccessRequest> AccessRequests { get; set; }
        public DbSet<InstitutionEnrichment> InstitutionEnrichments { get; set; }
        public DbSet<CourseEnrichment> CourseEnrichments { get; set; }
        public DbSet<McSession> McSessions { get;  set; }
        public DbSet<PgdeCourse> PgdeCourses { get; set; }

        public List<Course> GetUcasCourseRecordsByUcasCode(string instCode, string ucasCode, string email)
        {
            var ucasCourses = Courses.FromSql(@"
                    select c.* from course c
                    join mc_organisation_institution oi on oi.inst_code=c.inst_code 
                    join mc_organisation_user ou on ou.org_id=oi.org_id 
                    where lower(c.inst_code)=lower(@instCode) 
                    and lower(c.course_code)=lower(@ucasCode) 
                    and lower(ou.email)=lower(@email)", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("ucasCode", ucasCode), new NpgsqlParameter("email", email))
                .Include(x => x.Institution)
                .Include(x => x.CourseSubjects).ThenInclude(x => x.Subject)
                .Include(x => x.AccreditingInstitution)
                .Include(x => x.CourseSites).ThenInclude(x => x.Site)
                .ToList();

            return ucasCourses;
        }
        public List<Course> GetUcasCourseRecordsByInstCode(string instCode, string email)
        {
            var ucasCourses = Courses.FromSql(
                    $"select c.* from course c " +
                    $"join mc_organisation_institution oi on oi.inst_code=c.inst_code " +
                    $"join mc_organisation_user ou on ou.org_id=oi.org_id " +
                    $"where lower(c.inst_code)=lower(@instCode) " +
                    $"and lower(ou.email)=lower(@email) order by c.name", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("email", email))
                .Include(x => x.Institution)
                .Include(x => x.CourseSubjects).ThenInclude(x => x.Subject)
                .Include(x => x.AccreditingInstitution)
                .Include(x => x.CourseSites).ThenInclude(x => x.Site)
                .ToList();

            return ucasCourses;
        }

        public IQueryable<McOrganisationInstitution> GetUserOrganisations(string email)
        {
            var userOrganisations = McOrganisationIntitutions.FromSql(
                $"select oi.* from mc_organisation_institution oi " +
                $"join mc_organisation_user ou on ou.org_id = oi.org_id " +
                $"where lower(ou.email) = lower(@email)",
                new NpgsqlParameter("email", email)
            );

            return userOrganisations;
        }

        public McOrganisationInstitution GetUserOrganisation(string email, string instCode)
        {
            var userOrganisations = McOrganisationIntitutions.FromSql(
                $"select oi.* from mc_organisation_institution oi " +
                $"join mc_organisation_user ou on ou.org_id = oi.org_id " +
                $"where lower(ou.email) = lower(@email) and Lower(oi.inst_code) = lower(@instCode)",
                new NpgsqlParameter("email", email), new NpgsqlParameter("instCode", instCode)
            ).FirstOrDefault();

            return userOrganisations;
        }

        public IQueryable<McUser> GetMcUsers(string email)
        {
            var users = McUsers.FromSql(
                $"select * from mc_user " +
                $"where lower(email) = lower(@email)",
                new NpgsqlParameter("email", email)
            );

            return users;
        }

        public Institution GetInstitution(string name, string instCode)
        {
            return Institutions.FromSql(@"
                    SELECT i.* from institution i
                    JOIN mc_organisation_institution oi on i.inst_code = oi.inst_code
                    JOIN mc_organisation_user ou on oi.org_id = ou.org_id
                    WHERE lower(ou.email) = lower(@email)
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
