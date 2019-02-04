using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class CourseEnrichment
    {
        public int Id { get; set; }
        [Required]
        public string ProviderCode { get; set; }
        [Required]
        public string UcasCourseCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastPublishedTimestampUtc { get; set; }
        public User CreatedByUser { get; set; }
        public User UpdatedByUser { get; set; }
        [Column(TypeName = "jsonb")]
        public string JsonData { get; set; }
        public EnumStatus Status { get; set; }
    }
}
