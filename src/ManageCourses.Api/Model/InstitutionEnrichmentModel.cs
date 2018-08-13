using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
    /// <summary>
    /// Enrichment data entered by a manage courses user about one of the institutions they look after.
    /// </summary>
    public class InstitutionEnrichmentModel
    {
        public string TrainWithUs { get; set; }
        public string TrainWithDisability { get; set; }

        /// <summary>
        /// Enrichment data for all of the accrediting providers for the courses this institution offers.
        /// </summary>
        public IList<AccreditingProviderEnrichment> AccreditingProviderEnrichments { get; set; }
    }
}
