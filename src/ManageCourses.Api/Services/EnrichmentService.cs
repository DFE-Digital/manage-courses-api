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

        public UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string instCode, string email)
        {
            return GetInstitutionEnrichment(instCode, email, false);
        }
        public UcasInstitutionEnrichmentGetModel GetInstitutionEnrichmentForPublish(string instCode, string email)
        {
            return GetInstitutionEnrichment(instCode, email, true);
        }
        public UcasCourseEnrichmentGetModel GetCourseEnrichment(string instCode, string ucasCourseCode, string email)
        {
            return GetCourseEnrichment(instCode, ucasCourseCode, email, false);
        }
        public UcasCourseEnrichmentGetModel GetCourseEnrichmentForPublish(string instCode, string ucasCourseCode, string email)
        {
            return GetCourseEnrichment(instCode, ucasCourseCode, email, true);
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

            instCode = instCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.InstitutionEnrichments
                .Where(ie => ie.InstCode == instCode && ie.Status == EnumStatus.Draft)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            string content = _converter.ConvertToJson(model);

            if (enrichmentDraftRecord != null)
            {
                //update
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.User;
                enrichmentDraftRecord.JsonData = content;
            }
            else
            {
                //insert
                var enrichmentPublishRecord = _context.InstitutionEnrichments
                    .Where(ie => ie.InstCode == instCode && ie.Status == EnumStatus.Published)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                DateTime? lastPublishedDate = null;
                if (enrichmentPublishRecord != null)
                {
                    lastPublishedDate = enrichmentPublishRecord.LastPublishedTimestampUtc;
                }
                var enrichment = new InstitutionEnrichment
                {
                    InstCode = userInst.UcasInstitutionCode,
                    CreatedTimestampUtc = DateTime.UtcNow,
                    UpdatedTimestampUtc = DateTime.UtcNow,
                    LastPublishedTimestampUtc = lastPublishedDate,
                    CreatedByUser = userInst.User,
                    UpdatedByUser = userInst.User,
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

            instCode = instCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.InstitutionEnrichments
                .Where(ie => ie.InstCode == instCode && ie.Status == EnumStatus.Draft)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (enrichmentDraftRecord != null)
            {
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.User;
                enrichmentDraftRecord.LastPublishedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.Status = EnumStatus.Published;
                _context.Save();
                returnBool = true;
            }

            return returnBool;
        }

        public IList<UcasCourseEnrichmentGetModel> GetCourseEnrichmentMetadata(string instCode, string email)
        {
            ValidateUserOrg(email, instCode);

            instCode = instCode.ToUpperInvariant();

            var enrichments = _context.CourseEnrichments.FromSql(@"
SELECT b.id, b.created_by_user_id, b.created_timestamp_utc, b.inst_code, null as json_data, b.last_published_timestamp_utc, b.status, b.ucas_course_code, b.updated_by_user_id, b.updated_timestamp_utc
FROM (SELECT inst_code, ucas_course_code, MAX(id) id FROM course_enrichment GROUP BY inst_code, ucas_course_code) top_id
INNER JOIN course_enrichment b on top_id.id = b.id")
                .Where(e => e.InstCode == instCode);

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
        /// <param name="instCode">the institution code for the enrichment data</param>
        /// <param name="ucasCourseCode">the course code for the enrichment data</param>
        /// <param name="email">the email of the user</param>
        public void SaveCourseEnrichment(CourseEnrichmentModel model, string instCode, string ucasCourseCode, string email)
        {
            var userInst = ValidateUserOrg(email, instCode);

            instCode = instCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.CourseEnrichments
                .Where(ie => ie.InstCode == instCode && ie.UcasCourseCode == ucasCourseCode && ie.Status == EnumStatus.Draft)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            string content = _converter.ConvertToJson(model);

            if (enrichmentDraftRecord != null)
            {
                //update
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.User;
                enrichmentDraftRecord.JsonData = content;
            }
            else
            {
                //insert
                var enrichmentPublishRecord = _context.CourseEnrichments
                    .Where(ie => ie.InstCode == instCode && ie.UcasCourseCode == ucasCourseCode && ie.Status == EnumStatus.Published)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                DateTime? lastPublishedDate = null;
                if (enrichmentPublishRecord != null)
                {
                    lastPublishedDate = enrichmentPublishRecord.LastPublishedTimestampUtc;
                }
                var enrichment = new CourseEnrichment
                {
                    InstCode = userInst.UcasInstitutionCode,
                    UcasCourseCode = ucasCourseCode,
                    CreatedTimestampUtc = DateTime.UtcNow,
                    UpdatedTimestampUtc = DateTime.UtcNow,
                    LastPublishedTimestampUtc = lastPublishedDate,
                    CreatedByUser = userInst.User,
                    UpdatedByUser = userInst.User,
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
        /// <param name="instCode">institution code of the enrichemnt to be published</param>
        /// <param name="ucasCourseCode">course code of the enrichemnt to be published</param>
        /// <param name="email">email of the user</param>
        /// <returns>true if successful</returns>
        public bool PublishCourseEnrichment(string instCode, string ucasCourseCode, string email)
        {
            var returnBool = false;
            var userInst = ValidateUserOrg(email, instCode);

            instCode = instCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.CourseEnrichments
                .Where(ie => ie.InstCode == instCode && ie.UcasCourseCode == ucasCourseCode && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

            if (enrichmentDraftRecord != null)
            {
                enrichmentDraftRecord.UpdatedTimestampUtc = DateTime.UtcNow;
                enrichmentDraftRecord.UpdatedByUser = userInst.User;
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
        /// <param name="ucasCourseCode">can be null</param>
        /// <param name="email">email of the user</param>
        /// <param name="publishableOnly">if there is a draft enrichment then get the ucas contact details</param>
        /// <returns>An enrichment object with the enrichment data (if found). Nul if not found</returns>
        private UcasCourseEnrichmentGetModel GetCourseEnrichment(string instCode, string ucasCourseCode, string email, bool publishableOnly)
        {
            ValidateUserOrg(email, instCode);
            instCode = instCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichmentsQuery = _context.CourseEnrichments
                .Where(ie => ie.InstCode == instCode && ie.UcasCourseCode == ucasCourseCode);

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
        /// <param name="instCode">institution code for the enrichment</param>
        /// <param name="email">email of the user</param>
        /// <param name="publishableOnly">if there is a draft enrichment then get the ucas contact details</param>
        /// <returns>
        /// An enrichment object with the enrichment data.
        /// If there is nothing in the db then an empty record with Ucas contact details will be returned.
        /// </returns>
        private UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string instCode, string email, bool publishableOnly)
        {
            ValidateUserOrg(email, instCode);

            instCode = instCode.ToUpperInvariant();

            var enrichmentsQuery = _context.InstitutionEnrichments
                .Where(ie => ie.InstCode == instCode);

            if (publishableOnly)
            {
                enrichmentsQuery = enrichmentsQuery.Where(ie => ie.Status == EnumStatus.Published);
            }

            var enrichment = enrichmentsQuery.OrderByDescending(x => x.Id)
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .FirstOrDefault();

            var enrichmentGetModel = _converter.Convert(enrichment) ?? new UcasInstitutionEnrichmentGetModel();

            var enrichmentModel = enrichmentGetModel.EnrichmentModel;

            var useUcasContact =
                string.IsNullOrWhiteSpace(enrichmentModel.Email) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Telephone) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Website) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address1) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address2) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address4) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address3) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Postcode);

            if (useUcasContact)
            {
                var ucasInst = _context.Institutions.SingleOrDefault(x => x.ProviderCode == instCode);
                if (ucasInst != null)
                {
                    enrichmentModel.Email = ucasInst.Email;
                    enrichmentModel.Telephone = ucasInst.Telephone;
                    enrichmentModel.Website = ucasInst.Url;
                    enrichmentModel.Address1 = ucasInst.Address1;
                    enrichmentModel.Address2 = ucasInst.Address2;
                    enrichmentModel.Address3 = ucasInst.Address3;
                    enrichmentModel.Address4 = ucasInst.Address4;
                    enrichmentModel.Postcode = ucasInst.Postcode;
                }
            }

            return enrichmentGetModel;
        }
        private UserInstitution ValidateUserOrg(string email, string instCode)
        {
            if (string.IsNullOrWhiteSpace(instCode)) { throw new ArgumentException("The 'institution code' must be provided."); }
            if (string.IsNullOrWhiteSpace(email)) { throw new ArgumentException("The 'email' must be provided."); }

            instCode = instCode.ToUpperInvariant();
            email = email.ToLowerInvariant();

            var inst = _context.OrganisationIntitutions.Include(x => x.Organisation).Single(x => x.Provider.ProviderCode == instCode); //should throw an error if  the inst doesn't exist

            var orgUser = _context.OrganisationUsers
                .Where(x => x.User.Email == email && x.Organisation.OrgId == inst.Organisation.OrgId)
                .Include(x => x.User)
                .Include(x => x.Organisation)
                .Single(); //should throw an error if the user doesn't have acces to the inst

            var returnUserInst = new UserInstitution
            {
                User = orgUser.User,
                UcasInstitutionCode = instCode
            };

            return returnUserInst;
        }
    }
}
