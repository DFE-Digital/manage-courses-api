using System;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class CourseSite
    {
        public int Id { get; set; }
        public Course Course {get; set;} 
        public Site Site {get; set;}         
        public DateTime? ApplicationsAcceptedFrom { get; set; }
        public string Status { get; set; }
        public string Publish { get; set; }
        public string VacStatus { get; set; }
    }
}