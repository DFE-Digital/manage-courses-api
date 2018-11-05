using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.Api.Services.Data
{
    public class DataService : IDataService
    {
        private readonly IManageCoursesDbContext _context;
        IEnrichmentService _enrichmentService;
        private readonly ILogger _logger;

        public DataService(IManageCoursesDbContext context, IEnrichmentService enrichmentService, ILogger<DataService> logger)
        {
            _context = context;
            _enrichmentService = enrichmentService;
            _logger = logger;
        }

        /// <summary>
        /// returns a Course object containing all the required fields
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="providerCode">the provider code</param>
        /// <param name="courseCode">the ucas code of the course</param>
        /// <returns>new Course object. Null if not found</returns>
        public Course GetCourseForUser(string email, string providerCode, string courseCode)
        {

            var courseRecords = _context.GetCourse(providerCode, courseCode, email);

            if (courseRecords.Count == 0)
            {
                return null;
            }

            return WithEnrichmentMetadata(courseRecords, providerCode, email).Single();
        }

        /// <summary>
        /// returns an List&lt;Course&gt; object for a specified provider with the required courses mapped to a user email address
        /// </summary>
        /// <param name="email">user email address</param>
        /// <param name="providerCode">the provider code</param>
        /// <returns>new ProviderCourse object with a list of all courses found</returns>
        public List<Course> GetCoursesForUser(string email, string providerCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(providerCode))
            {
                return new List<Course>();
            }

            var courseRecords = _context.GetCoursesByProviderCode(providerCode, email);

            if (courseRecords.Count == 0)
            {
                return new List<Course>();
            }

            return WithEnrichmentMetadata(courseRecords, providerCode, email).ToList();
        }

        public Provider GetUcasProviderForUser(string name, string providerCode)
        {
            return _context.GetProvider(name, providerCode);
        }

        private IEnumerable<Course> WithEnrichmentMetadata(IEnumerable<Course> courseRecords, string providerCode, string email)
        {
            var enrichmentMetadata = _enrichmentService.GetCourseEnrichmentMetadata(providerCode, email);

            foreach (var course in courseRecords)
            {
                var bestEnrichment = enrichmentMetadata.Where(x => x.CourseCode == course.CourseCode).OrderByDescending(x => x.CreatedTimestampUtc).FirstOrDefault();
                course.EnrichmentWorkflowStatus = bestEnrichment?.Status;
            }
            return courseRecords;
        }
    }
}
