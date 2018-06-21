using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Course = GovUk.Education.ManageCourses.Domain.Models.Course;

namespace GovUk.Education.ManageCourses.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private Dictionary<string, string> _dictProgramType = new Dictionary<string, string>();
        private Dictionary<string, string> _dictProgramOutcome = new Dictionary<string, string>();
        private Dictionary<string, string> _dictStudyMode = new Dictionary<string, string>();

        private readonly IManageCoursesDbContext _context;

        public DataController(IManageCoursesDbContext context)
        {
            _dictProgramType = new Dictionary<string, string>();
            _dictProgramOutcome = new Dictionary<string, string>();
            _dictStudyMode = new Dictionary<string, string>();
            _dictProgramType.Add("HE", "Higher education programme");
            _dictProgramType.Add("SD", "School Direct training programme");
            _dictProgramType.Add("SS", "School Direct (salaried) training programme");
            _dictProgramType.Add("SC", "SCITT programme ");
            _dictProgramType.Add("TA", "PG Teaching Apprenticeship");
            _dictProgramOutcome.Add("empty", "Recommendation for QTS");
            _dictProgramOutcome.Add("PF", "Professional");
            _dictProgramOutcome.Add("PG", "Postgraduate");
            _dictProgramOutcome.Add("BO", "Professional/Postgraduate");
            _dictStudyMode.Add("F", "Full time");
            _dictStudyMode.Add("P", "Part time");
            _context = context;
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [Authorize]
        [HttpGet]
        public IEnumerable<Course> Export()
        {
            var email = _context.McUsers.FirstOrDefault(u => u.Id == 6221).Email;//TODO wire this up when sign in is completed

            var courses = GetCoursesForUser(email);

            return courses;
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
            _context.UcasCourseSubjects.RemoveRange(_context.UcasCourseSubjects);
            _context.UcasCampuses.RemoveRange(_context.UcasCampuses);
            _context.UcasCourseNotes.RemoveRange(_context.UcasCourseNotes);
            _context.UcasNoteTexts.RemoveRange(_context.UcasNoteTexts);
            _context.McOrganisations.RemoveRange(_context.McOrganisations);
            _context.McOrganisationIntitutions.RemoveRange(_context.McOrganisationIntitutions);
            _context.McOrganisationUsers.RemoveRange(_context.McOrganisationUsers);
            _context.McUsers.RemoveRange(_context.McUsers);
            _context.ProviderMappers.RemoveRange(_context.ProviderMappers);
            _context.Save();
        }

        private IEnumerable<Course> GetCoursesForUser(string email)
        {
            var coursesToReturn = new List<Course>();
            var userOrganisationNctls = _context.McOrganisationUsers.Where(o => o.Email == email).Select(x => x.NctlId);
            var mappedUcasCodes = _context.ProviderMappers.Where(uo => userOrganisationNctls.Contains(uo.NctlId))
                .Select(x => x.UcasCode);
            var mappedCourses = _context.UcasCourses.Where(c => mappedUcasCodes.Contains(c.InstCode));

            foreach (var instCode in mappedUcasCodes.Distinct().ToList())
            {
                var titles = mappedCourses.Where(c => c.InstCode == instCode).Select(x => x.CrseTitle).Distinct().ToList();

                foreach (var title in titles)
                {
                    var course = new Course
                    {
                        UcasCode = instCode,
                        Title = title,
                        //Id = uc.Id,
                        Type = "todo-type", // todo: type
                    };
                    var accProviders = mappedCourses.Where(c => c.InstCode == instCode && c.CrseTitle == title)
                        .Select(x => x.AccreditingProvider).Distinct().ToList();
             
                    var variants = new List<Variant>();
                    foreach (var accProvider in accProviders)
                    {
                        var variant = new Variant {AccreditedProvider = accProvider};
                        var tempRecords = mappedCourses.Where(c =>
                                c.InstCode == instCode && c.CrseTitle == title && c.AccreditingProvider == accProvider)
                            .ToList();
                        var profPostFlags = GetDictionarValue(tempRecords.Select(x => x.ProfpostFlag).Distinct().ToList(), _dictProgramOutcome);//tempRecords.Select(x => x.ProfpostFlag).Distinct().ToArray();
                        var programTypes = GetDictionarValue(tempRecords.Select(x => x.ProgramType).Distinct().ToList(), _dictProgramType);//tempRecords.Select(x => x.ProgramType).Distinct().ToArray();
                        var studyModes = GetDictionarValue(tempRecords.Select(x => x.Studymode).Distinct().ToList(), _dictStudyMode);//tempRecords.Select(x => x.Studymode).Distinct().ToArray();
                        variant.ProfpostFlags = profPostFlags;
                        variant.ProgramTypes = programTypes;
                        variant.StudyModes = studyModes;
                        variants.Add(variant);
                    }
                    //TODO create a course model class with a collection of variants
                    var json = JsonConvert.SerializeObject(variants);
                    course.Type = json;//dont use type or this course class

                    coursesToReturn.Add(course);
                }
            }

            return coursesToReturn;
        }

        private string GetDictionarValue(List<string> keys, Dictionary<string, string> dict)
        {
            var returnVal = string.Empty;
            foreach (var key in keys)
            {
                var k = key;
                if (string.IsNullOrWhiteSpace(key))
                    k = "empty";

                if (dict.ContainsKey(k))
                {
                    returnVal += dict[k] + ",";
                }

            }

            return returnVal.TrimEnd(',');
        }
  
        private void ProcessPayload(Payload payload)
        {
            foreach (var course in payload.Courses)
            {
                // copy props to prevent changing id
                // todo: consider removing id from exposed API
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
                    CrseOpenDate = course.CrseOpenDate
                });
            }

            foreach (var institution in payload.Institutions)
            {
                _context.AddUcasInstitution(
                    new UcasInstitution
                    {
                        InstCode = institution.InstCode,
                        InstName = institution.InstName,
                        InstBig = institution.InstBig,
                        InstFull = institution.InstFull,
                        InstType = institution.InstType,
                        Addr1 = institution.Addr1,
                        Addr2 = institution.Addr2,
                        Addr3 = institution.Addr3,
                        Addr4 = institution.Addr4,
                        Postcode = institution.Postcode,
                        ContactName = institution.ContactName,
                        Url = institution.Url,
                        YearCode = institution.YearCode,
                        Scitt = institution.Scitt,
                        AccreditingProvider = institution.AccreditingProvider,
                        SchemeMember = institution.SchemeMember
                    }
                );
            }

            foreach (var courseSubject in payload.CourseSubjects)
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

            foreach (var subject in payload.Subjects)
            {
                _context.AddUcasSubject(
                    new UcasSubject
                    {
                        SubjectCode = subject.SubjectCode,
                        SubjectDescription = subject.SubjectDescription,
                        TitleMatch = subject.TitleMatch
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
                        CampusName = campus.CampusName,
                        Addr1 = campus.Addr1,
                        Addr2 = campus.Addr2,
                        Addr3 = campus.Addr3,
                        Addr4 = campus.Addr4,
                        Postcode = campus.Postcode,
                        TelNo = campus.TelNo,
                        Email = campus.Email,
                        RegionCode = campus.RegionCode
                    }
                );
            }

            foreach (var courseNote in payload.CourseNotes)
            {
                _context.AddUcasCourseNote(
                    new UcasCourseNote
                    {
                        CrseCode = courseNote.CrseCode,
                        InstCode = courseNote.InstCode,
                        NoteNo = courseNote.NoteNo,
                        NoteType = courseNote.NoteType,
                        YearCode = courseNote.YearCode
                    }
                    );
            }

            foreach (var noteText in payload.NoteTexts)
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

            foreach (var organisation in payload.Organisations)
            {
                _context.AddMcOrganisation(
                    new McOrganisation
                    {
                        NctlId = organisation.NctlId,
                        Name = organisation.Name
                    }
                    );
            }
            foreach (var organisatioInstitution in payload.OrganisationInstitutions)
            {
                _context.AddMcOrganisationInstitution(
                    new McOrganisationInstitution
                    {
                        NctlId = organisatioInstitution.NctlId,
                        InstitutionCode = organisatioInstitution.InstitutionCode
                    }
                );
            }
            foreach (var organisationUser in payload.OrganisationUsers)
            {
                _context.AddMcOrganisationUser(
                    new McOrganisationUser
                    {
                        NctlId = organisationUser.NctlId,
                        Email = organisationUser.Email
                    }
                );
            }
            foreach (var user in payload.Users)
            {
                _context.AddMcUser(
                    new McUser
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    }
                );
            }

            foreach (var mapper in payload.Mappers)
            {
                _context.AddProviderMapper(
                    new ProviderMapper
                    {
                        InstitutionName = mapper.InstitutionName,
                        NctlId = mapper.NctlId,
                        Type = mapper.Type,
                        UcasCode = mapper.UcasCode,
                        Urn = mapper.Urn
                    }
                );
            }
            _context.Save();
        }
    }
}

