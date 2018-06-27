using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

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

        public OrganisationCourses GetCoursesForUser(string email)
        {
            var returnCourses = new OrganisationCourses();
            var userOrganisationNctls = _context.McOrganisationUsers.Where(o => o.Email == email).Select(x => x.OrgId);
            var mappedUcasCodes = _context.ProviderMappers.Where(uo => userOrganisationNctls.Contains(uo.OrgId)).Select(x => x.UcasCode);
            var mappedCourses = _context.UcasCourses.Where(c => mappedUcasCodes.Contains(c.InstCode));

            foreach (var instCode in mappedUcasCodes.Distinct().ToList())//should only be one atm
            {
                var org = _context.ProviderMappers.SingleOrDefault(m => m.UcasCode == instCode);

                if (org != null)
                {
                    returnCourses.OrganisationId = org.OrgId;
                    returnCourses.OrganisationName = org.InstitutionName;
                    returnCourses.ProviderCourses = new List<ProviderCourse>();
                    var titles = mappedCourses.Where(c => c.InstCode == instCode).Select(x => x.CrseTitle).Distinct().ToList();
                    foreach (var title in titles)
                    {
                        //if (title != "Biology")//test code
                        //{
                        //    continue;
                        //}
                        ////end test code

                        var accProviders = mappedCourses.Where(c => c.InstCode == instCode && c.CrseTitle == title).Select(x => x.AccreditingProvider).Distinct().ToList();
                        foreach (var accProvider in accProviders)
                        {
                            var tempRecords = mappedCourses.Where(c => c.InstCode == instCode && c.CrseTitle == title && c.AccreditingProvider == accProvider).ToList();
                            var courseCodes = tempRecords.Select(x => x.CrseCode).Distinct().ToList();

                            var accProviderName = _context.ProviderMappers.FirstOrDefault(m => m.UcasCode == accProvider)?.InstitutionName;
                            var course = new ProviderCourse
                            {
                                AccreditingProviderId = accProvider,
                                AccreditingProviderName = accProviderName,
                                CourseDetails = new List<CourseDetail>()
                            };
                            var courseDetail = new CourseDetail
                            {
                                CourseTitle = title,
                                Variants = new List<CourseVariant>()
                            };
                            foreach (var courseCode in courseCodes)
                            {
                                var variant = new CourseVariant
                                {    
                                    Name = title,
                                    UcasCode = courseCode,
                                    ProfPostFlag = DataMapper.GetStringData(tempRecords.FirstOrDefault(r => r.CrseCode == courseCode)?.ProfpostFlag),
                                    ProgramType = DataMapper.GetStringData(tempRecords.FirstOrDefault(r => r.CrseCode == courseCode)?.ProgramType),
                                    StudyMode = DataMapper.GetStringData(tempRecords.FirstOrDefault(r => r.CrseCode == courseCode)?.Studymode),
                                    Campuses = new List<Campus>()
                                };
                                var campusCodes = tempRecords.Where(c => c.CrseCode == courseCode).Select(c => c.CampusCode).Distinct().ToList();
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
                                            PostCode = campus.Postcode,
                                            Code = campusCode,
                                            CourseOpenDate = tempRecords.FirstOrDefault(c => c.CrseCode == courseCode && c.CampusCode == campusCode)?.CrseOpenDate
                                        });
                                    }
                                }

                                courseDetail.Variants.Add(variant);
                            }
                            course.CourseDetails.Add(courseDetail);
                            returnCourses.ProviderCourses.Add(course);
                        }
                    }
                }
            }

            return returnCourses;
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
      
        #endregion
    }
}
