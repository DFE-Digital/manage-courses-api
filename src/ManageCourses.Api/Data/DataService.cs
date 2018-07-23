using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class DataService : IDataService
    {
        private readonly IManageCoursesDbContext _context;

        public DataService(IManageCoursesDbContext context)
        {
            _context = context;
        }

        #region Import
        /// <summary>
        /// Processes data in the payload object into the database as an upsert/delta
        /// </summary>
        /// <param name="payload"></param>
        public void ProcessPayload(Payload payload)
        {
            //ResetDatabase();

            var uniqueCourseCodes = payload.Courses.Select(course => new CourseCode
            {
                InstCode = course.InstCode,
                CrseCode = course.CrseCode
            }).Distinct(new CourseCodeComparer());

            var userDatahelper = new UserDataHelper(_context, payload.Users.Where(u => ! string.IsNullOrWhiteSpace(u.Email)).ToList());
            if (! userDatahelper.Upsert())
            {
                //do something
            }

            #region Old Code

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
  
            _context.Save();

            #endregion

        }
        private void ResetDatabase()
        {
            // clear out the existing data
            _context.UcasCourses.RemoveRange(_context.UcasCourses);
            _context.CourseCodes.RemoveRange(_context.CourseCodes);
            _context.UcasInstitutions.RemoveRange(_context.UcasInstitutions);
            _context.UcasSubjects.RemoveRange(_context.UcasSubjects);
            _context.UcasCourseSubjects.RemoveRange(_context.UcasCourseSubjects);
            _context.UcasCampuses.RemoveRange(_context.UcasCampuses);
            _context.UcasCourseNotes.RemoveRange(_context.UcasCourseNotes);
            _context.UcasNoteTexts.RemoveRange(_context.UcasNoteTexts);
            _context.McOrganisations.RemoveRange(_context.McOrganisations);
            _context.McOrganisationIntitutions.RemoveRange(_context.McOrganisationIntitutions);
            _context.McOrganisationUsers.RemoveRange(_context.McOrganisationUsers);
            //_context.McUsers.RemoveRange(_context.McUsers);
            _context.Save();
        }

        /*
        /// <summary>
        /// Returns a list of course code that DO NOT exist in the database
        /// </summary>
        /// <param name="courseCodes"></param>
        /// <returns>list of new course codes to add</returns>
        private ImportMapper GetDataMapper(IReadOnlyCollection<CourseCode> courseCodes)
        {
            var mapperToReturn = new ImportMapper();
            var additions = new List<CourseCode>();
            var updates = new List<CourseCode>();
            var deletes = new List<CourseCode>();

            foreach (var courseCode in courseCodes)
            {
                var dbRecord = GetDbRecord(courseCode);
                if (dbRecord == null)
                {
                    additions.Add(courseCode);
                }
                else
                {
                    //dont worry about updates for course codes as there are only 2 fields
                }
            }

            foreach (var dbRecord in _context.CourseCodes)
            {
                if (! courseCodes.Any(c => c.CrseCode == dbRecord.CrseCode && c.InstCode == dbRecord.InstCode))
                {
                    deletes.Add(dbRecord);
                }
            }

            mapperToReturn.Additions = additions;
            mapperToReturn.Updates = updates;
            mapperToReturn.Deletes = deletes;

            return mapperToReturn;
        }
        /// <summary>
        /// Returns an ImportMapper class thats holds list for all additions/updates and deletions
        /// </summary>
        /// <param name="courses"></param>
        /// <returns>list of new course codes to add</returns>
        private ImportMapper GetDataMapper(IReadOnlyCollection<UcasCourse> courses)
        {
            var mapperToReturn = new ImportMapper();
            var additions = new List<UcasCourse>();
            var updates = new List<UcasCourse>();

            foreach (var course in courses)
            {
                var dbRecord = GetDbRecord(course);
                if (dbRecord == null)
                {
                    additions.Add(course);
                }
                else
                {
                    if (IsUpdated(dbRecord, course))
                    {
                        updates.Add(course);
                    }
                }
            }

            var deletes = _context.UcasCourses.Where(dbRecord => !courses.Any(c => c.CrseCode == dbRecord.CrseCode && c.InstCode == dbRecord.InstCode && dbRecord.CampusCode == c.CampusCode)).ToList();

            mapperToReturn.Additions = additions;
            mapperToReturn.Updates = updates;
            mapperToReturn.Deletes = deletes;

            return mapperToReturn;
        }
        /// <summary>
        /// Returns an ImportMapper class thats holds list for all additions/updates and deletions
        /// </summary>
        /// <param name="institutions"></param>
        /// <returns>list of new course codes to add</returns>
        private ImportMapper GetDataMapper(IReadOnlyCollection<UcasInstitution> institutions)
        {
            var mapperToReturn = new ImportMapper();
            var additions = new List<UcasInstitution>();
            var updates = new List<UcasInstitution>();

            foreach (var institution in institutions)
            {
                var dbRecord = GetDbRecord(institution);
                if (dbRecord == null)
                {
                    additions.Add(institution);
                }
                else
                {
                    if (IsUpdated(dbRecord, institution))
                    {
                        updates.Add(institution);
                    }
                }
            }
           
            var deletes = _context.UcasInstitutions.Where(dbRecord => institutions.All(x => x.InstCode != dbRecord.InstCode)).ToList();

            mapperToReturn.Additions = additions;
            mapperToReturn.Updates = updates;
            mapperToReturn.Deletes = deletes;

            return mapperToReturn;
        }
        /// <summary>
        /// Returns an ImportMapper class thats holds list for all additions/updates and deletions
        /// </summary>
        /// <param name="subjects"></param>
        /// <returns>list of new course codes to add</returns>
        private ImportMapper GetDataMapper(IReadOnlyCollection<UcasSubject> subjects)
        {
            var mapperToReturn = new ImportMapper();
            var additions = new List<UcasSubject>();
            var updates = new List<UcasSubject>();

            foreach (var subject in subjects)
            {
                var dbRecord = GetDbRecord(subject);
                if (dbRecord == null)
                {
                    additions.Add(subject);
                }
                else
                {
                    if (IsUpdated(dbRecord, subject))
                    {
                        updates.Add(subject);
                    }
                }
            }

            var deletes = _context.UcasSubjects.Where(dbRecord => !subjects.Any(c => c.SubjectCode == dbRecord.SubjectCode)).ToList();

            mapperToReturn.Additions = additions;
            mapperToReturn.Updates = updates;
            mapperToReturn.Deletes = deletes;

            return mapperToReturn;
        }

        private bool IsUpdated(UcasCourse dbRecord, UcasCourse importRecord)
        {
            bool returnBool;
            returnBool = (dbRecord.CrseCode != importRecord.CrseCode ||
                          dbRecord.InstCode != importRecord.InstCode ||
                          dbRecord.CampusCode != importRecord.CampusCode ||
                          dbRecord.AccreditingProvider != importRecord.AccreditingProvider ||
                          dbRecord.Age != importRecord.Age ||
                          dbRecord.CrseOpenDate != importRecord.CrseOpenDate ||
                          dbRecord.CrseTitle != importRecord.CrseTitle ||
                          dbRecord.ProfpostFlag != importRecord.ProfpostFlag ||
                          dbRecord.ProgramType != importRecord.ProgramType ||
                          dbRecord.Studymode != importRecord.Studymode);

            return returnBool;
        }
        private bool IsUpdated(UcasInstitution dbRecord, UcasInstitution importRecord)
        {
            bool returnBool;
            returnBool = (dbRecord.AccreditingProvider != importRecord.AccreditingProvider ||
                          dbRecord.ContactName != importRecord.ContactName ||
                          dbRecord.InstFull != importRecord.InstFull ||
                          dbRecord.InstName != importRecord.InstName ||
                          dbRecord.InstBig != importRecord.InstBig ||
                          dbRecord.InstType != importRecord.InstType ||
                          dbRecord.YearCode != importRecord.YearCode ||
                          dbRecord.Scitt != importRecord.Scitt ||
                          dbRecord.SchemeMember != importRecord.SchemeMember ||
                          dbRecord.Addr1 != importRecord.Addr1 ||
                          dbRecord.Addr2 != importRecord.Addr2 ||
                          dbRecord.Addr3 != importRecord.Addr3 ||
                          dbRecord.Addr4 != importRecord.Addr4 ||
                          dbRecord.Postcode != importRecord.Postcode);                          

            return returnBool;
        }
        private bool IsUpdated(UcasSubject dbRecord, UcasSubject importRecord)
        {
            bool returnBool;
            returnBool = (dbRecord.SubjectDescription != importRecord.SubjectDescription ||
                          dbRecord.TitleMatch != importRecord.TitleMatch);

            return returnBool;
        }
        private UcasCourse GetDbRecord(UcasCourse course)
        {
            return _context.UcasCourses.FirstOrDefault(x =>
                x.CrseCode == course.CrseCode && x.InstCode == course.InstCode && x.CampusCode == course.CampusCode);          
        }
        private CourseCode GetDbRecord(CourseCode courseCode)
        {
            return _context.CourseCodes.FirstOrDefault(x =>
                x.InstCode == courseCode.InstCode && x.CrseCode == courseCode.CrseCode);
        }
        private UcasInstitution GetDbRecord(UcasInstitution institution)
        {
            return _context.UcasInstitutions.FirstOrDefault(x => x.InstCode == institution.InstCode);
        }
        private UcasSubject GetDbRecord(UcasSubject subject)
        {
            return _context.UcasSubjects.FirstOrDefault(x =>
                x.SubjectCode == subject.SubjectCode);
        }
*/
        public class CourseCodeComparer : IEqualityComparer<CourseCode>
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
        public class CourseComparer : IEqualityComparer<UcasCourse>
        {
            public bool Equals(UcasCourse x, UcasCourse y)
            {
                return x.InstCode == y.InstCode && x.CrseCode == y.CrseCode;
            }

            public int GetHashCode(UcasCourse cc)
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

            var mcOrganisationInstitution = ucasInstitution.SingleOrDefault(oi => oi.UcasInstitution.InstCode == ucasCode);

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
            var userOrgs = _context.McUsers.ByEmail(email)
                .Join(_context.McOrganisationUsers, u => u.Email, ou => ou.Email,
                    (user, organisationUser) => organisationUser)
                .Join(_context.McOrganisationIntitutions, oi => oi.OrgId, i => i.OrgId,
                    (user, institution) => new
                    {
                        institution.OrgId,
                        institution.InstitutionCode,
                        institution.UcasInstitution.InstFull
                    })
                .Select(arg => new Organisation { Name = arg.InstFull, OrgId = arg.OrgId, UcasCode = arg.InstitutionCode }).ToList();

            var userOrganisations = userOrgs?.Select(organisation => new UserOrganisation
            {
                OrganisationId = organisation.OrgId,
                OrganisationName = organisation.Name,
                UcasCode = organisation.UcasCode,
                TotalCourses = GetProviderCourses(organisation).SelectMany(x => x.CourseDetails).SelectMany(v => v.Variants).Count()
            });
            return userOrganisations;
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
