using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class Organisation
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string Name { get; set; }

        public ICollection<OrganisationUser> OrganisationUsers { get; set; }
        public ICollection<OrganisationProvider> OrganisationProviders { get; set; }

        public ICollection<NctlOrganisation> NctlOrganisations { get; set; }
    }
}
