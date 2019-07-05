using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class EnrichmentService : IEnrichmentService
    {
        private IManageCoursesDbContext _context;
        private EnrichmentConverter _converter;

        public EnrichmentService(IManageCoursesDbContext context)
        {
            _context = context;
            _converter = new EnrichmentConverter();
        }

        public UcasProviderEnrichmentGetModel GetProviderEnrichment(string providerCode, string email)
        {
            return GetProviderEnrichment(providerCode, email, false);
        }
        public UcasProviderEnrichmentGetModel GetProviderEnrichmentForPublish(string providerCode, string email)
        {
            return GetProviderEnrichment(providerCode, email, true);
        }
        public UcasCourseEnrichmentGetModel GetCourseEnrichment(string providerCode, string ucasCourseCode, string email)
        {
            return GetCourseEnrichment(providerCode, ucasCourseCode, email, false);
        }
        public UcasCourseEnrichmentGetModel GetCourseEnrichmentForPublish(string providerCode, string ucasCourseCode, string email)
        {
            return GetCourseEnrichment(providerCode, ucasCourseCode, email, true);
        }

        /// <summary>
        /// This is an upsert.
        /// If a draft record exists then update it.
        /// If a draft record does not exist then add a new one.
        /// If a new draft record is created then:
        ///check for the existence of a previous published record and get the last pulished date
        /// </summary>
        /// <param name="model">holds the enriched data</param>
        /// <param name="providerCode">the provider code for the enrichment data</param>
        /// <param name="email">the email of the user</param>
        public void SaveProviderEnrichment(UcasProviderEnrichmentPostModel model, string providerCode, string email)
        {
            var userProvider = ValidateUserOrg(email, providerCode);

            providerCode = providerCode.ToUpperInvariant();

            var provider = _context.Providers.SingleOrDefault(x => x.ProviderCode == providerCode);
            if (provider == null)
            {
                throw new Exception($"Provider {providerCode} not found");
            }

            var enrichmentDraftRecord = provider.ProviderEnrichments
                .Where(ie => ie.Status == EnumStatus.Draft)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            // when saving enforce the region code is being saved.
            if(!model.EnrichmentModel.RegionCode.HasValue)
            {
                model.EnrichmentModel.RegionCode = provider.RegionCode;
            }

            string content = _converter.ConvertToJson(model);

            if (enrichmentDraftRecord != null)
            {
                //update
                enrichmentDraftRecord.UpdatedAt = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userProvider.User;
                enrichmentDraftRecord.JsonData = content;
            }
            else
            {
                //insert
                var enrichmentPublishRecord = provider.ProviderEnrichments
                    .Where(ie => ie.Status == EnumStatus.Published)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                DateTime? lastPublishedDate = null;
                if (enrichmentPublishRecord != null)
                {
                    lastPublishedDate = enrichmentPublishRecord.LastPublishedAt;
                }
                var enrichment = new ProviderEnrichment
                {
                    ProviderCode = userProvider.UcasProviderCode,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LastPublishedAt = lastPublishedDate,
                    CreatedByUser = userProvider.User,
                    UpdatedByUser = userProvider.User,
                    Status = EnumStatus.Draft,
                    JsonData = content,
                };
                provider.ProviderEnrichments.Add(enrichment);
            }
            _context.Save();
        }

        /// <summary>
        /// Changes the status of the latest draft record to published
        /// </summary>
        /// <param name="providerCode">provider code of the enrichment to be published</param>
        /// <param name="email">email of the user</param>
        /// <returns>true if successful</returns>
        public bool PublishProviderEnrichment(string providerCode, string email)
        {
            var returnBool = false;
            var userOrg = ValidateUserOrg(email, providerCode);

            providerCode = providerCode.ToUpperInvariant();
            var provider = _context.Providers
                .Include(p => p.ProviderEnrichments)
                .Single(p => p.ProviderCode == providerCode);

            var enrichmentDraftRecord = provider.ProviderEnrichments
                .Where(ie => ie.Status == EnumStatus.Draft)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            var publishTimestamp = DateTime.UtcNow;
            provider.LastPublishedAt = publishTimestamp;
            provider.ChangedAt = publishTimestamp; // make sure change shows in incremental fetch api
            if (enrichmentDraftRecord != null)
            {
                enrichmentDraftRecord.UpdatedAt = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userOrg.User;
                enrichmentDraftRecord.LastPublishedAt = publishTimestamp;
                enrichmentDraftRecord.Status = EnumStatus.Published;
                returnBool = true;
            }
            _context.Save();

            return returnBool;
        }

        public IList<UcasCourseEnrichmentGetModel> GetCourseEnrichmentMetadata(string providerCode, string email)
        {
            ValidateUserOrg(email, providerCode);

            providerCode = providerCode.ToUpperInvariant();

            var enrichments = _context.CourseEnrichments.FromSql(@"
SELECT b.id, b.created_by_user_id, b.created_at, b.provider_code, null as json_data, b.last_published_timestamp_utc, b.status, b.ucas_course_code, b.updated_by_user_id, b.updated_at, b.course_id
FROM (SELECT provider_code, ucas_course_code, MAX(id) id FROM course_enrichment GROUP BY provider_code, ucas_course_code) top_id
INNER JOIN course_enrichment b on top_id.id = b.id")
                .Where(e => e.ProviderCode == providerCode);

            return enrichments.Select(x => _converter.Convert(x)).ToList();
        }

        /// <summary>
        /// This is an upsert.
        /// If a draft record exists then update it.
        /// If a draft record does not exist then add a new one.
        /// If a new draft record is created then:
        ///check for the existence of a previous published record and get the last pulished date
        /// </summary>
        /// <param name="model">holds the enriched data</param>
        /// <param name="providerCode">the provider code for the enrichment data</param>
        /// <param name="ucasCourseCode">the course code for the enrichment data</param>
        /// <param name="email">the email of the user</param>
        public void SaveCourseEnrichment(CourseEnrichmentModel model, string providerCode, string ucasCourseCode, string email)
        {
            var userOrg = ValidateUserOrg(email, providerCode);

            providerCode = providerCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.CourseEnrichments
                .Where(ie => ie.ProviderCode == providerCode && ie.UcasCourseCode == ucasCourseCode && ie.Status == EnumStatus.Draft)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            string content = _converter.ConvertToJson(model);

            if (enrichmentDraftRecord != null)
            {
                //update
                enrichmentDraftRecord.UpdatedAt = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userOrg.User;
                enrichmentDraftRecord.JsonData = content;
            }
            else
            {
                //insert
                var enrichmentPublishRecord = _context.CourseEnrichments
                    .Where(ie => ie.ProviderCode == providerCode && ie.UcasCourseCode == ucasCourseCode && ie.Status == EnumStatus.Published)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                DateTime? lastPublishedDate = null;
                if (enrichmentPublishRecord != null)
                {
                    lastPublishedDate = enrichmentPublishRecord.LastPublishedTimestampUtc;
                }
                var enrichment = new CourseEnrichment
                {
                    ProviderCode = userOrg.UcasProviderCode,
                    UcasCourseCode = ucasCourseCode,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LastPublishedTimestampUtc = lastPublishedDate,
                    CreatedByUser = userOrg.User,
                    UpdatedByUser = userOrg.User,
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
        /// <param name="providerCode">provider code of the enrichemnt to be published</param>
        /// <param name="ucasCourseCode">course code of the enrichemnt to be published</param>
        /// <param name="email">email of the user</param>
        /// <returns>true if successful</returns>
        public bool PublishCourseEnrichment(string providerCode, string ucasCourseCode, string email)
        {
            var returnBool = false;
            var userOrg = ValidateUserOrg(email, providerCode);

            providerCode = providerCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.CourseEnrichments
                .Where(ie => ie.ProviderCode == providerCode && ie.UcasCourseCode == ucasCourseCode && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

            var course = _context.Courses.Single(c => c.CourseCode == ucasCourseCode && c.Provider.ProviderCode == providerCode);
            course.ChangedAt = DateTime.UtcNow; // make sure change shows in incremental fetch api

            if (enrichmentDraftRecord != null)
            {
                enrichmentDraftRecord.UpdatedAt = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userOrg.User;
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
        /// <param name="providerCode">provider code for the enrichment</param>
        /// <param name="ucasCourseCode">can be null</param>
        /// <param name="email">email of the user</param>
        /// <param name="publishableOnly">if there is a draft enrichment then get the ucas contact details</param>
        /// <returns>An enrichment object with the enrichment data (if found). Nul if not found</returns>
        private UcasCourseEnrichmentGetModel GetCourseEnrichment(string providerCode, string ucasCourseCode, string email, bool publishableOnly)
        {
            ValidateUserOrg(email, providerCode);
            providerCode = providerCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichmentsQuery = _context.CourseEnrichments
                .Where(ie => ie.ProviderCode == providerCode && ie.UcasCourseCode == ucasCourseCode);

            if (publishableOnly)
            {
                enrichmentsQuery = enrichmentsQuery.Where(ie => ie.Status == EnumStatus.Published);
            }

            var enrichment = enrichmentsQuery.OrderByDescending(x => x.Id)
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .FirstOrDefault();

            var enrichmentGetModel = _converter.Convert(enrichment) ?? new UcasCourseEnrichmentGetModel();

            return enrichmentGetModel;
        }
        /// <summary>
        /// Gets the latest enrichment record if any regardless of the status.
        /// If there is no contact data then it will be pre-populated from the Ucas data.
        /// </summary>
        /// <param name="providerCode">provider code for the enrichment</param>
        /// <param name="email">email of the user</param>
        /// <param name="publishableOnly">if there is a draft enrichment then get the ucas contact details</param>
        /// <returns>
        /// An enrichment object with the enrichment data.
        /// If there is nothing in the db then an empty record with Ucas contact details will be returned.
        /// </returns>
        private UcasProviderEnrichmentGetModel GetProviderEnrichment(string providerCode, string email, bool publishableOnly)
        {
            ValidateUserOrg(email, providerCode);

            providerCode = providerCode.ToUpperInvariant();

            var enrichmentsQuery = _context.ProviderEnrichments
                .Where(ie => ie.ProviderCode == providerCode);

            if (publishableOnly)
            {
                enrichmentsQuery = enrichmentsQuery.Where(ie => ie.Status == EnumStatus.Published);
            }

            var enrichment = enrichmentsQuery.OrderByDescending(x => x.Id)
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .FirstOrDefault();

            var enrichmentGetModel = _converter.Convert(enrichment) ?? new UcasProviderEnrichmentGetModel();

            var enrichmentModel = enrichmentGetModel.EnrichmentModel;

            var useUcasContact =
                string.IsNullOrWhiteSpace(enrichmentModel.Email) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Telephone) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Website) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address1) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address2) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address4) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address3) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Postcode) &&
                !enrichmentModel.RegionCode.HasValue;

            if (useUcasContact)
            {
                var ucasProvider = _context.Providers.SingleOrDefault(x => x.ProviderCode == providerCode);
                if (ucasProvider != null)
                {
                    enrichmentModel.Email = ucasProvider.Email;
                    enrichmentModel.Telephone = ucasProvider.Telephone;
                    enrichmentModel.Website = ucasProvider.Url;
                    enrichmentModel.Address1 = ucasProvider.Address1;
                    enrichmentModel.Address2 = ucasProvider.Address2;
                    enrichmentModel.Address3 = ucasProvider.Address3;
                    enrichmentModel.Address4 = ucasProvider.Address4;
                    enrichmentModel.Postcode = ucasProvider.Postcode;
                    // When first getting enrichment, seed the region code.
                    enrichmentModel.RegionCode = ucasProvider.RegionCode;
                }
            }

            return enrichmentGetModel;
        }
        private UserProvider ValidateUserOrg(string email, string providerCode)
        {
            if (string.IsNullOrWhiteSpace(providerCode)) { throw new ArgumentException("The 'provider code' must be provided."); }
            if (string.IsNullOrWhiteSpace(email)) { throw new ArgumentException("The 'email' must be provided."); }

            providerCode = providerCode.ToUpperInvariant();
            email = email.ToLowerInvariant();

            var provider = _context.OrganisationProviders.Include(x => x.Organisation).Single(x => x.Provider.ProviderCode == providerCode); //should throw an error if  the provider doesn't exist

            var orgUser = _context.OrganisationUsers
                .Where(x => x.User.Email == email && x.Organisation.OrgId == provider.Organisation.OrgId)
                .Include(x => x.User)
                .Include(x => x.Organisation)
                .Single(); //should throw an error if the user doesn't have acces to the provider

            var returnUserProvider = new UserProvider
            {
                User = orgUser.User,
                UcasProviderCode = providerCode
            };

            return returnUserProvider;
        }
    }
}
