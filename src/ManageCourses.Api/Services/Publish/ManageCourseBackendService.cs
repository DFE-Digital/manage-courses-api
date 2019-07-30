using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public class ManageCoursesBackendService : IManageCoursesBackendService
    {

        private readonly HttpClient _httpClient;
        private readonly IManageCoursesBackendJwtService _manageCoursesBackendJwtService;

        public ManageCoursesBackendService(HttpClient httpClient, IManageCoursesBackendJwtService manageCoursesBackendJwtService)
        {
            _httpClient = httpClient;
            _manageCoursesBackendJwtService = manageCoursesBackendJwtService;
        }
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="providerCode">provider code for the courses</param>
        /// <param name="email">email of the user</param>
        /// <returns>true if successful</returns>
        public async Task<bool> SaveCourses(string providerCode, string email)
        {
            var postUrl = $"/api/v2/providers/{providerCode}/publish";

            if (string.IsNullOrWhiteSpace(providerCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _manageCoursesBackendJwtService.GetCurrentUserToken());

            var result = await _httpClient.PostAsJsonAsync(postUrl, new {});

            return result.IsSuccessStatusCode;
        }
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="providerCode">provider code for the courses</param>
        /// <param name="courseCode">code for the course (if a single course is to be published)</param>
        /// <param name="email">email of the user</param>
        /// <returns>true if successful</returns>
        public async Task<bool> SaveCourse(string providerCode, string courseCode, string email)
        {
            var postUrl = $"/api/v2/providers/{providerCode}/courses/{courseCode}/publish";

            if (string.IsNullOrWhiteSpace(providerCode) || string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _manageCoursesBackendJwtService.GetCurrentUserToken());

            var result = await _httpClient.PostAsJsonAsync(postUrl, new {});

            return result.IsSuccessStatusCode;
        }
    }
}
