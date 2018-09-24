using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Mapping;
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
        private EnrichmentConverter _converter;

        public EnrichmentService(IManageCoursesDbContext context)
        {
            _context = context;
            _converter = new EnrichmentConverter();
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

            instCode = instCode.ToUpperInvariant(); 

            var enrichment = _context.InstitutionEnrichments
                .Where(ie => ie.InstCode == instCode)
                .OrderByDescending(x => x.Id)
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .FirstOrDefault();

            var enrichmentToReturn = _converter.Convert(enrichment);
            if (enrichmentToReturn == null)
            {
                return null;
            }
            
            var useUcasContact = 
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Email) &&
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Telephone) &&
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Website) &&
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Address1) &&
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Address2) &&
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Address4) &&
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Address3) &&
                string.IsNullOrWhiteSpace(enrichmentToReturn.EnrichmentModel.Postcode);

            if (useUcasContact)
            {
                var ucasInst = _context.UcasInstitutions.SingleOrDefault(x => x.InstCode == instCode);
                if (ucasInst != null)
                {
                    enrichmentToReturn.EnrichmentModel.Email = ucasInst.Email;
                    enrichmentToReturn.EnrichmentModel.Telephone = ucasInst.Telephone;
                    enrichmentToReturn.EnrichmentModel.Website = ucasInst.Url;
                    enrichmentToReturn.EnrichmentModel.Address1 = ucasInst.Addr1;
                    enrichmentToReturn.EnrichmentModel.Address2 = ucasInst.Addr2;
                    enrichmentToReturn.EnrichmentModel.Address3 = ucasInst.Addr3;
                    enrichmentToReturn.EnrichmentModel.Address4 = ucasInst.Addr4;
                    enrichmentToReturn.EnrichmentModel.Postcode = ucasInst.Postcode;
                }
            }

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
                enrichmentDraftRecord.UpdatedByUser = userInst.McUser;
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

            instCode = instCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.InstitutionEnrichments
                .Where(ie => ie.InstCode == instCode && ie.Status == EnumStatus.Draft)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

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
        /// <param name="ucasCourseCode">can be null</param>
        /// <param name="email">email of the user</param>
        /// <returns>An enrichment object with the enrichment data (if found). Nul if not found</returns>
        public UcasCourseEnrichmentGetModel GetCourseEnrichment(string instCode, string ucasCourseCode, string email)
        {
            ValidateUserOrg(email, instCode);

            instCode = instCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichment = _context.CourseEnrichments
                .Where(ie => ie.InstCode == instCode && ie.UcasCourseCode == ucasCourseCode)
                .OrderByDescending(x => x.Id)
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .FirstOrDefault();

            var enrichmentToReturn = _converter.Convert(enrichment);

            return enrichmentToReturn;
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
                enrichmentDraftRecord.UpdatedByUser = userInst.McUser;
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

            instCode = instCode.ToUpperInvariant();
            ucasCourseCode = ucasCourseCode.ToUpperInvariant();

            var enrichmentDraftRecord = _context.CourseEnrichments
                .Where(ie => ie.InstCode == instCode && ie.UcasCourseCode == ucasCourseCode && ie.Status == EnumStatus.Draft).OrderByDescending(x => x.Id).FirstOrDefault();

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

            instCode = instCode.ToUpperInvariant();
            email = email.ToLowerInvariant();

            var inst = _context.McOrganisationIntitutions.Single(x => x.InstitutionCode == instCode); //should throw an error if  the inst doesn't exist
            
            var orgUser = _context.McOrganisationUsers
                .Where(x => x.Email == email && x.OrgId == inst.OrgId)
                .Include(x=> x.McUser)
                .Single(); //should throw an error if the user doesn't have acces to the inst

            var returnUserInst = new UserInstitution
            {
                McUser = orgUser.McUser,
                UcasInstitutionCode = instCode
            };

            return returnUserInst;
        }
    }
}
