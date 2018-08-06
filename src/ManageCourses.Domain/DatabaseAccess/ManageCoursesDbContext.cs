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

            modelBuilder.Entity<UcasInstitution>()
                .HasIndex(ui => ui.InstCode)
                .IsUnique();

            modelBuilder.Entity<UcasCampus>()
                .HasOne(uc => uc.UcasInstitution)
                .WithMany(ui => ui.UcasCampuses)
                .HasForeignKey(uc => uc.InstCode)
                .HasPrincipalKey(ui => ui.InstCode);

            modelBuilder.Entity<McOrganisationInstitution>()
                .HasIndex(oi => new { oi.OrgId, oi.InstitutionCode })
                .IsUnique();
            modelBuilder.Entity<McOrganisationInstitution>()
                .HasOne(oi => oi.McOrganisation)
                .WithMany(o => o.McOrganisationInstitutions)
                .HasForeignKey(oi => oi.OrgId)
                .HasPrincipalKey(o => o.OrgId);
            modelBuilder.Entity<McOrganisationInstitution>()
                .HasOne(oi => oi.UcasInstitution)
                .WithMany(ui => ui.McOrganisationInstitutions)
                .HasForeignKey(oi => oi.InstitutionCode)
                .HasPrincipalKey(ui => ui.InstCode);

            modelBuilder.Entity<UcasCourse>()
                .HasIndex(oi => new { oi.InstCode, oi.CrseCode, oi.CampusCode })
                .IsUnique();
            modelBuilder.Entity<UcasCourse>()
                .HasOne(uc => uc.UcasInstitution)
                .WithMany(ui => ui.UcasCourses)
                .HasForeignKey(uc => uc.InstCode)
                .HasPrincipalKey(ui => ui.InstCode);
            modelBuilder.Entity<UcasCourse>()
                .HasOne(uc => uc.CourseCode)
                .WithMany(cc => cc.UcasCourses)
                .HasForeignKey(uc => new { uc.InstCode, uc.CrseCode })
                .HasPrincipalKey(cc => new { cc.InstCode, cc.CrseCode });
            modelBuilder.Entity<UcasCourse>()
                .HasOne(uc => uc.AccreditingProviderInstitution)
                .WithMany(ui => ui.AccreditedUcasCourses)
                .HasForeignKey(uc => uc.AccreditingProvider)
                .HasPrincipalKey(ui => ui.InstCode);
            modelBuilder.Entity<UcasCourse>()
                .HasOne(uc => uc.UcasCampus)
                .WithMany(ui => ui.UcasCourses)
                .HasForeignKey(ucs => new { ucs.InstCode, ucs.CampusCode })
                .HasPrincipalKey(ui => new { ui.InstCode, ui.CampusCode });

            modelBuilder.Entity<CourseCode>()
                .HasOne(cc => cc.UcasInstitution)
                .WithMany(ui => ui.CourseCodes)
                .HasForeignKey(oi => oi.InstCode)
                .HasPrincipalKey(ui => ui.InstCode);

            modelBuilder.Entity<UcasCourseSubject>()
                .HasOne(ucs => ucs.UcasInstitution)
                .WithMany(ui => ui.UcasCourseSubjects)
                .HasForeignKey(ucs => ucs.InstCode)
                .HasPrincipalKey(ui => ui.InstCode);
            modelBuilder.Entity<UcasCourseSubject>()
                .HasOne(ucs => ucs.UcasSubject)
                .WithMany(us => us.UcasCourseSubjects)
                .HasForeignKey(ucs => ucs.SubjectCode)
                .HasPrincipalKey(ui => ui.SubjectCode);
            modelBuilder.Entity<UcasCourseSubject>()
                .HasOne(ucs => ucs.CourseCode)
                .WithMany(cc => cc.UcasCourseSubjects)
                .HasForeignKey(ucs => new { ucs.InstCode, ucs.CrseCode })
                .HasPrincipalKey(cc => new { cc.InstCode, cc.CrseCode });

            modelBuilder.Entity<AccessRequest>()
                .HasOne(ar => ar.Requester)
                .WithMany(u => u.AccessRequests)
                .HasForeignKey(ar => ar.RequesterId)
                .OnDelete(DeleteBehavior.SetNull);

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

        public DbSet<UcasCourse> UcasCourses { get; set; }
        public DbSet<CourseCode> CourseCodes { get; set; }
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
        public DbSet<AccessRequest> AccessRequests { get; set; }

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
        public List<UcasCourse> GetUcasCourseRecordsByUcasCode(string instCode, string ucasCode, string email)
        {
            var ucasCourses = UcasCourses.FromSql(
                    $"select c.* from ucas_course c " +
                    $"join mc_organisation_institution oi on oi.institution_code=c.inst_code " +
                    $"join mc_organisation_user ou on ou.org_id=oi.org_id " +
                    $"where lower(c.inst_code)=lower(@instCode) " +
                    $"and lower(c.crse_code)=lower(@ucasCode) " +
                    $"and lower(ou.email)=lower(@email)", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("ucasCode", ucasCode), new NpgsqlParameter("email", email))
                .Include(x => x.UcasInstitution)
                .Include(x => x.UcasInstitution.UcasCourseSubjects).ThenInclude(x => x.UcasSubject)
                .Include(x => x.AccreditingProviderInstitution)
                .Include(x => x.UcasCampus)
                .ToList();

            return ucasCourses;
        }
        public List<UcasCourse> GetUcasCourseRecordsByInstCode(string instCode, string email)
        {
            var ucasCourses = UcasCourses.FromSql(
                    $"select c.* from ucas_course c " +
                    $"join mc_organisation_institution oi on oi.institution_code=c.inst_code " +
                    $"join mc_organisation_user ou on ou.org_id=oi.org_id " +
                    $"where lower(c.inst_code)=lower(@instCode) " +
                    $"and lower(ou.email)=lower(@email) order by c.crse_title", new NpgsqlParameter("instCode", instCode), new NpgsqlParameter("email", email))
                .Include(x => x.UcasInstitution)
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
                $"where lower(ou.email) = lower(@email) and Lower(oi.institution_code) = lower(@instCode)",
                new NpgsqlParameter("email", email), new NpgsqlParameter("instCode", instCode)
            ).FirstOrDefault();

            return userOrganisations;
        }

        public void AddUcasCourse(UcasCourse course)
        {
            UcasCourses.Add(course);
        }
        public void Save()
        {
            SaveChanges();
        }
    }
}
