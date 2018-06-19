using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class McOrganisation
    {
        public int Id { get; set; }
        public string NctlId { get; set; }
        public string Name { get; set; }

        public ICollection<McOrganisationUser> McOrganisationUsers { get; set; }
    }
}
