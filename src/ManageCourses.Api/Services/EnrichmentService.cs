using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class EnrichmentService : IEnrichmentService
    {
        private IManageCoursesDbContext _context;
        private JsonSerializerSettings _jsonSerializerSettings;
        public EnrichmentService(IManageCoursesDbContext context)
        {
            _context = context;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
        public UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string instCode, string email)
        {
            var enrichmentToReturn = new UcasInstitutionEnrichmentGetModel();
            var enrichment = _context.InstitutionEnrichments.SingleOrDefault(ie => instCode.ToLower() == ie.InstCode.ToLower());
            if (enrichment != null)
            {
                var enrichmentModel = enrichment.JsonData != null ? JsonConvert.DeserializeObject<InstitutionEnrichmentModel>(enrichment.JsonData, _jsonSerializerSettings) : null;

                enrichmentToReturn.EnrichmentModel = enrichmentModel;
            }

            return enrichmentToReturn;
        }

        public void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode, string email)
        {
            var mcUser = _context.McUsers.ByEmail(email).Single();

            var institution = mcUser.McOrganisationUsers
                .SelectMany(ou => ou.McOrganisation.McOrganisationInstitutions)
                .Single(i => i.InstitutionCode == instCode).UcasInstitution;//should throw an error if the user doesn't have acces to the inst or the inst doesn't exist

            var content = JsonConvert.SerializeObject(model.EnrichmentModel, _jsonSerializerSettings);

            var enrichment = new InstitutionEnrichment
            {
                InstCode = instCode,
                CreatedTimestampUtc = DateTime.UtcNow,
                UpdateTimestampUtc = DateTime.UtcNow,
                SavedByUserId = mcUser.Id,
                UpdatedByUserId = mcUser.Id,
                JsonData = content,
            };
            _context.InstitutionEnrichments.Add(enrichment);
            _context.Save();
        }
    }
}
