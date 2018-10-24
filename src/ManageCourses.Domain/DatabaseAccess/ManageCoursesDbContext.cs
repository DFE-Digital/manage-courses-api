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
            
            modelBuilder.Entity<McUser>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<McOrganisation>()
                .HasIndex(x => x.OrgId)
                .IsUnique();

            modelBuilder.Entity<McOrganisationUser>()
                .HasOne(ou => ou.McUser)
                .WithMany(u => u.McOrganisationUsers);

            modelBuilder.Entity<McOrganisationUser>()
                .HasOne(ou => ou.McOrganisation)
                .WithMany(u => u.McOrganisationUsers);

            modelBuilder.Entity<Institution>()
                .HasIndex(ui => ui.InstCode)
                .IsUnique();

            modelBuilder.Entity<Site>()
                .HasIndex(s => new {s.InstitutionId, s.Code})
                .IsUnique();

            modelBuilder.Entity<Site>()
                .HasOne(uc => uc.Institution)
                .WithMany(ui => ui.Sites);

            modelBuilder.Entity<McOrganisationInstitution>()
                .HasOne(oi => oi.McOrganisation)
                .WithMany(o => o.McOrganisationInstitutions);

            modelBuilder.Entity<McOrganisationInstitution>()
                .HasOne(oi => oi.Institution)
                .WithMany(ui => ui.McOrganisationInstitutions);

            modelBuilder.Entity<Course>()
                .HasIndex(x => new { x.InstitutionId, x.Id } )
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

            modelBuilder.Entity<Subject>()
                .HasIndex(x => x.SubjectCode)
                .IsUnique();

            modelBuilder.Entity<NctlOrganisation>()
                .HasOne(x => x.McOrganisation)                
                .WithMany(x => x.NctlOrganisations)
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
                    join institution i on c.institution_id = i.id
                    join mc_organisation_institution oi on oi.institution_id=i.id
                    join mc_organisation o on o.id = oi.mc_organisation_id 
                    join mc_organisation_user ou on ou.mc_organisation_id = o.id 
                    join mc_user u on u.id = ou.mc_user_id 
                    where lower(i.inst_code)=lower(@instCode) 
                    and lower(c.course_code)=lower(@ucasCode) 
                    and lower(u.email)=lower(@email)", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("ucasCode", ucasCode), new NpgsqlParameter("email", email))
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
                    $"join institution i on i.id=c.institution_id " +
                    $"join mc_organisation_institution oi on oi.institution_id=i.id " +
                    $"join mc_organisation o on o.id = oi.mc_organisation_id " +
                    $"join mc_organisation_user ou on ou.mc_organisation_id = o.id " +
                    $"join mc_user u on ou.mc_user_id = u.id " +
                    $"where lower(i.inst_code)=lower(@instCode) " +
                    $"and lower(u.email)=lower(@email) order by c.name", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("email", email))
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
                $"join mc_organisation o on o.id = oi.mc_organisation_id " +
                $"join mc_organisation_user ou on ou.mc_organisation_id = o.id " +
                $"join mc_user u on ou.mc_user_id = u.id " +
                $"where lower(u.email) = lower(@email)",
                new NpgsqlParameter("email", email)
            ).Include(x => x.McOrganisation)
            .Include(x => x.Institution);

            return userOrganisations;
        }

        public McOrganisationInstitution GetUserOrganisation(string email, string instCode)
        {
            var userOrganisations = McOrganisationIntitutions.FromSql(
                $"select oi.* from mc_organisation_institution oi " +
                $"join institution i on oi.institution_id = i.id " +
                $"join mc_organisation o on oi.mc_organisation_id = o.id " +
                $"join mc_organisation_user ou on ou.mc_organisation_id = o.id " +
                $"join mc_user u on ou.mc_user_id = u.id " +
                $"where lower(u.email) = lower(@email) and Lower(i.inst_code) = lower(@instCode)",
                new NpgsqlParameter("email", email), new NpgsqlParameter("instCode", instCode)
            ).Include(x => x.McOrganisation).Include(x => x.Institution).FirstOrDefault();

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
                    JOIN mc_organisation_institution oi on i.id = oi.institution_id
                    JOIN mc_organisation o on oi.mc_organisation_id = o.id
                    JOIN mc_organisation_user ou on o.id = ou.mc_organisation_id
                    JOIN mc_user u on ou.mc_user_id = u.id
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
