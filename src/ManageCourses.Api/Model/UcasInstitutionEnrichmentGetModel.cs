using System;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class UcasInstitutionEnrichmentGetModel
    {
        public UcasInstitutionEnrichmentGetModel()
        {
            EnrichmentModel = new InstitutionEnrichmentModel();
        }
        public InstitutionEnrichmentModel EnrichmentModel { get; set; }
        public DateTime? CreatedTimestampUtc { get; set; }
        public DateTime? UpdatedTimestampUtc { get; set; }
        public EnumStatus Status { get; set; }
    }
}
