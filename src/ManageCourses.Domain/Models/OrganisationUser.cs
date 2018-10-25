using System.Diagnostics;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class OrganisationUser
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Organisation Organisation { get; set; }

        public string DebuggerDisplay => $"OrganisationUser: OrgId {Organisation?.Id} <--> User {User?.Email}";
    }
}
