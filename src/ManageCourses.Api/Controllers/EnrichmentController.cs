using GovUk.Education.ManageCourses.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Authorize]
    [Route("api/enrichment")]
    public class EnrichmentController : Controller
    {
        [HttpGet]
        [Route("institution/{institutionCode}")]
        public UcasInstitutionEnrichmentGetModel GetInstitution(string institutionCode)
        {
            return new UcasInstitutionEnrichmentGetModel();
        }

        [HttpPost]
        [Route("institution/{institutionCode}")]
        public void SaveInstitution(string institutionCode, UcasInstitutionEnrichmentPostModel model)
        {
        }
    }
}
