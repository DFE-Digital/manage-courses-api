using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Course = GovUk.Education.ManageCourses.Domain.Models.Course;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly IManageCoursesDbContext _context;

        public DataController(IManageCoursesDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [HttpGet]
        public IEnumerable<Course> Export()
        {
            return _context.GetAll();
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        [HttpPost]
        public void Import([FromBody] Payload payload)
        {
            ResetDatabase();

            ProcessPayload(payload);

            //TODO return Ok/Fail in action result
        }

        private void ResetDatabase()
        {
            // clear out the existing data
            _context.UcasCourses.RemoveRange(_context.UcasCourses);
            _context.UcasInstitutions.RemoveRange(_context.UcasInstitutions);
            _context.UcasSubjects.RemoveRange(_context.UcasSubjects);
            _context.UcasCampuses.RemoveRange(_context.UcasCampuses);
            _context.UcasCourseNotes.RemoveRange(_context.UcasCourseNotes);
            _context.UcasNoteTexts.RemoveRange(_context.UcasNoteTexts);
            _context.Save();
        }

        private void ProcessPayload(Payload payload)
        {
            foreach (var course in payload.Courses)
            {
                // copy props to prevent changing id
                // todo: consider removing id from exposed API
                _context.AddUcasCourse(new UcasCourse
                {
                    Title = course.Title,
                    CourseCode = course.CourseCode,
                    InstCode = course.InstCode,
                    StudyMode = course.StudyMode,
                    AgeGroup = course.AgeGroup,
                    CampusCode = course.CampusCode,
                    ProfPostFlag = course.ProfPostFlag,
                    ProgramType = course.ProgramType,
                    AcreditedProvider = course.AcreditedProvider,
                    OpenDate = course.OpenDate
                });
            }

            foreach (var institution in payload.Institutions)
            {
                _context.AddUcasInstitution(
                    new UcasInstitution
                    {
                        InstCode = institution.InstCode,
                        FullName = institution.FullName,
                        InstType = institution.InstType,
                        Address1 = institution.Address1,
                        Address2 = institution.Address2,
                        Address3 = institution.Address3,
                        Address4 = institution.Address4,
                        PostCode = institution.PostCode,
                        ContactName = institution.ContactName,
                        Url = institution.Url,
                        Scitt = institution.Scitt,
                        AcreditedProvider = institution.AcreditedProvider
                    }
                );
            }

            foreach (var subject in payload.Subjects)
            {
                _context.AddUcasSubject(
                    new UcasSubject
                    {
                        InstCode = subject.InstCode,
                        CourseCode = subject.CourseCode,
                        SubjectCode = subject.SubjectCode
                    }
                );
            }

            foreach (var campus in payload.Campuses)
            {
                _context.AddUcasCampus(
                    new UcasCampus
                    {
                        InstCode = campus.InstCode,
                        CampusCode = campus.CampusCode,
                        Address1 = campus.Address1,
                        Address2 = campus.Address2,
                        Address3 = campus.Address3,
                        Address4 = campus.Address4,
                        PostCode = campus.PostCode,
                        TelNo = campus.TelNo,
                        RegionCode = campus.RegionCode
                    }
                );
            }

            foreach (var courseNote in payload.CourseNotes)
            {
                _context.AddUcasCourseNote(
                    new UcasCourseNote
                    {
                        CourseCode = courseNote.CourseCode,
                        InstCode = courseNote.InstCode,
                        NoteNo = courseNote.NoteNo,
                        NoteType = courseNote.NoteType,
                    }
                    );
            }

            foreach (var noteText in payload.NoteTexts)
            {
                _context.AddUcasNoteText(
                    new UcasNoteText
                    {
                        InstCode = noteText.InstCode,
                        LineText = noteText.LineText,
                        NoteNo = noteText.NoteNo,
                        NoteType = noteText.NoteType
                    }
                );
            }
            _context.Save();
        }
    }
}

