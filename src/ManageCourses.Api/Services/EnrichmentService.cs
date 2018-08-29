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
        /// <summary>
        /// gets the latest enrichment record regardless of the status
        /// </summary>
        /// <param name="instCode">institution code for the enrichment</param>
        /// <param name="email">email of the user</param>
        /// <returns>An enrichment object with the enrichment data (if found). Nul if not found</returns>
        public UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string instCode, string email)
        {
            ValidateUserOrg(email, instCode);

            var enrichment = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower())
                .OrderByDescending(x => x.Id)
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .FirstOrDefault();
            var enrichmentToReturn = Convert(enrichment);

            return enrichmentToReturn;
        }
        /// <summary>
        /// This is an upsert.
        /// If a draft record exists then update it.
        /// If a draft record does not exist then add a new one.
        /// If a new draft record is created then:
        ///check for the existence of a previous published record and get the last pulished date
        /// </summary>
        /// <param name="model">holds the enriched data</param>
        /// <param name="instCode">the institution code for the enrichment data</param>
        /// <param name="email">the email of the user</param>
        public void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode, string email)
        {
            var userInst = ValidateUserOrg(email, instCode);

            var enrichmentDraftRecord = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

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
                var enrichmentPublishRecord = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ie.Status == EnumStatus.Published).OrderByDescending(x => x.Id).FirstOrDefault();
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
        /// <summary>
        /// Changes the status of the latest draft record to published
        /// </summary>
        /// <param name="instCode">institution code of the enrichemtn to be published</param>
        /// <param name="email">email of the user</param>
        /// <returns>true if successful</returns>
        public bool PublishInstitutionEnrichment(string instCode, string email)
        {
            var returnBool = false;
            var userInst = ValidateUserOrg(email, instCode);

            var enrichmentDraftRecord = _context.InstitutionEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

            if (enrichmentDraftRecord != null)
            {
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.McUser;
                enrichmentDraftRecord.LastPublishedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.Status = EnumStatus.Published;
                _context.Save();
                returnBool = true;
            }

            return returnBool;
        }

        /// <summary>
        /// gets the latest enrichment record regardless of the status
        /// </summary>
        /// <param name="instCode">institution code for the enrichment</param>
        /// <param name="ucasCourseCode"></param>
        /// <param name="email">email of the user</param>
        /// <returns>An enrichment object with the enrichment data (if found). Nul if not found</returns>
        public UcasCourseEnrichmentGetModel GetCourseEnrichment(string instCode, string ucasCourseCode, string email)
        {
            ValidateUserOrg(email, instCode);

            // todo: make this EF friendly
            var enrichment = _context.CourseEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ucasCourseCode.ToLower() == ie.UcasCourseCode.ToLower())
                .OrderByDescending(x => x.Id)
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .FirstOrDefault();
            var enrichmentToReturn = Convert(enrichment);

            return enrichmentToReturn;
        }

        /// <summary>
        /// This is an upsert.
        /// If a draft record exists then update it.
        /// If a draft record does not exist then add a new one.
        /// If a new draft record is created then:
        ///check for the existence of a previous published record and get the last pulished date
        /// </summary>
        /// <param name="model">holds the enriched data</param>
        /// <param name="instCode">the institution code for the enrichment data</param>
        /// <param name="email">the email of the user</param>
        public void SaveCourseEnrichment(CourseEnrichmentModel model, string instCode, string ucasCourseCode, string email)
        {
            var userInst = ValidateUserOrg(email, instCode);

            var enrichmentDraftRecord = _context.CourseEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ucasCourseCode.ToLower() == ie.UcasCourseCode.ToLower() && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

            var content = JsonConvert.SerializeObject(model, _jsonSerializerSettings);

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
                // todo: make this EF friendly
                var enrichmentPublishRecord = _context.CourseEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ucasCourseCode.ToLower() == ie.UcasCourseCode.ToLower() && ie.Status == EnumStatus.Published).OrderByDescending(x => x.Id).FirstOrDefault();
                DateTime? lastPublishedDate = null;
                if (enrichmentPublishRecord != null)
                {
                    lastPublishedDate = enrichmentPublishRecord.LastPublishedTimestampUtc;
                }
                var enrichment = new CourseEnrichment
                {
                    InstCode = userInst.UcasInstitution.InstCode,
                    UcasCourseCode = ucasCourseCode,
                    CreatedTimestampUtc = DateTime.UtcNow,
                    UpdatedTimestampUtc = DateTime.UtcNow,
                    LastPublishedTimestampUtc = lastPublishedDate,
                    CreatedByUser = userInst.McUser,
                    UpdatedByUser = userInst.McUser,
                    Status = EnumStatus.Draft,
                    JsonData = content,
                };
                _context.CourseEnrichments.Add(enrichment);
            }
            _context.Save();
        }

        /// <summary>
        /// Changes the status of the latest draft record to published
        /// </summary>
        /// <param name="instCode">institution code of the enrichemtn to be published</param>
        /// <param name="email">email of the user</param>
        /// <returns>true if successful</returns>
        public bool PublishCourseEnrichment(string instCode, string ucasCourseCode, string email)
        {
            var returnBool = false;
            var userInst = ValidateUserOrg(email, instCode);

            // todo: make this EF friendly
            var enrichmentDraftRecord = _context.CourseEnrichments.Where(ie => instCode.ToLower() == ie.InstCode.ToLower() && ucasCourseCode.ToLower() == ie.UcasCourseCode.ToLower() && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

            if (enrichmentDraftRecord != null)
            {
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.McUser;
                enrichmentDraftRecord.LastPublishedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.Status = EnumStatus.Published;
                _context.Save();
                returnBool = true;
            }

            return returnBool;
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

        /// <summary>
        /// maps enrichment data from the data object to the returned enrichment model
        /// </summary>
        /// <param name="source">enrichment data object</param>
        /// <returns>enrichment model with enrichment data (if found). Null if there is no source record</returns>
        private UcasCourseEnrichmentGetModel Convert(CourseEnrichment source)
        {
            if (source == null) return null;

            var enrichmentToReturn = new UcasCourseEnrichmentGetModel();
            var enrichmentModel = source.JsonData != null ? JsonConvert.DeserializeObject<CourseEnrichmentModel>(source.JsonData, _jsonSerializerSettings) : null;

            enrichmentToReturn.EnrichmentModel = enrichmentModel;
            enrichmentToReturn.CreatedTimestampUtc = source.CreatedTimestampUtc;
            enrichmentToReturn.UpdatedTimestampUtc = source.UpdatedTimestampUtc;
            enrichmentToReturn.CreatedByUserId = source.CreatedByUser.Id;
            enrichmentToReturn.UpdatedByUserId = source.UpdatedByUser.Id;
            enrichmentToReturn.LastPublishedTimestampUtc = source.LastPublishedTimestampUtc;
            enrichmentToReturn.Status = source.Status;
            return enrichmentToReturn;
        }

        /// <summary>
        /// maps enrichment data from the data object to the returned enrichment model
        /// </summary>
        /// <param name="source">enrichment data object</param>
        /// <returns>enrichment model with enrichment data (if found). Null if there is no source record</returns>
        private UcasInstitutionEnrichmentGetModel Convert(InstitutionEnrichment source)
        {
            if (source == null) return null;

            var enrichmentToReturn = new UcasInstitutionEnrichmentGetModel();
            var enrichmentModel = source.JsonData != null ? JsonConvert.DeserializeObject<InstitutionEnrichmentModel>(source.JsonData, _jsonSerializerSettings) : null;

            enrichmentToReturn.EnrichmentModel = enrichmentModel;
            enrichmentToReturn.CreatedTimestampUtc = source.CreatedTimestampUtc;
            enrichmentToReturn.UpdatedTimestampUtc = source.UpdatedTimestampUtc;
            enrichmentToReturn.CreatedByUserId = source.CreatedByUser.Id;
            enrichmentToReturn.UpdatedByUserId = source.UpdatedByUser.Id;
            enrichmentToReturn.LastPublishedTimestampUtc = source.LastPublishedTimestampUtc;
            enrichmentToReturn.Status = source.Status;
            return enrichmentToReturn;
        }
    }
}
