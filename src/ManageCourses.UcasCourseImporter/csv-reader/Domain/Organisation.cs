using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Csv.Domain
{
    /// <summary>
    /// Used to get the data from the csv file. Maps property names to the field names
    /// </summary>
    internal class Organisation
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
