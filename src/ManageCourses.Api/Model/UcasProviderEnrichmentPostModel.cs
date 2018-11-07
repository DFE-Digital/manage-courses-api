namespace GovUk.Education.ManageCourses.Api.Model
{
    public class UcasProviderEnrichmentPostModel
    {
        public UcasProviderEnrichmentPostModel()
        {
            EnrichmentModel = new ProviderEnrichmentModel();
        }
        public ProviderEnrichmentModel EnrichmentModel { get; set; }
    }
}
