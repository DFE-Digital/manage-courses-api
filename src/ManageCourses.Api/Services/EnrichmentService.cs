using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class EnrichmentService : IEnrichmentService
    {
        private IManageCoursesDbContext _context;

        public EnrichmentService(IManageCoursesDbContext context)
        {
            _context = context;
        }
        public UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string instCode)
        {
            throw new NotImplementedException();
        }

        public void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode)
        {
            throw new NotImplementedException();
        }
    }
}
