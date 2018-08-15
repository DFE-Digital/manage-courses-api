using System;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
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
            if (string.IsNullOrWhiteSpace(instCode)) { throw new ArgumentException("The 'institution code' must be provided.");}
            if (string.IsNullOrWhiteSpace(email)) { throw new ArgumentException("The 'email' must be provided."); }

            ValidateUserOrg(email, instCode);

            var enrichmentToReturn = new UcasInstitutionEnrichmentGetModel();
            var enrichment = _context.InstitutionEnrichments.SingleOrDefault(ie => instCode.ToLower() == ie.InstCode.ToLower());
            if (enrichment != null)
            {
                var enrichmentModel = enrichment.JsonData != null ? JsonConvert.DeserializeObject<InstitutionEnrichmentModel>(enrichment.JsonData, _jsonSerializerSettings) : null;

                enrichmentToReturn.EnrichmentModel = enrichmentModel;
                enrichmentToReturn.CreatedTimestampUtc = enrichment.CreatedTimestampUtc;
                enrichmentToReturn.UpdatedTimestampUtc = enrichment.UpdatedTimestampUtc;
            }

            return enrichmentToReturn;
        }

        private UserInstitution ValidateUserOrg(string email, string instCode)
        {
            var user = _context.McUsers.ByEmail(email)
                .Include(x => x.McOrganisationUsers)
                .ThenInclude(x => x.McOrganisation)
                .ThenInclude(x => x.McOrganisationInstitutions)
                .ThenInclude(x => x.UcasInstitution)
                .Single();

            var institution = user.McOrganisationUsers
                .SelectMany(ou => ou.McOrganisation.McOrganisationInstitutions)
                .Single(i => instCode.ToLower() == i.InstitutionCode.ToLower()).UcasInstitution;//should throw an error if the user doesn't have acces to the inst or the inst doesn't exist

            var returnUserInst = new UserInstitution
            {
                McUser = user,
                UcasInstitution = institution
            };

            return returnUserInst;
        }
        public void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode, string email)
        {
            if (string.IsNullOrWhiteSpace(instCode)) { throw new ArgumentException("The 'institution code' must be provided."); }
            if (string.IsNullOrWhiteSpace(email)) { throw new ArgumentException("The 'email' must be provided."); }

            var userInst = ValidateUserOrg(email, instCode);

            var enrichmentRecord = _context.InstitutionEnrichments.SingleOrDefault(ie => instCode.ToLower() == ie.InstCode.ToLower());
            var content = JsonConvert.SerializeObject(model.EnrichmentModel, _jsonSerializerSettings);

            if (enrichmentRecord != null)
            {
                //update
                enrichmentRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentRecord.UpdatedByUser = userInst.McUser;
                enrichmentRecord.JsonData = content;                
            }
            else
            {
                //insert
                var enrichment = new InstitutionEnrichment
                {
                    InstCode = userInst.UcasInstitution.InstCode,
                    CreatedTimestampUtc = DateTime.UtcNow,
                    UpdatedTimestampUtc = DateTime.UtcNow,
                    CreatedByUser = userInst.McUser,
                    UpdatedByUser = userInst.McUser,
                    JsonData = content,
                    //UcasInstitution = institution
                };
                _context.InstitutionEnrichments.Add(enrichment);
            }
            _context.Save();
        }
    }
}
