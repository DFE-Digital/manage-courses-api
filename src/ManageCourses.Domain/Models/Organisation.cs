using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class Organisation
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string Name { get; set; }

        public ICollection<OrganisationUser> OrganisationUsers { get; set; }
        public ICollection<OrganisationProvider> OrganisationInstitutions { get; set; }

        public ICollection<NctlOrganisation> NctlOrganisations { get; set; }
    }
}
