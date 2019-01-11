using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Xls.Domain
{
    public class UcasCampus
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CampusCode { get; set; }
        public string CampusName { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public string Postcode { get; set; }
        public string TelNo { get; set; }
        public string Email { get; set; }
        public int? RegionCode { get; set; }
    }
}
