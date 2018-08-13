namespace GovUk.Education.ManageCourses.Api.Model
{
    /// <summary>
    /// Institutions have courses that have accrediting_provider entries that are FKs to ucas_institution's
    /// These can be enriched. This is the storage of that enrichment.
    /// </summary>
    public class AccreditingProviderEnrichment
    {
        /// <summary>
        /// Foreign key to the ucas institution that is this accrediting provider.
        /// </summary>
        public string UcasInstitutionCode { get; set; }

        /// <summary>
        /// Enrichment data entered by a manage courses user, explaining more about this accrediting provider
        /// </summary>
        public string Description { get; set; }
    }
}
