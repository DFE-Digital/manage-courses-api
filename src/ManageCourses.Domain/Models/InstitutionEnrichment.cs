using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class InstitutionEnrichment
    {
        public int Id { get; set; }
        [Required]
        public string InstCode { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }
        public DateTime UpdatedTimestampUtc { get; set; }
        public DateTime? LastPublishedTimestampUtc { get; set; }
        public McUser CreatedByUser { get; set; }
        public McUser UpdatedByUser { get; set; }
        [Column(TypeName = "jsonb")]
        public string JsonData { get; set; }
        public EnumStatus Status { get; set; }
    }
}
 