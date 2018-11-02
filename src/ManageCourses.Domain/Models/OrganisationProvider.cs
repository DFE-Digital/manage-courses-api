namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class OrganisationProvider
    {
        public int Id { get; set; }
        public Organisation Organisation { get; set; }
        public Provider Provider { get; set; }
    }
}
