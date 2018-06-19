using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class McUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public ICollection<McOrganisationUser> McOrganisationUsers { get; set; }
    }
}
