using System;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class UcasInstitutionEnrichmentGetModel
    {
        public InstitutionEnrichmentModel EnrichmentModel { get; set; }

        public DateTime CreatedTimestampUtc { get; set; }
        public DateTime ModifiedTimestampUtc { get; set; }
    }
}
