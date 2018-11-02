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

        private readonly ILogger _logger;
        private readonly UcasPayload payload;

        public UcasDataMigrator(ManageCoursesDbContext manageCoursesDbContext, ILogger logger,UcasPayload payload)
        {
            _context = manageCoursesDbContext;
            _logger = logger;
            this.payload = payload;
        }
        
        /// <summary>
        /// Processes data in the payload object into the database as an upsert/delta
        /// </summary>
        public void UpdateUcasData()
        {
            var allCampusesGrouped = payload.Campuses.GroupBy(x => x.InstCode).ToDictionary(x => x.Key);
            var ucasSubjects = payload.Subjects.ToList();
            var pgdeCourses = _context.PgdeCourses.ToList();

            var ucasCourseGroupings = payload.Courses.GroupBy(x => x.InstCode).ToDictionary(x => x.Key);
            var ucasCourseSubjectGroupings = payload.CourseSubjects.GroupBy(x => x.InstCode).ToDictionary(x => x.Key);

            _logger.Warning("Beginning UCAS import");
            _logger.Information($"Upserting {payload.Institutions.Count()} institutions");

            var allSubjects = new Dictionary<string, Subject>();
            MigrateOnce("upsert subjects", () => {
                foreach(var s in payload.Subjects)
                {
                    var savedSubject = UpsertSubject(new Subject {
                        SubjectName = s.SubjectDescription,
                        SubjectCode = s.SubjectCode
                    });
                    allSubjects[savedSubject.SubjectCode] = savedSubject;
                }
            });

            var allInstitutions = new Dictionary<string, Provider>();
            MigratePerInstitution("upsert institutions", inst => {
                var savedInst = UpsertInstitution(ToInstitution(inst));
                _context.Save();
                allInstitutions[savedInst.ProviderCode] = savedInst;
            });

            var courseLoader = new CourseLoader(allInstitutions, allSubjects, pgdeCourses);


            MigratePerInstitution("drop-and-create sites and courses", ucasInst => {
                var inst = allInstitutions[ucasInst.InstCode];

                DeleteForInstitution(inst.ProviderCode);
                _context.Save();

                var campuses = allCampusesGrouped.ContainsKey(inst.ProviderCode) ? allCampusesGrouped[inst.ProviderCode] : null;
                IEnumerable<Site> sites = new List<Site>();
                if (campuses != null)
                {
                    inst.Sites = inst.Sites ?? new Collection<Site>();
                    sites = campuses.Select(x => ToSite(x)).ToList();
                    foreach(var site in (IEnumerable<Site>) sites)
                    {
                        inst.Sites.Add(site);                                
                        site.Provider = inst;
                    }
                    _context.Save();
                }                

                var allCoursesForThisInstitution = courseLoader.LoadCourses(
                    inst,
                    ucasCourseGroupings.GetValueOrDefault(inst.ProviderCode).AsEnumerable() ?? new List<UcasCourse>(), 
                    ucasCourseSubjectGroupings.GetValueOrDefault(inst.ProviderCode).AsEnumerable() ?? new List<UcasCourseSubject>(),
                    sites);

                inst.Courses = new Collection<Course>(allCoursesForThisInstitution.ToList());       
                _context.Save();
            });
            
            _logger.Warning("Completed UCAS import");
        }

        private Subject UpsertSubject(Subject subject)
        {
            var entity = _context.Subjects.Where(x => x.SubjectCode == subject.SubjectCode).FirstOrDefault();
            if (entity == null)
            {
                _context.Add(subject);
                return subject;
            }
            else
            {
                entity.SubjectName = subject.SubjectName;
                return entity;
            }
        }

        private void MigratePerInstitution(string operationName, Action<UcasInstitution> action)
        {            
            int processed = 0;   

            _logger.Information($"Begin operation \"{operationName}\" on {payload.Institutions.Count()} institutions");

            foreach (var inst in payload.Institutions)
            {
                using (var transaction = (_context as DbContext).Database.BeginTransaction())
                {
                    try 
                    {
                        action(inst);
                        transaction.Commit();
                    }                    
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        _logger.Error(e, $"UCAS import operation \"{operationName}\"failed to update institution {inst.InstName} [{inst.InstCode}]");
                    }
                }                
                if (++processed % 100 == 0)
                {
                    _logger.Information($"Ran operation \"{operationName}\" on {processed} institutions so far");
                }
            }
            _logger.Information($"Finished operation \"{operationName}\"");
        }

        private void MigrateOnce(string operationName, Action action)
        {            
            _logger.Information($"Begin operation \"{operationName}\"");

            using (var transaction = (_context as DbContext).Database.BeginTransaction())
            {
                try 
                {
                    action();
                    transaction.Commit();
                }                    
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.Error(e, $"UCAS import operation \"{operationName}\"failed");
                }
            }       
            _logger.Information($"Finished operation \"{operationName}\"");
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

        private Provider ToInstitution(UcasInstitution x)
        {
            return new Provider
            {
                Address1 = x.Addr1,
                Address2 = x.Addr2,
                Address3 = x.Addr3,
                Address4 = x.Addr4,
                ContactName = x.ContactName,
                Email = x.Email,
                Telephone = x.Telephone,
                Url = x.Url,

                ProviderName = x.InstFull,
                ProviderCode = x.InstCode,
                ProviderType = x.InstType,
                YearCode = x.YearCode,
                Scitt = x.Scitt,
                SchemeMember = x.SchemeMember
            };
        }

        private Provider UpsertInstitution(Provider newValues)
        {
            newValues.ProviderCode = newValues.ProviderCode.ToUpperInvariant();
            var entity = _context.Providers
                .Include(x => x.Sites).Include(x => x.Courses)
                .FirstOrDefault(x => x.ProviderCode == newValues.ProviderCode);
            if (entity == null)
            {
                // insert
                _context.Providers.Add(newValues);
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
            _context.Courses.RemoveRange(_context.Courses.Where(x => x.Provider.ProviderCode == instCode));
            _context.Sites.RemoveRange(_context.Sites.Where(x => x.Provider.ProviderCode == instCode));
        }
    }
}