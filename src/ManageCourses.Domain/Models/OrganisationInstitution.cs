namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class OrganisationInstitution
    {
        public int Id { get; set; }
        public Organisation Organisation { get; set; }
        public Institution Institution { get; set; }
    }
}
