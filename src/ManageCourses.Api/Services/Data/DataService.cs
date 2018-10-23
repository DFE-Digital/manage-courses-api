using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.EqualityComparers;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.Api.Services.Data
{
    public class DataService : IDataService
    {
        private readonly CourseLoader _courseLoader;
        private readonly IManageCoursesDbContext _context;
        IEnrichmentService _enrichmentService;
        private readonly IDataHelper _dataHelper;
        private readonly ILogger _logger;
        private readonly IPgdeWhitelist _pgdeWhitelist;

        public DataService(IManageCoursesDbContext context, IEnrichmentService enrichmentService, IDataHelper dataHelper, ILogger<DataService> logger, IPgdeWhitelist pgdeWhitelist)
        {
            _context = context;
            _enrichmentService = enrichmentService;
            _dataHelper = dataHelper;
            _logger = logger;
            _pgdeWhitelist = pgdeWhitelist;
            _courseLoader = new CourseLoader();
        }

        #region Import
        /// <summary>
        /// Processes data in the payload object into the database as an upsert/delta
        /// </summary>
        /// <param name="payload">Holds all the data entities that need to be imported</param>
        public void ProcessUcasPayload(UcasPayload payload)
        {
            var allInstitutions = payload.Institutions.ToList();

            _logger.LogWarning("Beginning UCAS import");
            _logger.LogInformation($"Upserting {allInstitutions.Count()} institutions");
            int processed = 0;
            foreach (var inst in allInstitutions)
            {
                using (var transaction = (_context as DbContext).Database.BeginTransaction())
                {
                    try 
                    {
                        UpsertInstitution(inst);
                        _context.Save();
                        DeleteForInstitution(inst.InstCode);
                        _context.Save();
                        AddForInstitution(inst.InstCode, payload);
                        _context.Save();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        _logger.LogError(e, $"UCAS import failed to update institution {inst.InstFull} [{inst.InstCode}]");
                    }
                }
                if (++processed % 100 == 0)
                {
                    _logger.LogInformation($"Upserted {processed} institutions so far");
                }
            }
            
            _logger.LogWarning("Completed UCAS import");
        }

        private void UpsertInstitution(UcasInstitution newValues)
        {
            newValues.InstCode = newValues.InstCode.ToUpperInvariant();
            var entity = _context.UcasInstitutions.FirstOrDefault(x => x.InstCode == newValues.InstCode);
            if (entity == null)
            {
                // insert
                _context.UcasInstitutions.Add(newValues);

            }
            else
            {
                // update
                entity.UpdateWith(newValues);
            }
        }

        private void DeleteForInstitution(string instCode)
        {
            var toDelete = new UcasPayload()
            {
                NoteTexts = _context.UcasNoteTexts.Where(c => c.InstCode == instCode).ToList(),
                CourseNotes = _context.UcasCourseNotes.Where(c => c.InstCode == instCode).ToList(),
                CourseSubjects = _context.UcasCourseSubjects.Where(c => c.InstCode == instCode).ToList(),
                Courses = _context.UcasCourses.Where(c => c.InstCode == instCode).ToList(),
                Campuses = _context.UcasCampuses.Where(c => c.InstCode == instCode).ToList()
            };

            _context.UcasNoteTexts.RemoveRange(toDelete.NoteTexts);
            _context.UcasCourseNotes.RemoveRange(toDelete.CourseNotes);
            _context.UcasCourseSubjects.RemoveRange(toDelete.CourseSubjects);
            _context.UcasCourses.RemoveRange(toDelete.Courses);
            _context.CourseCodes.RemoveRange(_context.CourseCodes.Where(c => c.InstCode == instCode));
            _context.UcasCampuses.RemoveRange(toDelete.Campuses);
        }

        private void AddForInstitution(string instCode, UcasPayload payload)
        {

            foreach (var campus in payload.Campuses.Where(c => c.InstCode == instCode))
            {
                _context.AddUcasCampus(
                    new UcasCampus
                    {
                        InstCode = campus.InstCode,
                        CampusCode = campus.CampusCode,
                        CampusName = campus.CampusName,
                        Addr1 = campus.Addr1,
                        Addr2 = campus.Addr2,
                        Addr3 = campus.Addr3,
                        Addr4 = campus.Addr4,
                        Postcode = campus.Postcode,
                        TelNo = campus.TelNo,
                        Email = campus.Email,
                        RegionCode = campus.RegionCode
                    });
            }

            var courseCodes = payload.Courses.Where(c => c.InstCode == instCode)
                .Select(c => new CourseCode()
                {
                    CrseCode = c.CrseCode,
                    InstCode = c.InstCode
                }).Distinct(new CourseCodeEquivalencyComparer());

            foreach (var course in courseCodes)
            {
                _context.CourseCodes.Add(new CourseCode
                {
                    CrseCode = course.CrseCode,
                    InstCode = course.InstCode
                });
            }


            foreach (var course in payload.Courses.Where(c => c.InstCode == instCode))
            {
                // copy props to prevent changing id
                _context.AddUcasCourse(new UcasCourse
                {
                    InstCode = course.InstCode,
                    CrseCode = course.CrseCode,
                    CrseTitle = course.CrseTitle,
                    Studymode = course.Studymode,
                    Age = course.Age,
                    CampusCode = course.CampusCode,
                    ProfpostFlag = course.ProfpostFlag,
                    ProgramType = course.ProgramType,
                    AccreditingProvider = course.AccreditingProvider,
                    CrseOpenDate = course.CrseOpenDate,
                    Publish = course.Publish,
                    Status = course.Status,
                    VacStatus = course.VacStatus,
                    HasBeenPublished = course.HasBeenPublished,
                    StartYear = course.StartYear,
                    StartMonth = course.StartMonth
                });
            }
            foreach (var courseSubject in payload.CourseSubjects.Where(c => c.InstCode == instCode))
            {
                _context.AddUcasCourseSubject(
                    new UcasCourseSubject
                    {
                        InstCode = courseSubject.InstCode,
                        CrseCode = courseSubject.CrseCode,
                        SubjectCode = courseSubject.SubjectCode,
                        YearCode = courseSubject.YearCode
                    }
                );
            }
            foreach (var courseNote in payload.CourseNotes.Where(c => c.InstCode == instCode))
            {
                _context.AddUcasCourseNote(
                    new UcasCourseNote
                    {
                        CrseCode = courseNote.CrseCode,
                        InstCode = courseNote.InstCode,
                        NoteNo = courseNote.NoteNo,
                        NoteType = courseNote.NoteType,
                        YearCode = courseNote.YearCode
                    });
            }

            foreach (var noteText in payload.NoteTexts.Where(c => c.InstCode == instCode))
            {
                _context.AddUcasNoteText(
                    new UcasNoteText
                    {
                        InstCode = noteText.InstCode,
                        NoteNo = noteText.NoteNo,
                        NoteType = noteText.NoteType,
                        LineText = noteText.LineText,
                        YearCode = noteText.YearCode
                    }
                );
            }
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
            
            var isPgde = _pgdeWhitelist.IsPgde(instCode, ucasCode);

            var course = _courseLoader.LoadCourse(courseRecords, enrichmentMetadata, isPgde);
            return course;
        }
        /// <summary>
        /// returns an InstitutionCourses object for a specified institution with the required courses mapped to a user email address
        /// </summary>
        /// <param name="email">user email address</param>
        /// <param name="instCode">the institution code</param>
        /// <returns>new InstitutionCourse object with a list of all courses found</returns>
        public InstitutionCourses GetCourses(string email, string instCode)
        {
            var returnCourses = new InstitutionCourses();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(instCode))
            {
                return returnCourses;
            }

            var courseRecords = _context.GetUcasCourseRecordsByInstCode(instCode, email);
            var enrichmentMetadata = _enrichmentService.GetCourseEnrichmentMetadata(instCode, email);
            var pgdeCourses = _pgdeWhitelist.ForInstitution(instCode);
            returnCourses = _courseLoader.LoadCourses(courseRecords, enrichmentMetadata, pgdeCourses);
            return returnCourses;
        }
        
        private void ResetReferenceSchema()
        {
            // clear out the existing data            
            _context.McOrganisationIntitutions.RemoveRange(_context.McOrganisationIntitutions);
            _context.UcasInstitutions.RemoveRange(_context.UcasInstitutions);
            _context.McOrganisations.RemoveRange(_context.McOrganisations);
            _context.McOrganisationUsers.RemoveRange(_context.McOrganisationUsers);
            _context.Save();
        }

        public class CourseComparer : IEqualityComparer<CourseCode>
        {
            public bool Equals(CourseCode x, CourseCode y)
            {
                return x.InstCode == y.InstCode && x.CrseCode == y.CrseCode;
            }

            public int GetHashCode(CourseCode cc)
            {
                return ($"{cc.InstCode}_{cc.CrseCode}").GetHashCode();
            }
        }
        #endregion

        public IEnumerable<UserOrganisation> GetOrganisationsForUser(string email)
        {
            var userOrganisations = _context.GetUserOrganisations(email)
                .Select(orgInst => new UserOrganisation()
                {
                    OrganisationId = orgInst.OrgId,
                    OrganisationName = orgInst.UcasInstitution.InstFull,
                    UcasCode = orgInst.InstitutionCode,
                    TotalCourses = orgInst.UcasInstitution.UcasCourses.Select(c => c.CrseCode).Distinct().Count()
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
                    OrganisationName = userOrganisation.UcasInstitution.InstFull,
                    UcasCode = userOrganisation.InstitutionCode,
                    TotalCourses = userOrganisation.UcasInstitution.UcasCourses.Select(c => c.CrseCode).Distinct()
                        .Count(),
                    EnrichmentWorkflowStatus = enrichment?.Status
                };
            }

            return null;
        }        

        public UcasInstitution GetUcasInstitutionForUser(string name, string instCode)
        {
            return _context.GetUcasInstitution(name, instCode);
        }
    }
}
