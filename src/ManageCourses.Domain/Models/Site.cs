﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class Site
    {
        public int Id { get; set; }

        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public IEnumerable<CourseSite> CourseSites { get; set; }

        public string LocationName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string Code { get; set; }
        public int? RegionCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
