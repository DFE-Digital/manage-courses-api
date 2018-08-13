using GovUk.Education.ManageCourses.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/enrichment")]
    public class EnrichmentController : Controller
    {
        [HttpGet]
        [Route("institution/{institutionCode}")]
        public UcasInstitutionEnrichment GetInstitution(string institutionCode)
        {
            return new UcasInstitutionEnrichment();
        }

        [HttpPost]
        [Route("institution/{institutionCode}")]
        public void SaveInstitution(string institutionCode, UcasInstitutionEnrichment model)
        {
        }
    }
}
