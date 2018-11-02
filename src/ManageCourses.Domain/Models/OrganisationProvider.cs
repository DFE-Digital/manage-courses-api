namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class OrganisationProvider
    {
        public int Id { get; set; }
        public Organisation Organisation { get; set; }
        public Institution Provider { get; set; }
    }
}
