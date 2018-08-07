using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class DataService : IDataService
    {
        private readonly IManageCoursesDbContext _context;
        private readonly IDataHelper _dataHelper;
        private readonly ILogger _logger;

        public DataService(IManageCoursesDbContext context, IDataHelper dataHelper, ILogger<DataService> logger)
        {
            _context = context;
            _dataHelper = dataHelper;
            _logger = logger;
        }

        #region Import
        /// <summary>
        /// Processes data in the payload object into the database as an upsert/delta
        /// </summary>
        /// <param name="payload">Holds all the data entities that need to be imported</param>
        public void ProcessUcasPayload(UcasPayload payload)
        {
            ResetUcasSchema();
            
            var uniqueCourses = payload.Courses.Select(course => new CourseCode
            {
                InstCode = course.InstCode,
                CrseCode = course.CrseCode
            }).Distinct(new CourseComparer());

            _context.CourseCodes.AddRange(uniqueCourses);

            foreach (var course in payload.Courses)
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
                    CrseOpenDate = course.CrseOpenDate
                });
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

            _context.Save();

        }

        public UserOrganisation GetOrganisationForUser(string email, string instCode)
        {
            var org = GetUserOrganisation(email, instCode);
            return org;
        }

        public void ProcessReferencePayload(ReferenceDataPayload payload)
        {
            ResetReferenceSchema();
            _dataHelper.Load(_context, payload.Users.Where(u => !string.IsNullOrWhiteSpace(u.Email)).ToList());

            var result = _dataHelper.Upsert();
            if (! result.Success)
            {
                _logger.LogCritical("Error during user upsert:{0} Import terminated", result.errorMessage);
                return;
            }
            _logger.LogInformation("User upsert result: {0} inserted, {1} deleted, {2} updated", result.NumberInserted, result.NumberDeleted, result.NumberUpdated);

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
            
  
            _context.Save();

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

            if (courseRecords.Count == 0)
            {
                return null;
            }
            var course = LoadCourse(courseRecords);
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
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(instCode)) { return returnCourses; }

            try
            {
                var courseRecords = _context.GetUcasCourseRecordsByInstCode(instCode, email);
                returnCourses = LoadCourses(courseRecords);
            }
            catch (Exception e)
            {
                //log the exception and return empty object
                _logger.LogCritical(e, "Error in GetCourses(): InstCode={0}", instCode);
            }
            return returnCourses;
        }
        private InstitutionCourses LoadCourses(IReadOnlyList<UcasCourse> courseRecords)
        {
            var returnCourses = new InstitutionCourses();
            if (courseRecords.Count > 0)
            {
                var organisationCourseRecord = courseRecords[0];//all the records in the list hold identical info so just get the first one
                returnCourses.InstitutionName = organisationCourseRecord.UcasInstitution.InstFull;
                returnCourses.InstitutionCode = organisationCourseRecord.InstCode;
                returnCourses.Courses = new List<Course>();
                foreach (var courseCode in courseRecords.Select(c => c.CrseCode).Distinct())
                {
                    var courseRecord = courseRecords.FirstOrDefault(c => c.CrseCode == courseCode);
                    if (courseRecord != null)
                    {
                        var course = new Course
                        {
                            Name = courseRecord.CrseTitle,
                            CourseCode = courseRecord.CrseCode,
                            ProfpostFlag = courseRecord.ProfpostFlag,
                            ProgramType = courseRecord.ProgramType,
                            StudyMode = courseRecord.Studymode
                        };
                        returnCourses.Courses.Add(course);
                    }
                }
            }

            return returnCourses;

        }
        private Course LoadCourse(IReadOnlyList<UcasCourse> courseRecords)
        {
            var returnCourse = new Course();
            if (courseRecords.Count > 0)
            {
                var organisationCourseRecord = courseRecords[0];//all the records in the list hold identical info so just get the first one
                returnCourse.InstCode = organisationCourseRecord.InstCode;
                returnCourse.CourseCode = organisationCourseRecord.CrseCode;
                returnCourse.AccreditingProviderId = organisationCourseRecord.AccreditingProvider;
                if (organisationCourseRecord.AccreditingProviderInstitution != null)
                {
                    returnCourse.AccreditingProviderName = organisationCourseRecord.AccreditingProviderInstitution.InstFull;
                }                    
                returnCourse.AgeRange = organisationCourseRecord.Age;
                returnCourse.Name = organisationCourseRecord.CrseTitle;
                returnCourse.ProgramType = organisationCourseRecord.ProgramType;
                returnCourse.ProfpostFlag = organisationCourseRecord.ProfpostFlag;
                returnCourse.StudyMode = organisationCourseRecord.Studymode;
                var subjects = _context.UcasSubjects
                    .Join(_context.UcasCourseSubjects, s => s.SubjectCode, cs => cs.SubjectCode,
                        (s, cs) => new { s.SubjectDescription, cs.CrseCode, cs.InstCode })
                    .Where(x => x.CrseCode == organisationCourseRecord.CrseCode && x.InstCode == organisationCourseRecord.InstCode)
                    .Select(x => x.SubjectDescription).ToList();

                returnCourse.Subjects = string.Join(", ", subjects);
            }
            var schools = courseRecords.Select(courseRecord => new School
            {
                LocationName = courseRecord.UcasCampus.CampusName,
                Address1 = courseRecord.UcasCampus.Addr1,
                Address2 = courseRecord.UcasCampus.Addr2,
                Address3 = courseRecord.UcasCampus.Addr3,
                Address4 = courseRecord.UcasCampus.Addr4,
                PostCode = courseRecord.UcasCampus.Postcode,
                ApplicationsAcceptedFrom = courseRecord.CrseOpenDate,
                Code = courseRecord.UcasCampus.CampusCode
            })
                .ToList();
            //look for the main site and move it to the top of the list
            var main = schools.FirstOrDefault(s => s.Code == "-");
            if (main != null)
            {
                schools.Remove(main);
                schools.Insert(0, main);
            }

            returnCourse.Schools = schools;

            return returnCourse;
        }
        private void ResetUcasSchema()
        {
            // clear out the existing data
            _context.UcasCourses.RemoveRange(_context.UcasCourses);
            _context.CourseCodes.RemoveRange(_context.CourseCodes);
            _context.UcasSubjects.RemoveRange(_context.UcasSubjects);
            _context.UcasCourseSubjects.RemoveRange(_context.UcasCourseSubjects);
            _context.UcasCampuses.RemoveRange(_context.UcasCampuses);
            _context.UcasCourseNotes.RemoveRange(_context.UcasCourseNotes);
            _context.UcasNoteTexts.RemoveRange(_context.UcasNoteTexts);
            _context.Save();
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

        #region Export

        /// <summary>
        /// This method return an object containing a list of course for an organisation mapped to an email
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <param name="ucasCode"></param>
        /// <returns>The user's organisation courses.</returns>
        public OrganisationCourses GetCoursesForUserOrganisation(string email, string ucasCode)
        {
            var org = GetOrganisation(email, ucasCode);

            var returnCourses = new OrganisationCourses();

            if (org == null) return returnCourses;

            returnCourses.OrganisationId = org.OrgId;
            returnCourses.UcasCode = org.UcasCode;
            returnCourses.OrganisationName = org.Name;

            var providersCourses = GetProviderCourses(org);
            returnCourses.ProviderCourses = providersCourses;

            return returnCourses;
        }
        public IEnumerable<UserOrganisation> GetOrganisationsForUser(string email)
        {
            var orgs = GetOrganisations(email);
            return orgs;
        }

        private List<ProviderCourse> GetProviderCourses(Organisation org)
        {
            var mappedCourses = _context.UcasCourses.Where(c => c.InstCode == org.UcasCode);

            var accProviders = GetProviders(mappedCourses);

            if (accProviders.Count == 0)
            {
                var course = GetCourseDetail(mappedCourses, org.UcasCode);
                return new List<ProviderCourse>() { course };
            }
            else
            {
                var returnCoursesProviderCourses = accProviders
                    .Select(x => GetCourseDetail(mappedCourses, org.UcasCode, x.InstCode, x.InstFull)).ToList();
                return returnCoursesProviderCourses;
            }
        }

        #endregion
    
        /// <summary>
        /// Gets a specific organisation that is linked to the users email
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <param name="ucasCode"></param>
        /// <returns>The organisation of the user.</returns>
        private Organisation GetOrganisation(string email, string ucasCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(ucasCode)) return null;

            var mcOrganisationUsers = _context.McUsers.ByEmail(email)
                .Include("McOrganisationUsers.McOrganisation.McOrganisationInstitutions.UcasInstitution").ToList();

             var ucasInstitution = mcOrganisationUsers.SelectMany(ou =>
                ou.McOrganisationUsers.SelectMany(oi => oi.McOrganisation.McOrganisationInstitutions)).ToList();

            var mcOrganisationInstitution = ucasInstitution.SingleOrDefault(oi => ucasCode.Equals(oi.UcasInstitution.InstCode, StringComparison.InvariantCultureIgnoreCase));

            if (mcOrganisationInstitution == null) return null;

            return new Organisation
            {
                Name = mcOrganisationInstitution.UcasInstitution.InstFull,
                OrgId = mcOrganisationInstitution.OrgId,
                UcasCode = mcOrganisationInstitution.InstitutionCode
            };
        }
        private IEnumerable<UserOrganisation> GetOrganisations(string email)
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
        private UserOrganisation GetUserOrganisation(string email, string instCode)
        {
            var userOrganisation = _context.GetUserOrganisation(email, instCode);
            if (userOrganisation != null)
            {
                return new UserOrganisation()
                {
                    OrganisationId = userOrganisation.OrgId,
                    OrganisationName = userOrganisation.UcasInstitution.InstFull,
                    UcasCode = userOrganisation.InstitutionCode,
                    TotalCourses = userOrganisation.UcasInstitution.UcasCourses.Select(c => c.CrseCode).Distinct()
                        .Count()
                };
            }

            return null;
        }
        private List<UcasInstitution> GetProviders(IQueryable<UcasCourse> mappedCourses)
        {
            var accProviders = mappedCourses.Where(mc => (!string.IsNullOrWhiteSpace(mc.AccreditingProvider))).Select(mc => mc.AccreditingProvider).Distinct()
                .Select(accProvider => _context.UcasInstitutions.FirstOrDefault(o => o.InstCode == accProvider))
                .OrderBy(x => x.InstFull).ToList();

            return accProviders;
        }
        private ProviderCourse GetCourseDetail(IQueryable<UcasCourse> mappedCourses, string organisationCode, string providerCode = null, string providerName = null)
        {
            var course = new ProviderCourse
            {
                AccreditingProviderId = providerCode,
                AccreditingProviderName = providerName,
                CourseDetails = new List<CourseDetail>()
            };

            foreach (var title in GetTitles(mappedCourses, organisationCode, providerCode))
            {
                var tempRecords = mappedCourses.Where(c => c.InstCode == organisationCode && c.CrseTitle == title && c.AccreditingProvider == providerCode).ToList();
                var courseCodes = tempRecords.Select(x => x.CrseCode).Distinct().ToList();

                var courseDetail = new CourseDetail
                {
                    CourseTitle = title,
                    Variants = courseCodes.Select(c => GetVariant(tempRecords, title, c, organisationCode)).ToList(),
                    AgeRange = tempRecords.FirstOrDefault()?.Age
                };

                course.CourseDetails.Add(courseDetail);
            }

            return course;
        }

        private CourseVariant GetVariant(IReadOnlyCollection<UcasCourse> tempRecords, string title, string courseCode, string organisationCode)
        {
            var subjects = GetSubjects(courseCode, organisationCode);

            var currentCourse = tempRecords.FirstOrDefault(r => r.CrseCode == courseCode);
            var variant = new CourseVariant
            {
                Name = title,
                UcasCode = courseCode,
                ProfPostFlag = currentCourse?.ProfpostFlag,
                ProgramType = currentCourse?.ProgramType,
                StudyMode = currentCourse?.Studymode,
                Campuses = new List<Campus>(),
                Subjects = subjects
            };

            var campusCodes = tempRecords.Where(c => c.CrseCode == courseCode && !string.IsNullOrWhiteSpace(c.CrseCode) && (c.CampusCode != null)).OrderBy(x => x.CampusCode).Select(c => c.CampusCode.Trim()).Distinct().ToList();

            //look for dash and put add the top of the list
            if (campusCodes.Contains("-"))
            {
                campusCodes.Remove("-");
                campusCodes.Insert(0, "-");
            }

            //look for empty string and put add the top of the list
            if (campusCodes.Contains(""))
            {
                campusCodes.Remove("");
                campusCodes.Insert(0, "");
            }

            variant.Campuses = campusCodes.Select(x => GetCampus(x, organisationCode, tempRecords.FirstOrDefault(c => c.CrseCode == courseCode && c.CampusCode == x)?.CrseOpenDate)).ToList();

            return variant;
        }

        private Campus GetCampus(string campusCode, string organisationCode, string openDate)
        {
            var campus = _context.UcasCampuses.FirstOrDefault(c => c.InstCode == organisationCode && c.CampusCode == campusCode);
            if (campus != null)
            {
                return new Campus
                {
                    Name = campus.CampusName,
                    Address1 = campus.Addr1,
                    Address2 = campus.Addr2,
                    Address3 = campus.Addr3,
                    Address4 = campus.Addr4,
                    PostCode = campus.Postcode,
                    Code = campusCode,
                    CourseOpenDate = openDate
                };
            }

            return null;
        }
        private IEnumerable<string> GetTitles(IQueryable<UcasCourse> mappedCourses, string ucasCode, string instCode)
        {
            var titles = mappedCourses
                .Where(c => c.InstCode == ucasCode && c.AccreditingProvider == instCode)
                .OrderBy(x => x.CrseCode).Select(x => x.CrseTitle).Distinct().ToList();
            return titles;
        }

        private List<string> GetSubjects(string courseCode, string organisationCode)
        {
            var subjects = _context.UcasSubjects
                .Join(_context.UcasCourseSubjects, s => s.SubjectCode, cs => cs.SubjectCode,
                    (s, cs) => new { s.SubjectDescription, cs.CrseCode, cs.InstCode })
                .Where(x => x.CrseCode == courseCode && x.InstCode == organisationCode)
                .Select(x => x.SubjectDescription).ToList();
            return subjects;
        }
    }
}
