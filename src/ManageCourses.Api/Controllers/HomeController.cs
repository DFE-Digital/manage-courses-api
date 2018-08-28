
using GovUk.Education.ManageCourses.Api.ActionFilters;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    ///<summary>
    /// Simple root endpoint.
    ///</summary>
    public class HomeController : Controller
    {
        ///<summary>
        /// takes you to swagger.
        ///</summary>
        [ExemptFromAcceptTerms]
        [Route("/")]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}