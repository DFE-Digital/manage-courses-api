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
            var enrichment = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower()).OrderByDescending(x => x.Id).FirstOrDefault();
            if (enrichment != null)
            {
                enrichmentToReturn = Convert(enrichment);
            }

            return enrichmentToReturn;
        }
        public void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode, string email)
        {
            var userInst = ValidateUserOrg(email, instCode);

            var enrichmentDraftRecord = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();
            var enrichmentPublishRecord = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ie.Status == EnumStatus.Published).OrderByDescending(x => x.Id).FirstOrDefault();
            var content = JsonConvert.SerializeObject(model.EnrichmentModel, _jsonSerializerSettings);

            if (enrichmentDraftRecord != null)
            {
                //update
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.McUser;
                enrichmentDraftRecord.JsonData = content;                
            }
            else
            {
                //insert
                DateTime? lastPublishedDate = null;
                if (enrichmentPublishRecord != null)
                {
                    lastPublishedDate = enrichmentPublishRecord.LastPublishedTimestampUtc;
                }
                var enrichment = new InstitutionEnrichment
                {
                    InstCode = userInst.UcasInstitution.InstCode,
                    CreatedTimestampUtc = DateTime.UtcNow,
                    UpdatedTimestampUtc = DateTime.UtcNow,
                    LastPublishedTimestampUtc = lastPublishedDate,
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

            var enrichmentDraftRecord = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

            if (enrichmentDraftRecord != null)
            {
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.McUser;
                enrichmentDraftRecord.LastPublishedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.Status = EnumStatus.Published;
                _context.Save();
                enrichmentToReturn = Convert(enrichmentDraftRecord);
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
                enrichmentToReturn.CreatedByUserId = source.CreatedByUser.Id;
                enrichmentToReturn.UpdatedByUserId = source.UpdatedByUser.Id;
                enrichmentToReturn.LastPublishedTimestampUtc = source.LastPublishedTimestampUtc;
                enrichmentToReturn.Status = source.Status;
            }

            return enrichmentToReturn;
        }
    }
}
