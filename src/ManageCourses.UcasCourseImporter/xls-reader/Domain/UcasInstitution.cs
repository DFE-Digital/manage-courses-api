using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Xls.Domain
{
    public class UcasInstitution
    {
        public void UpdateWith(UcasInstitution inst)
        {
            InstName = inst.InstName;
            InstBig = inst.InstBig;
            InstFull = inst.InstFull;
            InstType = inst.InstType;
            Addr1 = inst.Addr1;
            Addr2 = inst.Addr2;
            Addr3 = inst.Addr3;
            Addr4 = inst.Addr4;
            Postcode = inst.Postcode;
            ContactName = inst.ContactName;
            Email = inst.Email;
            Telephone = inst.Telephone;
            Url = inst.Url;
            YearCode = inst.YearCode;
            Scitt = inst.Scitt;
            SchemeMember = inst.SchemeMember;
            RegionCode = inst.RegionCode;
        }

        public int Id { get; set; }
        public string InstCode { get; set; }
        public string InstName { get; set; }
        public string InstBig { get; set; }
        public string InstFull { get; set; }
        public string InstType { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public string Postcode { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Url { get; set; }
        public string YearCode { get; set; }
        public string Scitt { get; set; }
        public string AccreditingProvider { get; set; }
        public string SchemeMember { get; set; }
        public string RegionCode { get; set; }
    }
}
