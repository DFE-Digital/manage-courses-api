using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public class ManageCoursesBackendService : IManageCoursesBackendService
    {

        private readonly HttpClient _httpClient;
        public ManageCoursesBackendService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="providerCode">provider code for the courses</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveCourses(string providerCode, string email)
        {
            var postUrl = $"/api/v2/providers/{providerCode}/publish";

            if (string.IsNullOrWhiteSpace(providerCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }


            var result = await _httpClient.PostAsJsonAsync(postUrl, new {});

            return result.IsSuccessStatusCode;
        }
        /// <summary>
        /// Publishes a list of courses to Search and Compare
        /// </summary>
        /// <param name="providerCode">provider code for the courses</param>
        /// <param name="courseCode">code for the course (if a single course is to be published)</param>
        /// <param name="email">email of the user</param>
        /// <returns></returns>
        public async Task<bool> SaveCourse(string providerCode, string courseCode, string email)
        {
            var postUrl = $"/api/v2/providers/{providerCode}/courses/{courseCode}/publish";

            if (string.IsNullOrWhiteSpace(providerCode) || string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var result = await _httpClient.PostAsJsonAsync(postUrl, new {});

            return result.IsSuccessStatusCode;
        }
    }
}
