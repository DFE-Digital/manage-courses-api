namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class McOrganisationInstitution
    {
        public int Id { get; set; }
        public McOrganisation McOrganisation { get; set; }
        public Institution Institution { get; set; }
    }
}
