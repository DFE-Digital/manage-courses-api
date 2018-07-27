using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Api.Middleware
{
    /// <summary>
    /// Claims from DfE Sign-in.
    /// Use this class for deserializing and inspecting.
    /// </summary>
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
