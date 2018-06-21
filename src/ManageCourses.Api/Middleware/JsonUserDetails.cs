using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class JsonUserDetails
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("sub")]
        public string Subject { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
    }
}
