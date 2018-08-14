using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class InstitutionEnrichment
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public DateTime CreatedTimestampUtc { get; set; }
        public DateTime UpdateTimestampUtc { get; set; }
        public int SavedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        [Column(TypeName = "jsonb")]
        public string JsonData { get; set; }
        public UcasInstitution UcasInstitution { get; set; }
    }
}
 