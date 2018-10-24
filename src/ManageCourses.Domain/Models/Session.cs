using System;
using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    ///<summary>
    /// Stores information about an OAuth Access Token so that consecutive requests can identify the user without calling out to the OAuth Server
    ///</summary>
    public class Session
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        public string AccessToken { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}
