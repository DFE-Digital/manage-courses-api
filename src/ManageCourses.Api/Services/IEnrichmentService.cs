using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface IEnrichmentService
    {
        UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string email, string instCode);
        void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string email, string instCode);
    }
}
