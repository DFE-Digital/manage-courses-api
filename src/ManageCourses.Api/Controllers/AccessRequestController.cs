using System;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    
    
    [Route("api/[controller]")]
    public class AccessRequestController : Controller
    {
        private readonly IAccessRequestService _service;

        public AccessRequestController(IAccessRequestService accessRequestService)
        {
            _service = accessRequestService;
        }

        [Authorize]
        [HttpPost]
        public StatusCodeResult Index([FromBody] AccessRequest request) 
        {
            var requesterEmail = this.User.Identity.Name;
            _service.LogAccessRequest(request, requesterEmail);        
            return StatusCode(200);
            
        }
    }
}