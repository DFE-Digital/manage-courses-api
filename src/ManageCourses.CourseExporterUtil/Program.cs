using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GovUk.Education.ManageCourses.Api;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.CourseExporterUtil
{
    public static class Program
    {
        public static void Main(string[] args)
        {   
            var context = GetContext();

            Console.WriteLine("Retrieve courses");

            var ucasCourses = context.UcasCourses.Include(x => x.UcasInstitution)
                .Include(x => x.UcasInstitution.UcasCourseSubjects).ThenInclude(x => x.UcasSubject)
                .Include(x => x.CourseCode).ThenInclude(x => x.UcasCourseSubjects)
                .Include(x => x.AccreditingProviderInstitution)
                .Include(x => x.UcasCampus)
                .Where(x => x.Publish == "Y")
                .ToList();
        
            Console.WriteLine("Retrieve institutions");
            var insts = context.UcasInstitutions.ToDictionary(x => x.InstCode);

            
            Console.WriteLine("Retrieve enrichments");

            var courseEnrichments = context.CourseEnrichments
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Where(x => x.Status == EnumStatus.Published)
                .ToLookup(x => x.InstCode + "_@@_" + x.UcasCourseCode)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.UpdatedTimestampUtc).First());

            var orgEnrichments = context.InstitutionEnrichments
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Where(x => x.Status == EnumStatus.Published)
                .ToLookup(x => x.InstCode)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.UpdatedTimestampUtc).First());
            
            var pgdeCourses = context.PgdeCourses.ToList();

            Console.WriteLine("Load courses");
            var courses = new CourseLoader().LoadCourses(ucasCourses, new List<UcasCourseEnrichmentGetModel>(), pgdeCourses);
            

            var courseMapper = new CourseMapper();
            var converter = new EnrichmentConverter();

            var mappedCourses = new List<SearchAndCompare.Domain.Models.Course>();

            Console.WriteLine("Combine courses with institution and enrichment data");

            foreach(var c in courses.Courses)
            {
                var mappedCourse = courseMapper.MapToSearchAndCompareCourse(
                    insts[c.InstCode],
                    c,
                    converter.Convert(orgEnrichments.GetValueOrDefault(c.InstCode))?.EnrichmentModel,
                    converter.Convert(courseEnrichments.GetValueOrDefault(c.InstCode + "_@@_" + c.CourseCode))?.EnrichmentModel
                );

                if (!mappedCourse.Campuses.Any())
                {
                    // only publish running courses
                    continue;
                }

                if (!mappedCourse.CourseSubjects.Any())
                {

                    Console.WriteLine($"failed to assign subject to [{c.InstCode}]/[{c.CourseCode}] {c.Name}. UCAS tags: {c.Subjects}");
                    // only publish courses we could map to one or more subjects.
                    continue;
                }

                // hacks - remove when coursemapper refactor has completed
                mappedCourse.ProviderLocation = new Location { Address = mappedCourse.ContactDetails.Address };
                mappedCourse.Fees = mappedCourse.Fees ?? new Fees();
                mappedCourse.Salary = mappedCourse.Salary ?? new Salary();

                mappedCourses.Add(mappedCourse);
            } 

            Console.WriteLine("Saving to JSON");

            var asJson = JsonConvert.SerializeObject(mappedCourses, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });

            System.IO.File.WriteAllText("out.json", asJson);

            Console.WriteLine("Done");
        }

        private static ManageCoursesDbContext GetContext()
        {
            var config =  new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddUserSecrets<Api.Startup>()
                .AddEnvironmentVariables()
                .Build();

            var mcConfig = new McConfig(config);
            mcConfig.Validate();
            var connectionString = mcConfig.BuildConnectionString();           
        
            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new ManageCoursesDbContext(options);           
        }
    }

}