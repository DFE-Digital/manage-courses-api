using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    /// <summary>
    /// This class should eventually replace all course objects return from the Api.
    /// This should contain alll details required by the Ui
    /// </summary>
    public class Course
    {
        public int Id { get; set; }

        public int InstitutionId { get; set; }
        public Institution Institution {get; set;}
        public int? AccreditingInstitutionId { get; set; }
        public Institution AccreditingInstitution {get; set;}

        public string CourseCode { get; set; }
        public string ProgramType { get; set; }
        public string ProfpostFlag { get; set; }
        public string Name { get; set; }
        
        public string Subjects => string.Join(", ", CourseSubjects.Select(x => x.Subject.SubjectName));
        
        public string StudyMode { get; set; }
        public string AgeRange { get; set; }        
        public CourseQualification Qualification { get; set; }

        /// <summary>
        /// This describes the course in how it differs to other courses with similar titles and subjects.
        /// Information may include
        ///  - The qualification
        ///  - whether it's salaried or an apprenticeship
        ///  - whether it's full-time and/or part time
        /// </summary>
        public string TypeDescription => GetCourseVariantType();
        public string Route => GetRoute();

        public DateTime? StartDate { get; set; }
        
        public ICollection<CourseSite> CourseSites { get; set; }

        [NotMapped]
        public IEnumerable<Site> Sites { get => CourseSites.Select(x => x.Site); }
        [NotMapped]

        public EnumStatus? EnrichmentWorkflowStatus { get; set; }
        
        public ICollection<CourseSubject> CourseSubjects { get; set; }

        /// <summary>
        /// Aggregate of vacancy status for each campus.
        /// </summary>
        /// <value></value>
        [NotMapped]
        public bool HasVacancies { get => CourseSites.Any(s => s.VacStatus == "B" || s.VacStatus == "F" || s.VacStatus == "P");}


        private string GetCourseVariantType()
        {
            string result;
            
            switch(Qualification)
            {
                case CourseQualification.Qts:
                    result = "QTS";
                    break;
                case CourseQualification.QtsWithPgce:
                    result = "PGCE with QTS";
                    break;
                case CourseQualification.QtlsWithPgce:
                    result = "PGCE";
                    break;
                case CourseQualification.QtlsWithPgde:
                    result = "PGDE";
                    break;
                case CourseQualification.QtsWithPgde:
                    result = "PGDE with QTS";
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(Qualification)} is unknown value: {Qualification}");
            }

            if ((!string.IsNullOrWhiteSpace(result)) && string.Equals(StudyMode, "B", StringComparison.InvariantCultureIgnoreCase))
            {
                result += ", ";
            }
            else
            {
                result += " ";
            }

            result += GetStudyModeText(StudyMode);

            result += string.Equals(ProgramType, "ss", StringComparison.InvariantCultureIgnoreCase)
                ? " with salary"
                : "";

            result += string.Equals(ProgramType, "ta", StringComparison.InvariantCultureIgnoreCase)
                ? " teaching apprenticeship" 
                : "";
            
            return result;
        }

        private string GetRoute()
        {
            if (string.IsNullOrWhiteSpace(ProgramType))
            {
                return "";
            }

            var route = ProgramType.ToLowerInvariant();

            switch (route)
            {
                case "he":
                    return "Higher education programme";
                case "sd":
                    return "School Direct training programme";
                case "ss":
                    return "School Direct (salaried) training programme";
                case "sc":
                    return "SCITT programme";
                case "ta":
                    return "PG Teaching Apprenticeship";
                default:
                    return "";
            }
        }
        private static string GetStudyModeText(string studyMode)
        {
            var returnString = string.Empty;

            if (string.IsNullOrWhiteSpace(studyMode))
            {
                return returnString;//TODO clarify what happens if study mode is missing
            }

            if (studyMode.Equals("F", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "full time";
            }
            else if (studyMode.Equals("P", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "part time";
            }
            else if (studyMode.Equals("B", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "full time or part time";
            }

            return returnString;
        }
    }
}
