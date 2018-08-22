using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    public interface IManageCoursesDbContext
    {
        DbSet<UcasCourse> UcasCourses { get; set; }
        DbSet<CourseCode> CourseCodes { get; set; }
        DbSet<UcasInstitution> UcasInstitutions { get; set; }
        DbSet<UcasCourseSubject> UcasCourseSubjects { get; set; }
        DbSet<UcasSubject> UcasSubjects { get; set; }
        DbSet<UcasCampus> UcasCampuses { get; set; }
        DbSet<UcasCourseNote> UcasCourseNotes { get; set; }
        DbSet<UcasNoteText> UcasNoteTexts { get; set; }
        DbSet<McOrganisation> McOrganisations { get; set; }
        DbSet<McOrganisationInstitution> McOrganisationIntitutions { get; set; }
        DbSet<McOrganisationUser> McOrganisationUsers { get; set; }
        DbSet<McUser> McUsers { get; set; }
        DbSet<AccessRequest> AccessRequests { get; set; }
        DbSet<InstitutionEnrichment> InstitutionEnrichments { get; set; }
        IList<UcasCourse> GetAllUcasCourses();
        IList<UcasInstitution> GetAllUcasInstitutions();
        IList<UcasSubject> GetAllUcasSubjects();
        IList<UcasCourseSubject> GetAllUcasCourseSubjects();
        IList<UcasCampus> GetAllUcasCampuses();
        IList<UcasCourseNote> GetAllUcasCourseNotes();
        IList<UcasNoteText> GetAllUcasNoteTexts();
        IList<McOrganisation> GetAllMcOrganisations();
        IList<McOrganisationInstitution> GetAllMcOrganisationsInstitutions();
        IList<McOrganisationUser> GetAllMcOrganisationsUsers();
        IList<McUser> GetAllMcUsers();
        IList<InstitutionEnrichment> GetAllInstitutionEnrichments();

        void AddUcasCourse(UcasCourse course);
        void AddUcasInstitution(UcasInstitution institution);
        void AddUcasSubject(UcasSubject subject);
        void AddUcasCourseSubject(UcasCourseSubject courseSubject);
        void AddUcasCampus(UcasCampus campus);
        void AddUcasCourseNote(UcasCourseNote courseNote);
        void AddUcasNoteText(UcasNoteText noteText);
        void AddNctlOrganisation(NctlOrganisation nctlOrganisation);
        void AddMcOrganisation(McOrganisation organisation);
        void AddMcOrganisationInstitution(McOrganisationInstitution organisationInstitution);
        void AddMcOrganisationUser(McOrganisationUser organisationUser);
        void AddInstitutionEnrichment(InstitutionEnrichment institutionEnrichment);
        void AddMcUser(McUser user);
        List<UcasCourse> GetUcasCourseRecordsByUcasCode(string instCode, string ucasCode, string email);
        List<UcasCourse> GetUcasCourseRecordsByInstCode(string instCode, string email);
        IQueryable<McOrganisationInstitution> GetUserOrganisations(string email);
        McOrganisationInstitution GetUserOrganisation(string email, string instCode);

        IQueryable<McUser> GetMcUsers(string email);
        void Save();
    }
}
