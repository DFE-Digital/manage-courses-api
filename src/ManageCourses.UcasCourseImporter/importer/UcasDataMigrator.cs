using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.UcasCourseImporter.Mapping;
using GovUk.Education.ManageCourses.Xls.Domain;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    public class UcasDataMigrator
    {
        private readonly ManageCoursesDbContext _context;

        private readonly CourseLoader _courseLoader = new CourseLoader();
        
        private readonly ILogger _logger;

        public UcasDataMigrator(ManageCoursesDbContext manageCoursesDbContext, ILogger logger)
        {
            _context = manageCoursesDbContext;
            _logger = logger;
        }
        
        /// <summary>
        /// Processes data in the payload object into the database as an upsert/delta
        /// </summary>
        /// <param name="payload">Holds all the data entities that need to be imported</param>
        public void UpdateUcasData(UcasPayload payload)
        {
            var allInstitutions = payload.Institutions.Select(x => ToInstitution(x)).ToList();
            var allCampusesGrouped = payload.Campuses.GroupBy(x => x.InstCode).ToDictionary(x => x.Key);
            var ucasSubjects = payload.Subjects.ToList();
            var pgdeCourses = _context.PgdeCourses.ToList();
            var allSubjects = _context.Subjects.ToList();

            var ucasCourseGroupings = payload.Courses.GroupBy(x => x.InstCode).ToDictionary(x => x.Key);
            var ucasCourseSubjectGroupings = payload.CourseSubjects.GroupBy(x => x.InstCode).ToDictionary(x => x.Key);

            _logger.Warning("Beginning UCAS import");
            _logger.Information($"Upserting {allInstitutions.Count()} institutions");
            int processed = 0;
            foreach (var inst in allInstitutions)
            {
                using (var transaction = (_context as DbContext).Database.BeginTransaction())
                {
                    try 
                    {
                        var savedInst = UpsertInstitution(inst);
                        _context.Save();
                        DeleteForInstitution(inst.InstCode);
                        _context.Save();

                        var campuses = allCampusesGrouped.ContainsKey(inst.InstCode) ? allCampusesGrouped[inst.InstCode] : null;
                        IEnumerable<Site> sites = new List<Site>();
                        if (campuses != null)
                        {
                            savedInst.Sites = savedInst.Sites ?? new Collection<Site>();
                            sites = campuses.Select(x => ToSite(x)).ToList();
                            foreach(var site in (IEnumerable<Site>) sites)
                            {
                                savedInst.Sites.Add(site);                                
                                site.Institution = savedInst;
                            }
                            _context.Save();
                        }
                        

                        IEnumerable<UcasCourse> ucasCourses = ucasCourseGroupings.GetValueOrDefault(inst.InstCode);
                        IEnumerable<UcasCourseSubject> ucasCourseSubjects = ucasCourseSubjectGroupings.GetValueOrDefault(inst.InstCode);

                        var allCoursesForThisInstitution = _courseLoader.LoadCourses(ucasCourses ?? new List<UcasCourse>(), ucasCourseSubjects ?? new List<UcasCourseSubject>(), ucasSubjects, pgdeCourses, allSubjects, sites, allInstitutions);
                        AddForInstitution(allCoursesForThisInstitution, savedInst);
                        _context.Save();
                        
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        _logger.Error(e, $"UCAS import failed to update institution {inst.InstFull} [{inst.InstCode}]");
                    }
                }
                if (++processed % 100 == 0)
                {
                    _logger.Information($"Upserted {processed} institutions so far");
                }
            }
            
            _logger.Warning("Completed UCAS import");
        }

        private Site ToSite(UcasCampus x)
        {
            return new Site
            {
                Address1 = x.Addr1, 
                Address2 = x.Addr2, 
                Address3 = x.Addr3, 
                Address4 = x.Addr4, 
                Postcode = x.Postcode,
                Code = x.CampusCode,
                LocationName = x.CampusName
            };
        }

        private Institution ToInstitution(UcasInstitution x)
        {
            return new Institution
            {
                Address1 = x.Addr1,
                Address2 = x.Addr2,
                Address3 = x.Addr3,
                Address4 = x.Addr4,
                ContactName = x.ContactName,
                Email = x.Email,
                Telephone = x.Telephone,
                Url = x.Url,

                InstFull = x.InstFull,
                InstCode = x.InstCode,
                InstType = x.InstType,
                YearCode = x.YearCode,
                Scitt = x.Scitt,
                SchemeMember = x.SchemeMember
            };
        }

        private Institution UpsertInstitution(Institution newValues)
        {
            newValues.InstCode = newValues.InstCode.ToUpperInvariant();
            var entity = _context.Institutions
                .Include(x => x.Sites).Include(x => x.Courses)
                .FirstOrDefault(x => x.InstCode == newValues.InstCode);
            if (entity == null)
            {
                // insert
                _context.Institutions.Add(newValues);
                return newValues;
            }
            else
            {
                // update
                entity.UpdateWith(newValues);
                return entity;
            }
        }

        private void DeleteForInstitution(string instCode)
        {
            _context.Courses.RemoveRange(_context.Courses.Where(x => x.Institution.InstCode == instCode));
            _context.Sites.RemoveRange(_context.Sites.Where(x => x.Institution.InstCode == instCode));
        }

        private void AddForInstitution(IEnumerable<Course> courses, Institution inst)
        {
            inst.Courses = inst.Courses ?? new Collection<Course>();
            foreach(var course in courses)
            {
                inst.Courses.Add(course);
            }
        }
    }
}