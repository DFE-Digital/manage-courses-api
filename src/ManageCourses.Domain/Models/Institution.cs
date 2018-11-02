using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class Institution
    {
        public Institution()
        {
            Courses = new List<Course>();
        }

        public void UpdateWith(Institution inst)
        {
            ProviderName = inst.ProviderName;
            InstType = inst.InstType;
            Address1 = inst.Address1;
            Address2 = inst.Address2;
            Address3 = inst.Address3;
            Address4 = inst.Address4;
            Postcode = inst.Postcode;
            ContactName = inst.ContactName;
            Email = inst.Email;
            Telephone = inst.Telephone;
            Url = inst.Url;
            YearCode = inst.YearCode;
            Scitt = inst.Scitt;
            SchemeMember = inst.SchemeMember;
        }

        public int Id { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string InstType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Url { get; set; }
        public string YearCode { get; set; }
        public string Scitt { get; set; }
        public string SchemeMember { get; set; }

        public ICollection<OrganisationProvider> OrganisationProviders { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Course> AccreditedCourses { get; set; }
        public ICollection<Site> Sites { get; set; }
    }
}
