using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class ReferenceDataPayload
    {
        public ReferenceDataPayload()
        {
            this.Organisations = new List<McOrganisation>();
            this.Institutions = new List<UcasInstitution>();
            this.OrganisationInstitutions = new List<McOrganisationInstitution>();
            this.OrganisationUsers = new List<McOrganisationUser>();
            this.Users = new List<McUser>();
        }
        public IEnumerable<McOrganisation> Organisations { get; set; }
        public IEnumerable<UcasInstitution> Institutions { get; set; }
        public IEnumerable<McOrganisationInstitution> OrganisationInstitutions { get; set; }
        public IEnumerable<McOrganisationUser> OrganisationUsers { get; set; }
        public IEnumerable<McUser> Users { get; set; }
    }
}
