using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        [ApiTokenAuth]
        [ExemptFromAcceptTerms]
        [HttpPost]
        [Route("ucas")]
        public void Import([FromBody] UcasPayload payload)
        {
            _dataService.ProcessUcasPayload(payload);
        }
    }
}

