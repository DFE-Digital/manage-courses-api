using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class ProviderEnrichment
    {
        public int Id { get; set; }
        [Required]
        public string ProviderCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastPublishedAt { get; set; }
        public User CreatedByUser { get; set; }
        public User UpdatedByUser { get; set; }
        [Column(TypeName = "jsonb")]
        public string JsonData { get; set; }
        public EnumStatus Status { get; set; }
    }
}
