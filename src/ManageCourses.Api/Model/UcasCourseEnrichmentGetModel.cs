using System;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class UcasCourseEnrichmentGetModel
    {
        public UcasCourseEnrichmentGetModel()
        {
            EnrichmentModel = new CourseEnrichmentModel();
        }
        public CourseEnrichmentModel EnrichmentModel { get; set; }
        public DateTime? CreatedTimestampUtc { get; set; }
        public DateTime? UpdatedTimestampUtc { get; set; }
        public DateTime? LastPublishedTimestampUtc { get; set; }
        public int CreatedByUserId { get; set; }//TODO should be a user object however it causes an error 
        public int UpdatedByUserId { get; set; }//TODO should be a user object however it causes an error 
        public EnumStatus Status { get; set; }
        public string ProviderCode { get; set; }
        public string CourseCode { get; set; }
    }
}
