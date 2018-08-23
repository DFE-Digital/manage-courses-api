using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class NctlOrganisation
    {
        public int Id { get; set; }

        [Required]
        public string NctlId { get; set; }

        public string OrgId { get; set; }

        public string Name { get; set; }

        public McOrganisation McOrganisation { get; set; }
    }
}
