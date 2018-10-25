using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Csv.Domain
{
    /// <summary>
    /// Used to get the data from the csv file. Maps property names to the field names
    /// </summary>
    public class UcasInstitutionProfile
    {
        public string inst_code { get; set; }	
        public string inst_code_alpha { get; set; }	
        public string inst_name { get; set; }	
        public string inst_person { get; set; }	
        public string inst_job { get; set; }	
        public string inst_address1 { get; set; }	
        public string inst_address2 { get; set; }	
        public string inst_address3 { get; set; }	
        public string inst_address4 { get; set; }
        public string inst_post_code { get; set; }
        public string inst_tel { get; set; }
        public string inst_fax { get; set; }
        public string region_code { get; set; }
        public string map_ref { get; set; }	
        public string primary_intake { get; set; }
        public string middle_intake { get; set; }
        public string secondary_intake { get; set; }
        public string further_ed { get; set; }
        public string coll_desc { get; set; }
        public string web_addr { get; set; }
        public string filenamey { get; set; }
        public string email { get; set; }
        public string scitt { get; set; }
    }
}
