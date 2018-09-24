using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
    /// <summary>
    /// Enrichment data entered by a manage courses user about one of the institutions they look after.
    /// </summary>
    public class InstitutionEnrichmentModel
    {
        public InstitutionEnrichmentModel()
        {
            AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>();
        }

        public string TrainWithUs { get; set; }
        public string TrainWithDisability { get; set; }

        /// <summary>
        /// Enrichment data for all of the accrediting providers for the courses this institution offers.
        /// </summary>
        public IList<AccreditingProviderEnrichment> AccreditingProviderEnrichments { get; set; }

        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Website { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
    }
}
