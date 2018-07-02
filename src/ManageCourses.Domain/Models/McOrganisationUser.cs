namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class McOrganisationUser
    {
        public int Id { get; set; }

        public string OrgId { get; set; }

        public string Email { get; set; }

        public virtual McUser McUser { get; set; }

        public virtual McOrganisation McOrganisation { get; set; }
    }
}
