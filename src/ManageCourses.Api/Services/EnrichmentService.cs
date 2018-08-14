using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class EnrichmentService : IEnrichmentService
    {
        private IManageCoursesDbContext _context;

        public EnrichmentService(IManageCoursesDbContext context)
        {
            _context = context;
        }
        public UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string instCode, string email)
        {
            return new UcasInstitutionEnrichmentGetModel();
        }

        public void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode, string email)
        {
            var mcUser = _context.McUsers.ByEmail(email).Single();
            var institution = mcUser.McOrganisationUsers
                .SelectMany(ou => ou.McOrganisation.McOrganisationInstitutions)
                .Single(i => i.InstitutionCode == instCode).UcasInstitution;

            institution.InstitutionEnrichments.Add(new InstitutionEnrichment
            {
                CreatedTimestampUtc = DateTime.UtcNow,
                UpdateTimestampUtc = DateTime.UtcNow,
                SavedByUserId = mcUser.Id,
                UpdatedByUserId = mcUser.Id
            });
            _context.Save();
        }
    }
}
