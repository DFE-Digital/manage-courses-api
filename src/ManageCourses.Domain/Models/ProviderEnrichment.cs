using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    /// <summary>
    /// A historical trail of all of the published versions of the enrichment.
    /// For each provider there are 0-n published records (latest plus history),
    /// plus 0-1 draft record(s).
    /// </summary>
    public class ProviderEnrichment
    {
        public int Id { get; set; }

        [Required]
        public string ProviderCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// For published records this is when the record was published.
        /// For the draft record this is a copy of the published date for the most recently published record.
        /// </summary>
        public DateTime? LastPublishedAt { get; set; }

        public User CreatedByUser { get; set; }

        public User UpdatedByUser { get; set; }

        [Column(TypeName = "jsonb")]

        public string JsonData { get; set; }

        public EnumStatus Status { get; set; }

        public Provider Provider { get; set; }
    }
}
