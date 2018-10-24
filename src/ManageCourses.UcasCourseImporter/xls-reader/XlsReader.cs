using System.Collections.Generic;
using System.IO;
using System.Linq;
using GovUk.Education.ManageCourses.ApiClient;
using NPOI.HSSF.UserModel;
using Serilog;

namespace GovUk.Education.ManageCourses.Xls
{
    public class XlsReader
    {
        private readonly ILogger _logger;

        private const string InstFilename = "GTTR_INST.xls";
        private const string CrseFilename = "GTTR_CRSE.xls";
        private const string CrseSubjectFilename = "GTTR_CRSE_SUBJECT.xls";
        private const string SubjectFilename = "GTTR_SUBJECT.xls";
        private const string CampusFilename = "GTTR_CAMPUS.xls";
        private const string CrseNoteFilename = "GTTR_CRSENOTE.xls";
        private const string NoteTextFilename = "GTTR_NOTETEXT.xls";

        public XlsReader(ILogger logger)
        {
            _logger = logger;
        }

        public List<UcasCourse> ReadCourses(string folder, IList<UcasCampus> campuses)
        {
            var file = new FileInfo(Path.Combine(folder, CrseFilename));
            _logger.Information("Reading course xls file from: " + file.FullName);

            var courses = new List<UcasCourse>();
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var wb = new HSSFWorkbook(stream);
                var sheet = wb.GetSheetAt(0);
                var header = sheet.GetRow(0);

                var columnMap = header.Cells.ToDictionary(c => c.StringCellValue, c => c.ColumnIndex);
                for (int dataRowIndex = 1; dataRowIndex <= sheet.LastRowNum; dataRowIndex++)
                {
                    if (sheet.GetRow(dataRowIndex) == null)
                    {
                        continue;
                    }

                    var row = sheet.GetRow(dataRowIndex);
                    var accreditingProvider = row.GetCell(columnMap["ACCREDITING_PROVIDER"]).StringCellValue.Trim();
                    var ucasCourse = new UcasCourse
                    {
                        InstCode = row.GetCell(columnMap["INST_CODE"]).StringCellValue.Trim(),
                        CrseCode = row.GetCell(columnMap["CRSE_CODE"]).StringCellValue.Trim(),
                        CrseTitle = row.GetCell(columnMap["CRSE_TITLE"]).StringCellValue.Trim(),
                        Studymode = row.GetCell(columnMap["STUDYMODE"]).StringCellValue.Trim(),
                        Age = row.GetCell(columnMap["AGE"]).StringCellValue.Trim(),
                        CampusCode = row.GetCell(columnMap["CAMPUS_CODE"]).StringCellValue.Trim(),
                        ProfpostFlag = row.GetCell(columnMap["PROFPOST_FLAG"]).StringCellValue.Trim(),
                        ProgramType = row.GetCell(columnMap["PROGRAM_TYPE"]).StringCellValue.Trim(),
                        AccreditingProvider = accreditingProvider == "" ? null : accreditingProvider,
                        CrseOpenDate = row.GetCell(columnMap["CRSE_OPEN_DATE"]).StringCellValue.Trim(),
                        Publish = row.GetCell(columnMap["PUBLISH"]).StringCellValue.Trim(),
                        Status = row.GetCell(columnMap["STATUS"]).StringCellValue.Trim(),
                        VacStatus = row.GetCell(columnMap["VAC_STATUS"]).StringCellValue.Trim(),
                        HasBeenPublished = row.GetCell(columnMap["HAS_BEEN_PUBLISHED"]).StringCellValue.Trim(),
                        StartYear = row.GetCell(columnMap["YEAR_CODE"]).StringCellValue.Trim(),
                        StartMonth = row.GetCell(columnMap["CRSE_MONTH"]).StringCellValue.Trim()
                    };
                    if (!campuses.Any(c => c.InstCode == ucasCourse.InstCode && c.CampusCode == ucasCourse.CampusCode))
                    {
                        _logger.Warning($"Skipped invalid record in {CrseFilename} with crse_code {ucasCourse.CrseCode}. "
                                       +$"{CampusFilename} didn't contain a valid record with inst_code {ucasCourse.InstCode} and campus_code '{ucasCourse.CampusCode}'");
                        continue;
                    }
                    courses.Add(ucasCourse);
                }
            }
            _logger.Information(courses.Count + " courses loaded from xls");
            return courses;
        }
        public List<UcasInstitution> ReadInstitutions(string folder)
        {
            var file = new FileInfo(Path.Combine(folder, InstFilename));
            _logger.Information("Reading institution xls file from: " + file.FullName);

            var institutions = new List<UcasInstitution>();
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var wb = new HSSFWorkbook(stream);
                var sheet = wb.GetSheetAt(0);
                var header = sheet.GetRow(0);

                var columnMap = header.Cells.ToDictionary(c => c.StringCellValue, c => c.ColumnIndex);
                for (int dataRowIndex = 1; dataRowIndex <= sheet.LastRowNum; dataRowIndex++)
                {
                    if (sheet.GetRow(dataRowIndex) == null)
                    {
                        continue;
                    }

                    var row = sheet.GetRow(dataRowIndex);
                    institutions.Add(new UcasInstitution
                    {
                        InstCode = row.GetCell(columnMap["INST_CODE"]).StringCellValue.Trim(),
                        InstName = row.GetCell(columnMap["INST_NAME"]).StringCellValue.Trim(),
                        InstBig = row.GetCell(columnMap["INST_BIG"]).StringCellValue.Trim(),
                        InstFull = row.GetCell(columnMap["INST_FULL"]).StringCellValue.Trim(),
                        InstType = row.GetCell(columnMap["INST_TYPE"]).StringCellValue.Trim(),
                        Addr1 = row.GetCell(columnMap["ADDR_1"]).StringCellValue.Trim(),
                        Addr2 = row.GetCell(columnMap["ADDR_2"]).StringCellValue.Trim(),
                        Addr3 = row.GetCell(columnMap["ADDR_3"]).StringCellValue.Trim(),
                        Addr4 = row.GetCell(columnMap["ADDR_4"]).StringCellValue.Trim(),
                        Postcode = row.GetCell(columnMap["POSTCODE"]).StringCellValue.Trim(),
                        ContactName = row.GetCell(columnMap["CONTACT_NAME"]).StringCellValue.Trim(),
                        YearCode = row.GetCell(columnMap["YEAR_CODE"]).StringCellValue.Trim(),
                        Url = row.GetCell(columnMap["URL"]).StringCellValue.Trim(),
                        Scitt = row.GetCell(columnMap["SCITT"]).StringCellValue.Trim(),
                        AccreditingProvider = row.GetCell(columnMap["ACCREDITING_PROVIDER"]).StringCellValue.Trim(),
                        SchemeMember = row.GetCell(columnMap["SCHEME_MEMBER"]).StringCellValue.Trim()
                    }
                    );
                }
            }
            _logger.Information(institutions.Count + " intitutions loaded from xls");
            return institutions;
        }

        public List<UcasCourseSubject> ReadCourseSubjects(string folder, IList<UcasCourse> courses, IList<UcasSubject> subjects)
        {
            var file = new FileInfo(Path.Combine(folder, CrseSubjectFilename));
            _logger.Information("Reading course subject xls file from: " + file.FullName);

            var courseSubjects = new List<UcasCourseSubject>();
            int skipCount = 0;
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var wb = new HSSFWorkbook(stream);
                var sheet = wb.GetSheetAt(0);
                var header = sheet.GetRow(0);

                var columnMap = header.Cells.ToDictionary(c => c.StringCellValue, c => c.ColumnIndex);
                for (int dataRowIndex = 1; dataRowIndex <= sheet.LastRowNum; dataRowIndex++)
                {
                    if (sheet.GetRow(dataRowIndex) == null)
                    {
                        continue;
                    }

                    var row = sheet.GetRow(dataRowIndex);
                    var ucasCourseSubject = new UcasCourseSubject
                    {
                        InstCode = row.GetCell(columnMap["INST_CODE"]).StringCellValue.Trim(),
                        CrseCode = row.GetCell(columnMap["CRSE_CODE"]).StringCellValue.Trim(),
                        SubjectCode = row.GetCell(columnMap["SUBJECT_CODE"]).StringCellValue.Trim(),
                        YearCode = row.GetCell(columnMap["YEAR_CODE"]).StringCellValue.Trim()
                    };
                    var skipMessage = $"Skipped invalid record in {CrseSubjectFilename} with inst_code {ucasCourseSubject.InstCode} and crse_code {ucasCourseSubject.CrseCode}. ";
                    if (!courses.Any(c => c.InstCode == ucasCourseSubject.InstCode && c.CrseCode == ucasCourseSubject.CrseCode))
                    {
                        skipCount++;
                        _logger.Warning(skipMessage+$"No valid record with this inst_code and crse_code found in {CrseFilename}");
                        continue;
                    }
                    if (!subjects.Any(c => c.SubjectCode == ucasCourseSubject.SubjectCode))
                    {
                        skipCount++;
                        _logger.Warning(skipMessage+$"No valid record with this subject_code found in {SubjectFilename}");
                        continue;
                    }
                    courseSubjects.Add(ucasCourseSubject
                    );
                }
            }
            _logger.Information(courseSubjects.Count + " course-subjects loaded from xls");
            _logger.Warning($"{skipCount} course-subjects rows skipped due to integrity violations");
            return courseSubjects;
        }
        public List<UcasSubject> ReadSubjects(string folder)
        {
            var file = new FileInfo(Path.Combine(folder, SubjectFilename));
            _logger.Information("Reading subject xls file from: " + file.FullName);

            var subjects = new List<UcasSubject>();
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var wb = new HSSFWorkbook(stream);
                var sheet = wb.GetSheetAt(0);
                var header = sheet.GetRow(0);

                var columnMap = header.Cells.ToDictionary(c => c.StringCellValue, c => c.ColumnIndex);
                for (int dataRowIndex = 1; dataRowIndex <= sheet.LastRowNum; dataRowIndex++)
                {
                    if (sheet.GetRow(dataRowIndex) == null)
                    {
                        continue;
                    }

                    var row = sheet.GetRow(dataRowIndex);
                    subjects.Add(new UcasSubject
                    {
                        SubjectCode = row.GetCell(columnMap["SUBJECT_CODE"]).StringCellValue.Trim(),
                        SubjectDescription = row.GetCell(columnMap["SUBJECT_DESCRIPTION"]).StringCellValue.Trim(),
                        TitleMatch = row.GetCell(columnMap["TITLE_MATCH"]).StringCellValue.Trim(),
                    }
                    );
                }
            }
            _logger.Information(subjects.Count + " subjects loaded from xls");
            return subjects;
        }
        public List<UcasCampus> ReadCampuses(string folder, IList<UcasInstitution> institutions)
        {
            var file = new FileInfo(Path.Combine(folder, CampusFilename));
            _logger.Information("Reading campus xls file from: " + file.FullName);

            var campuses = new List<UcasCampus>();
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var wb = new HSSFWorkbook(stream);
                var sheet = wb.GetSheetAt(0);
                var header = sheet.GetRow(0);

                var columnMap = header.Cells.ToDictionary(c => c.StringCellValue, c => c.ColumnIndex);
                for (int dataRowIndex = 1; dataRowIndex <= sheet.LastRowNum; dataRowIndex++)
                {
                    if (sheet.GetRow(dataRowIndex) == null)
                    {
                        continue;
                    }

                    var row = sheet.GetRow(dataRowIndex);
                    var ucasCampus = new UcasCampus
                    {
                        InstCode = row.GetCell(columnMap["INST_CODE"]).StringCellValue.Trim(),
                        CampusCode = row.GetCell(columnMap["CAMPUS_CODE"]).StringCellValue.Trim(),
                        CampusName = row.GetCell(columnMap["CAMPUS_NAME"]).StringCellValue.Trim(),
                        Addr1 = row.GetCell(columnMap["ADDR_1"]).StringCellValue.Trim(),
                        Addr2 = row.GetCell(columnMap["ADDR_2"]).StringCellValue.Trim(),
                        Addr3 = row.GetCell(columnMap["ADDR_3"]).StringCellValue.Trim(),
                        Addr4 = row.GetCell(columnMap["ADDR_4"]).StringCellValue.Trim(),
                        Postcode = row.GetCell(columnMap["POSTCODE"]).StringCellValue.Trim(),
                        TelNo = row.GetCell(columnMap["TEL_NO"]).StringCellValue.Trim(),
                        Email = row.GetCell(columnMap["EMAIL"]).StringCellValue.Trim(),
                        RegionCode = row.GetCell(columnMap["REGION_CODE"]).StringCellValue.Trim(),
                    };
                    if (!institutions.Any(i => i.InstCode == ucasCampus.InstCode))
                    {
                        _logger.Warning($"Skipped invalid record in {CampusFilename} with inst_code {ucasCampus.InstCode} and campus_code '{ucasCampus.CampusCode}'. "
                                       +$"inst_code not found in {InstFilename}");
                        continue;
                    }
                    campuses.Add(ucasCampus);
                }
            }
            _logger.Information(campuses.Count + " campuses loaded from xls");
            return campuses;
        }
        public List<UcasCourseNote> ReadCourseNotes(string folder)
        {
            var file = new FileInfo(Path.Combine(folder, CrseNoteFilename));
            _logger.Information("Reading course note xls file from: " + file.FullName);

            var courseNotes = new List<UcasCourseNote>();
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var wb = new HSSFWorkbook(stream);
                var sheet = wb.GetSheetAt(0);
                var header = sheet.GetRow(0);

                var columnMap = header.Cells.ToDictionary(c => c.StringCellValue, c => c.ColumnIndex);
                for (int dataRowIndex = 1; dataRowIndex <= sheet.LastRowNum; dataRowIndex++)
                {
                    if (sheet.GetRow(dataRowIndex) == null)
                    {
                        continue;
                    }

                    var row = sheet.GetRow(dataRowIndex);
                    courseNotes.Add(new UcasCourseNote
                    {
                        InstCode = row.GetCell(columnMap["INST_CODE"]).StringCellValue.Trim(),
                        CrseCode = row.GetCell(columnMap["CRSE_CODE"]).StringCellValue.Trim(),
                        NoteNo = row.GetCell(columnMap["NOTE_NO"]).StringCellValue.Trim(),
                        NoteType = row.GetCell(columnMap["NOTE_TYPE"]).StringCellValue.Trim(),
                        YearCode = row.GetCell(columnMap["YEAR_CODE"]).StringCellValue.Trim()
                    }
                    );
                }
            }
            _logger.Information(courseNotes.Count + " course notes loaded from xls");
            return courseNotes;
        }
        public List<UcasNoteText> ReadNoteText(string folder)
        {
            var file = new FileInfo(Path.Combine(folder, NoteTextFilename));
            _logger.Information("Reading note text xls file from: " + file.FullName);

            var noteTexts = new List<UcasNoteText>();
            using (var stream = new FileStream(file.FullName, FileMode.Open))
            {
                var wb = new HSSFWorkbook(stream);
                var sheet = wb.GetSheetAt(0);
                var header = sheet.GetRow(0);

                var columnMap = header.Cells.ToDictionary(c => c.StringCellValue, c => c.ColumnIndex);
                for (int dataRowIndex = 1; dataRowIndex <= sheet.LastRowNum; dataRowIndex++)
                {
                    if (sheet.GetRow(dataRowIndex) == null)
                    {
                        continue;
                    }

                    var row = sheet.GetRow(dataRowIndex);
                    noteTexts.Add(new UcasNoteText
                    {
                        InstCode = row.GetCell(columnMap["INST_CODE"]).StringCellValue.Trim(),
                        NoteNo = row.GetCell(columnMap["NOTE_NO"]).StringCellValue.Trim(),
                        NoteType = row.GetCell(columnMap["NOTE_TYPE"]).StringCellValue.Trim(),
                        LineText = row.GetCell(columnMap["LINE_TEXT"]).StringCellValue.Trim(),
                        YearCode = row.GetCell(columnMap["YEAR_CODE"]).StringCellValue.Trim()
                    }
                    );
                }
            }
            _logger.Information(noteTexts.Count + " note texts loaded from xls");
            return noteTexts;
        }
    }
}
