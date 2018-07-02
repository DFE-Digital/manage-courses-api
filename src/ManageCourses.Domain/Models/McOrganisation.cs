using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class McOrganisation
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<McOrganisationUser> McOrganisationUsers { get; set; }
        public virtual ICollection<McOrganisationInstitution> McOrganisationInstitutions { get; set; }
    }
}
