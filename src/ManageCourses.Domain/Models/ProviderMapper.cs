using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class ProviderMapper
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string UcasCode { get; set; }
        public int Urn { get; set; }
        public string Type { get; set; }
        public string InstitutionName { get; set; }
    }
}
