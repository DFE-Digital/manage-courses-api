using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasInstitution
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string FullName { get; set; }
        public string InstType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PostCode { get; set; }
        public string ContactName { get; set; }
        public string Url { get; set; }
        public string Scitt { get; set; }
        public string AcreditedProvider { get; set; }
    }
}
