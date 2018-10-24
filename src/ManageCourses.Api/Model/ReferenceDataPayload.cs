using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class ReferenceDataPayload
    {
        public ReferenceDataPayload()
        {
            this.Organisations = new List<Organisation>();
            this.NctlOrganisation = new List<NctlOrganisation>();
            this.Institutions = new List<Institution>();
            this.OrganisationInstitutions = new List<OrganisationInstitution>();
            this.OrganisationUsers = new List<OrganisationUser>();
            this.Users = new List<User>();
        }
        public IEnumerable<Organisation> Organisations { get; set; }
        public IEnumerable<NctlOrganisation> NctlOrganisation { get; set; }
        public IEnumerable<Institution> Institutions { get; set; }
        public IEnumerable<OrganisationInstitution> OrganisationInstitutions { get; set; }
        public IEnumerable<OrganisationUser> OrganisationUsers { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
