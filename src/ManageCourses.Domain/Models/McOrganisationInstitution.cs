namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class McOrganisationInstitution
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string InstitutionCode { get; set; }

        public virtual McOrganisation McOrganisation { get; set; }
        public virtual UcasInstitution UcasInstitution { get; set; }
    }
}
