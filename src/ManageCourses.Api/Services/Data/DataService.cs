﻿using System;
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
        /// <param name="ucasCode">the ucas code of the course</param>
        /// <returns>new Course object. Null if not found</returns>
        public Course GetCourse(string email, string instCode, string ucasCode)
        {

            var courseRecords = _context.GetUcasCourseRecordsByUcasCode(instCode, ucasCode, email);
            var enrichmentMetadata = _enrichmentService.GetCourseEnrichmentMetadata(instCode, email);

            if (courseRecords.Count == 0)
            {
                return null;
            }

            return courseRecords.Single();
        }

        /// <summary>
        /// returns an InstitutionCourses object for a specified institution with the required courses mapped to a user email address
        /// </summary>
        /// <param name="email">user email address</param>
        /// <param name="instCode">the institution code</param>
        /// <returns>new InstitutionCourse object with a list of all courses found</returns>
        public InstitutionCourses GetCourses(string email, string instCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(instCode))
            {
                return new InstitutionCourses();
            }

            var courseRecords = _context.GetUcasCourseRecordsByInstCode(instCode, email);

            if (courseRecords.Count == 0)
            {
                return new InstitutionCourses();
            }
            
            return new InstitutionCourses
            {
                Courses = courseRecords,
                InstitutionCode = courseRecords.First().InstCode,
                InstitutionName = courseRecords.First().Institution.InstFull
            };
        }

        public IEnumerable<UserOrganisation> GetOrganisationsForUser(string email)
        {
            var userOrganisations = _context.GetUserOrganisations(email)
                .Select(orgInst => new UserOrganisation()
                {
                    OrganisationId = orgInst.OrgId,
                    OrganisationName = orgInst.Institution.InstFull,
                    UcasCode = orgInst.InstCode,
                    TotalCourses = orgInst.Institution.Courses.Select(c => c.CourseCode).Distinct().Count()
                }).OrderBy(x => x.OrganisationName).ToList();

            return userOrganisations;
        }

        public UserOrganisation GetOrganisationForUser(string email, string instCode)
        {
            var userOrganisation = _context.GetUserOrganisation(email, instCode);
            var enrichment = _enrichmentService.GetInstitutionEnrichment(instCode, email);

            if (userOrganisation != null)
            {
                return new UserOrganisation()
                {
                    OrganisationId = userOrganisation.OrgId,
                    OrganisationName = userOrganisation.Institution.InstFull,
                    UcasCode = userOrganisation.InstCode,
                    TotalCourses = userOrganisation.Institution.Courses.Select(c => c.CourseCode).Distinct()
                        .Count(),
                    EnrichmentWorkflowStatus = enrichment?.Status
                };
            }

            return null;
        }        

        public Institution GetUcasInstitutionForUser(string name, string instCode)
        {
            return _context.GetInstitution(name, instCode);
        }
    }
}
