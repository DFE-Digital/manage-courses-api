namespace GovUk.Education.ManageCourses.Xls.Domain
{
    public class UcasCourse
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CrseCode { get; set; }
        public string CrseTitle { get; set; }
        public string Studymode { get; set; }
        public string Age { get; set; }
        public string CampusCode { get; set; }
        public string ProfpostFlag { get; set; }
        public string ProgramType { get; set; }
        public string AccreditingProvider { get; set; }
        public string CrseOpenDate { get; set; }
        public string Publish { get; set; }
        public string Status { get; set; }
        public string VacStatus { get; set; }
        public string StartYear { get; set; }
        public string StartMonth { get; set; }
        public string Modular { get; set; }
        public int? English { get; set; }
        public int? Maths { get; set; }
        public int? Science { get; set; }
    }
}
