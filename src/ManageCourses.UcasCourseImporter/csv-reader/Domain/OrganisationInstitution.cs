using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Csv.Domain
{
    /// <summary>
    /// Used to get the data from the csv file. Maps property names to the field names
    /// </summary>
    internal class OrganisationInstitution
    {
        public string org_id { get; set; }
        public string institution_code { get; set; }
    }
}
