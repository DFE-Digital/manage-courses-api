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
            ValidateUserOrg(email, instCode);

            var enrichmentToReturn = new UcasInstitutionEnrichmentGetModel();
            var enrichment = _context.InstitutionEnrichments.SingleOrDefault(ie => instCode.ToLower() == ie.InstCode.ToLower());
            if (enrichment != null)
            {
                enrichmentToReturn = Convert(enrichment);
            }

            return enrichmentToReturn;
        }
        public void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode, string email)
        {
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
                    Status = EnumStatus.Draft,
                    JsonData = content,
                };
                _context.InstitutionEnrichments.Add(enrichment);
            }
            _context.Save();
        }
        public UcasInstitutionEnrichmentGetModel PublishInstitutionEnrichment(string instCode, string email)
        {
            var userInst = ValidateUserOrg(email, instCode);

            var enrichmentToReturn = new UcasInstitutionEnrichmentGetModel();

            var enrichmentRecord = _context.InstitutionEnrichments.SingleOrDefault(ie => instCode.ToLower() == ie.InstCode.ToLower());

            if (enrichmentRecord != null)
            {
                enrichmentRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentRecord.UpdatedByUser = userInst.McUser;
                enrichmentRecord.LastPublishedTimestampUtc = DateTime.UtcNow;
                enrichmentRecord.Status = EnumStatus.Published;
                _context.Save();
                enrichmentToReturn = Convert(enrichmentRecord);
            }

            return enrichmentToReturn;
        }

        private UserInstitution ValidateUserOrg(string email, string instCode)
        {
            if (string.IsNullOrWhiteSpace(instCode)) { throw new ArgumentException("The 'institution code' must be provided."); }
            if (string.IsNullOrWhiteSpace(email)) { throw new ArgumentException("The 'email' must be provided."); }

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
        private UcasInstitutionEnrichmentGetModel Convert(InstitutionEnrichment source)
        {
            var enrichmentToReturn = new UcasInstitutionEnrichmentGetModel();
            if (source != null)
            {
                var enrichmentModel = source.JsonData != null ? JsonConvert.DeserializeObject<InstitutionEnrichmentModel>(source.JsonData, _jsonSerializerSettings) : null;

                enrichmentToReturn.EnrichmentModel = enrichmentModel;
                enrichmentToReturn.CreatedTimestampUtc = source.CreatedTimestampUtc;
                enrichmentToReturn.UpdatedTimestampUtc = source.UpdatedTimestampUtc;
                enrichmentToReturn.CreatedByUser = source.CreatedByUser;
                enrichmentToReturn.UpdatedByUser = source.UpdatedByUser;
                enrichmentToReturn.LastPublishedTimestampUtc = source.LastPublishedTimestampUtc;
                enrichmentToReturn.Status = source.Status;
            }

            return enrichmentToReturn;
        }
    }
}
