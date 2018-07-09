using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
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
        public void ResetDatabase()
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
        public void ProcessPayload(Payload payload)
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
        #endregion

        #region Export
        /// <summary>
        /// This method return an object containing a list of course for an organisation mapped to an email
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <returns>The user's organisation courses.</returns>
        public OrganisationCourses GetCoursesForUser(string email)
        {
            var org = GetOrganisation(email);

            var returnCourses = new OrganisationCourses();

            if (org == null) return returnCourses;

            returnCourses.OrganisationId = org.OrgId;
            returnCourses.UcasCode = org.UcasCode;
            returnCourses.OrganisationName = org.Name;

            var providersCourses = GetProviderCourses(org);
            returnCourses.ProviderCourses = providersCourses;

            return returnCourses;
        }

        /// <summary>
        /// This method return an object containing a list of course for an organisation mapped to an email
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <param name="orgId"></param>
        /// <returns>The user's organisation courses.</returns>
        public OrganisationCourses GetCoursesForUserOrganisation(string email, string orgId)
        {
            var org = GetOrganisation(email, orgId);

            var returnCourses = new OrganisationCourses();

            if (org == null) return returnCourses;

            returnCourses.OrganisationId = org.OrgId;
            returnCourses.UcasCode = org.UcasCode;
            returnCourses.OrganisationName = org.Name;

            var providersCourses = GetProviderCourses(org);
            returnCourses.ProviderCourses = providersCourses;

            return returnCourses;
        }
        /// <summary>
        /// Gets a list of organisation the user in linked to
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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
        /// Gets a single (or the first) organisation that is linked to the users email
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <returns>The organisation of the user.</returns>
        private Organisation GetOrganisation(string email)
        {
            var mcOrgUser = _context.McOrganisationUsers
                .Include(x => x.McOrganisation)
                .ThenInclude(x => x.McOrganisationInstitutions)
                .SingleOrDefault(o =>
                 o.Email == email && o.McOrganisation != null && o.McOrganisation.McOrganisationInstitutions.FirstOrDefault() != null
                );

            if (mcOrgUser != null)
            {

                var ucaseInstitution = _context.UcasInstitutions.First(x => x.InstCode == mcOrgUser.McOrganisation.McOrganisationInstitutions
                    .First().InstitutionCode);

                return new Organisation
                {
                    Name = ucaseInstitution.InstFull,
                    OrgId = mcOrgUser.OrgId,
                    UcasCode = ucaseInstitution.InstCode
                };
            }

            return null;
        }

        /// <summary>
        /// Gets a specific organisation that is linked to the users email
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <param name="orgId"></param>
        /// <returns>The organisation of the user.</returns>
        private Organisation GetOrganisation(string email, string orgId)
        {
            var mcOrgUser = _context.McOrganisationUsers
                .Include(x => x.McOrganisation)
                .ThenInclude(x => x.McOrganisationInstitutions)
                .SingleOrDefault(o =>
                    o.Email == email && o.OrgId == orgId && o.McOrganisation != null && o.McOrganisation.McOrganisationInstitutions.FirstOrDefault() != null
                );

            if (mcOrgUser == null) return null;
            {
                var ucaseInstitution = _context.UcasInstitutions.First(x => x.InstCode == mcOrgUser.McOrganisation.McOrganisationInstitutions
                                                                                .First().InstitutionCode);

                return new Organisation
                {
                    Name = ucaseInstitution.InstFull,
                    OrgId = mcOrgUser.OrgId,
                    UcasCode = ucaseInstitution.InstCode
                };
            }
        }

        private IEnumerable<UserOrganisation> GetOrganisations(string email)
        {
            var mcOrgUsers = _context.McOrganisationUsers
                .Join(_context.McOrganisationIntitutions, ou => ou.OrgId, oi => oi.OrgId,
                    (ou, oi) => new {ou.Email, ou.OrgId, oi.UcasInstitution.InstCode, oi.UcasInstitution.InstFull})
                .Where(x => x.Email == email)
                .Select(o => new Organisation {Name = o.InstFull, OrgId = o.OrgId, UcasCode = o.InstCode}).ToList();

            var userOrganisations = mcOrgUsers.Select(x => new UserOrganisation
            {
                OrganisationId = x.OrgId,
                OrganisationName = x.Name,
                UcasCode = x.UcasCode,
                TotalCourses = GetProviderCourses(x).SelectMany(y => y.CourseDetails).Count()
            }).ToList();

            return userOrganisations;
        }

        private List<UcasInstitution> GetProviders(IQueryable<UcasCourse> mappedCourses)
        {
            var accProviders = mappedCourses.Where(mc => (!string.IsNullOrWhiteSpace(mc.AccreditingProvider))).Select(mc => mc.AccreditingProvider).Distinct()
                .Select(accProvider => _context.UcasInstitutions.FirstOrDefault(o => o.InstCode == accProvider))
                .OrderBy(x => x.InstFull).ToList();

            return accProviders;
        }
        private ProviderCourse GetCourseDetail(IQueryable<UcasCourse> mappedCourses, string organisationCode, string providerCode="", string providerName="")
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

            var campusCodes = tempRecords.Where(c => c.CrseCode == courseCode && !string.IsNullOrWhiteSpace(c.CrseCode) && !string.IsNullOrWhiteSpace(c.CampusCode)).OrderBy(x => x.CampusCode).Select(c => c.CampusCode.Trim()).Distinct().ToList();
         
            //look for dash and put add the top of the list
            if (campusCodes.Contains("-"))
            {
                campusCodes.Remove("-");
                campusCodes.Insert(0, "-");
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
