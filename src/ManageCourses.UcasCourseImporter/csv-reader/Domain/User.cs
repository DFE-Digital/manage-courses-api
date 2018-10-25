using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Csv.Domain
{
    /// <summary>
    /// Used to get the data from the csv file. Maps property names to the field names
    /// </summary>
    internal class User
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
    }
}
