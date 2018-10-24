using System;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    public class UcasDataMigrator
    {
        private readonly ManageCoursesDbContext _context;
        
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
            var allInstitutions = payload.Institutions.ToList();

            _logger.Warning("Beginning UCAS import");
            _logger.Information($"Upserting {allInstitutions.Count()} institutions");
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
    }
}