using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class Provider
    {
        public Provider()
        {
            Courses = new List<Course>();
        }

        public void UpdateWith(Provider provider)
        {
            ProviderName = provider.ProviderName;
            ProviderType = provider.ProviderType;
            Address1 = provider.Address1;
            Address2 = provider.Address2;
            Address3 = provider.Address3;
            Address4 = provider.Address4;
            Postcode = provider.Postcode;
            ContactName = provider.ContactName;
            Email = provider.Email;
            Telephone = provider.Telephone;
            Url = provider.Url;
            YearCode = provider.YearCode;
            Scitt = provider.Scitt;
            SchemeMember = provider.SchemeMember;
            RegionCode = provider.RegionCode;
        }

        public int Id { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ProviderType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public int? RegionCode { get; set; }
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
