using System;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    ///<summary>
    /// Stores information about an OAuth Access Token so that consecutive requests can identify the user without calling out to the OAuth Server
    ///</summary>
    public class McSession
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public McUser McUser { get; set; }

        public string AccessToken { get; set; }

        public string Subject { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}
