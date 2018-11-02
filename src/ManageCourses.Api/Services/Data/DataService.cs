using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
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
        /// <param name="instCode">the institution code</param>
        /// <param name="courseCode">the ucas code of the course</param>
        /// <returns>new Course object. Null if not found</returns>
        public Course GetCourseForUser(string email, string instCode, string courseCode)
        {

            var courseRecords = _context.GetCourse(instCode, courseCode, email);

            if (courseRecords.Count == 0)
            {
                return null;
            }

            return WithEnrichmentMetadata(courseRecords, instCode, email).Single();
        }

        /// <summary>
        /// returns an List&lt;Course&gt; object for a specified institution with the required courses mapped to a user email address
        /// </summary>
        /// <param name="email">user email address</param>
        /// <param name="instCode">the institution code</param>
        /// <returns>new InstitutionCourse object with a list of all courses found</returns>
        public List<Course> GetCoursesForUser(string email, string instCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(instCode))
            {
                return new List<Course>();
            }

            var courseRecords = _context.GetCoursesByInstCode(instCode, email);

            if (courseRecords.Count == 0)
            {
                return new List<Course>();
            }

            return WithEnrichmentMetadata(courseRecords, instCode, email).ToList();
        }

        public IEnumerable<InstitutionSummary> GetInstitutionSummariesForUser(string email)
        {
            var institutionSummaries = _context.GetOrganisationInstitutions(email)
                .Select(institutionSummary => new InstitutionSummary()
                {
                    InstName = institutionSummary.Provider.ProviderName,
                    InstCode = institutionSummary.Provider.ProviderCode,
                    TotalCourses = institutionSummary.Provider.Courses.Select(c => c.CourseCode).Distinct().Count()
                }).OrderBy(x => x.InstName).ToList();

            return institutionSummaries;
        }

        public InstitutionSummary GetInstitutionSummaryForUser(string email, string instCode)
        {
            var organisationInstitution = _context.GetOrganisationInstitution(email, instCode);
            var enrichment = _enrichmentService.GetInstitutionEnrichment(instCode, email);

            if (organisationInstitution != null)
            {
                return new InstitutionSummary()
                {
                    InstName = organisationInstitution.Provider.ProviderName,
                    InstCode = organisationInstitution.Provider.ProviderCode,
                    TotalCourses = organisationInstitution.Provider.Courses.Select(c => c.CourseCode).Distinct()
                        .Count(),
                    EnrichmentWorkflowStatus = enrichment?.Status
                };
            }

            return null;
        }

        public Provider GetUcasInstitutionForUser(string name, string instCode)
        {
            return _context.GetInstitution(name, instCode);
        }

        private IEnumerable<Course> WithEnrichmentMetadata(IEnumerable<Course> courseRecords, string instCode, string email)
        {
            var enrichmentMetadata = _enrichmentService.GetCourseEnrichmentMetadata(instCode, email);

            foreach (var course in courseRecords)
            {
                var bestEnrichment = enrichmentMetadata.Where(x => x.CourseCode == course.CourseCode).OrderByDescending(x => x.CreatedTimestampUtc).FirstOrDefault();
                course.EnrichmentWorkflowStatus = bestEnrichment?.Status;
            }
            return courseRecords;
        }
    }
}
