namespace GovUk.Education.ManageCourses.Api.Model
{
    public class UcasInstitutionEnrichmentPostModel
    {
        public UcasInstitutionEnrichmentPostModel()
        {
            EnrichmentModel = new InstitutionEnrichmentModel();
        }
        public InstitutionEnrichmentModel EnrichmentModel { get; set; }
    }
}
