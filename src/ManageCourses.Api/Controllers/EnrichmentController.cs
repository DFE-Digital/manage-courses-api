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
        [Route("institution/{ucasInstitutionCode}")]
        public UcasInstitutionEnrichmentGetModel GetInstitution(string ucasInstitutionCode)
        {
            return new UcasInstitutionEnrichmentGetModel();
        }

        [HttpPost]
        [Route("institution/{ucasInstitutionCode}")]
        public void SaveInstitution(string ucasInstitutionCode, UcasInstitutionEnrichmentPostModel model)
        {
        }
    }
}
