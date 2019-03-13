using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Domain;
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
        private readonly IClock _clock;

        public UcasDataMigrator(ManageCoursesDbContext manageCoursesDbContext, ILogger logger, UcasPayload payload, IClock clock = null)
        {
            _context = manageCoursesDbContext;
            _logger = logger;
            _clock = clock ?? new Clock();
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
            MigrateOnce("upsert subjects", () =>
            {
                foreach (var s in payload.Subjects)
                {
                    var savedSubject = UpsertSubject(new Subject
                    {
                        SubjectName = s.SubjectDescription,
                        SubjectCode = s.SubjectCode
                    });
                    allSubjects[savedSubject.SubjectCode] = savedSubject;
                }
            });

            var providerCache = _context.Providers
                .Include(x => x.Sites)
                .Include(x => x.Courses)
                .ToDictionary(p => p.ProviderCode);
            var upsertedProviders = new Dictionary<string, Provider>();
            MigratePerProvider("upsert providers", providerCache, inst =>
            {
                var savedProvider = UpsertProvider(ToProvider(inst), providerCache);
                _context.Save();
                upsertedProviders[savedProvider.ProviderCode] = savedProvider;
            });

            var courseLoader = new CourseLoader(upsertedProviders, allSubjects, pgdeCourses, _clock);


            MigratePerProvider("drop-and-create sites and courses", providerCache, ucasInst =>
            {
                var inst = upsertedProviders[ucasInst.InstCode];

                DeleteForProvider(inst.ProviderCode);
                _context.Save();

                var campuses = allCampusesGrouped.ContainsKey(inst.ProviderCode) ? allCampusesGrouped[inst.ProviderCode] : null;
                IEnumerable<Site> sites = new List<Site>();
                if (campuses != null)
                {
                    inst.Sites = inst.Sites ?? new Collection<Site>();
                    sites = campuses.Select(x => ToSite(x)).ToList();
                    foreach (var site in (IEnumerable<Site>)sites)
                    {
                        inst.Sites.Add(site);
                        site.Provider = inst;
                    }
                    _context.Save();
                }

                var allCoursesForThisProvider = courseLoader.LoadCourses(
                    inst,
                    ucasCourseGroupings.GetValueOrDefault(inst.ProviderCode).AsEnumerable() ?? new List<UcasCourse>(),
                    ucasCourseSubjectGroupings.GetValueOrDefault(inst.ProviderCode).AsEnumerable() ?? new List<UcasCourseSubject>(),
                    sites);

                inst.Courses = new Collection<Course>(allCoursesForThisProvider.ToList());
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

        private void MigratePerProvider(string operationName, Dictionary<string, Provider> providerCache,
            Action<UcasInstitution> action)
        {
            int processed = 0;

            _logger.Information($"Begin operation \"{operationName}\" on {payload.Institutions.Count()} institutions");

            foreach (var inst in payload.Institutions)
            {
                using (var transaction = (_context as DbContext).Database.BeginTransaction())
                {
                    try
                    {
                        if (providerCache.GetValueOrDefault(inst.InstCode)?.OptedIn == true)
                        {
                            _logger.Debug($"Skipped OptedIn provider {inst.InstCode}");
                            continue;
                        }
                        action(inst);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        _logger.Error(e, $"UCAS import operation \"{operationName}\"failed to update provider {inst.InstName} [{inst.InstCode}]");
                    }
                }
                if (++processed % 100 == 0)
                {
                    _logger.Information($"Ran operation \"{operationName}\" on {processed} providers so far");
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
                LocationName = x.CampusName,
                RegionCode = x.RegionCode,
                CreatedAt = _clock.UtcNow,
                UpdatedAt = _clock.UtcNow,
            };
        }

        private Provider ToProvider(UcasInstitution x)
        {
            return new Provider
            {
                Address1 = x.Addr1,
                Address2 = x.Addr2,
                Address3 = x.Addr3,
                Address4 = x.Addr4,
                Postcode = x.Postcode,
                RegionCode = x.RegionCode,

                ContactName = x.ContactName,
                Email = x.Email,
                Telephone = x.Telephone,
                Url = x.Url,

                ProviderName = x.InstFull,
                ProviderCode = x.InstCode,
                ProviderType = x.InstType,
                YearCode = x.YearCode,
                Scitt = x.Scitt,
                SchemeMember = x.SchemeMember,
                AccreditingProvider = x.AccreditingProvider,
            };
        }

        private Provider UpsertProvider(Provider newValues, IReadOnlyDictionary<string, Provider> providerCache)
        {
            newValues.ProviderCode = newValues.ProviderCode.ToUpperInvariant();
            var entity = providerCache.GetValueOrDefault(newValues.ProviderCode);
            if (entity == null)
            {
                // insert
                _context.Providers.Add(newValues);
                newValues.CreatedAt = _clock.UtcNow;
                newValues.UpdatedAt = _clock.UtcNow;
                return newValues;
            }
            else
            {
                // update
                entity.UpdateWith(newValues);
                entity.UpdatedAt = _clock.UtcNow;
                return entity;
            }
        }

        private void DeleteForProvider(string providerCode)
        {
            _context.Courses.RemoveRange(_context.Courses.Where(x => x.Provider.ProviderCode == providerCode));
            _context.Sites.RemoveRange(_context.Sites.Where(x => x.Provider.ProviderCode == providerCode));
        }
    }
}
