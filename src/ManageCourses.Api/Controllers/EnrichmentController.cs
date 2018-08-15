using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Authorize]
    [Route("api/enrichment")]
    public class EnrichmentController : Controller
    {
        private IEnrichmentService _service;
        public EnrichmentController(IEnrichmentService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("institution/{ucasInstitutionCode}")]
        public UcasInstitutionEnrichmentGetModel GetInstitution(string ucasInstitutionCode)
        {
            return _service.GetInstitutionEnrichment(ucasInstitutionCode, User.Identity.Name);
        }

        [HttpPost]
        [Route("institution/{ucasInstitutionCode}")]
        public void SaveInstitution(string ucasInstitutionCode, [FromBody]UcasInstitutionEnrichmentPostModel model)
        {
            _service.SaveInstitutionEnrichment(model, ucasInstitutionCode, User.Identity.Name);
        }
    }
}
