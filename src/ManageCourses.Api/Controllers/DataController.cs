using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Mapping;
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

        private readonly IManageCoursesDbContext _context;

        public DataController(IManageCoursesDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <returns>The exported data</returns>
        [Authorize]
        [HttpGet]
        public IEnumerable<Model.Course> Export()
        {
            var name = this.User.Identity.Name;
            
            var courses = GetCoursesForUser(name);

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

        private IEnumerable<Model.Course> GetCoursesForUser(string email)
        {
            var coursesToReturn = new List<Model.Course>();
            var userOrganisationNctls = _context.McOrganisationUsers.Where(o => o.Email == email).Select(x => x.OrgId);
            var mappedUcasCodes = _context.ProviderMappers.Where(uo => userOrganisationNctls.Contains(uo.OrgId)).Select(x => x.UcasCode);
            var mappedCourses = _context.UcasCourses.Where(c => mappedUcasCodes.Contains(c.InstCode));

            foreach (var instCode in mappedUcasCodes.Distinct().ToList())
            {
                var titles = mappedCourses.Where(c => c.InstCode == instCode).Select(x => x.CrseTitle).Distinct().ToList();
                var orgId = _context.ProviderMappers.FirstOrDefault(m => m.UcasCode == instCode)?.OrgId;

                foreach (var title in titles)
                {
                    var course = new Model.Course
                    {
                        UcasCode = instCode,
                        Title = title,
                        OrganisationName = _context.McOrganisations.FirstOrDefault(o => o.OrgId == orgId)?.Name
                    };

                    var accProviders = mappedCourses.Where(c => c.InstCode == instCode && c.CrseTitle == title).Select(x => x.AccreditingProvider).Distinct().ToList();

                    var variants = new List<Variant>();
                    foreach (var accProvider in accProviders)
                    {
                        var variant = new Variant { ProviderCode = accProvider };
                        var tempRecords = mappedCourses.Where(c => c.InstCode == instCode && c.CrseTitle == title && c.AccreditingProvider == accProvider).ToList();
                        var institution = _context.UcasInstitutions.FirstOrDefault(i => i.InstCode == accProvider);

                        if (institution != null)
                        {
                            variant.ProviderName = institution.InstFull;
                            variant.Address1 = institution.Addr1;
                            variant.Address2 = institution.Addr2;
                            variant.Address3 = institution.Addr3;
                            variant.Address4 = institution.Addr4;
                            variant.Postcode = institution.Postcode;
                        }
                        variant.CourseCode = tempRecords.FirstOrDefault()?.CrseCode;
                        var campusCodes = tempRecords.Select(c => c.CampusCode).Distinct().ToList();

                        variant.ProfpostFlags = ListDataValues(tempRecords.Select(x => x.ProfpostFlag).Distinct().ToList());
                        variant.ProgramTypes = ListDataValues(tempRecords.Select(x => x.ProgramType).Distinct().ToList());
                        variant.StudyModes = ListDataValues(tempRecords.Select(x => x.Studymode).Distinct().ToList());
                        //add campuses
                        variant.Campuses = new List<Campus>();
                        foreach (var campusCode in campusCodes)
                        {
                            var campus = _context.UcasCampuses.FirstOrDefault(c => c.InstCode == instCode && c.CampusCode == campusCode);
                            if (campus != null)
                            {
                                variant.Campuses.Add(new Campus
                                {
                                    Name = campus.CampusName,
                                    Address1 = campus.Addr1,
                                    Address2 = campus.Addr2,
                                    Address3 = campus.Addr3,
                                    Address4 = campus.Addr4,
                                    PostCode = campus.Postcode
                                });
                            }
                        }

                        variants.Add(variant);
                    }

                    course.Variants = variants;

                    coursesToReturn.Add(course);
                }
            }

            return coursesToReturn;
        }
        /// <summary>
        /// //returns a list of data from the list of keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private List<string> ListDataValues(List<string> keys)
        {
            var returnList = new List<string>();

            foreach (var key in keys)
            {
                returnList.Add(DataMapper.GetStringData(key));
            }

            return returnList;
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
                        OrgId = organisation.OrgId,
                        Name = organisation.Name
                    }
                    );
            }
            foreach (var organisatioInstitution in payload.OrganisationInstitutions)
            {
                _context.AddMcOrganisationInstitution(
                    new McOrganisationInstitution
                    {
                        OrgId = organisatioInstitution.OrgId,
                        InstitutionCode = organisatioInstitution.InstitutionCode
                    }
                );
            }
            foreach (var organisationUser in payload.OrganisationUsers)
            {
                _context.AddMcOrganisationUser(
                    new McOrganisationUser
                    {
                        OrgId = organisationUser.OrgId,
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
                        OrgId = mapper.OrgId,
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

