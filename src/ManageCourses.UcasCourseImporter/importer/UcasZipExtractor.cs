using System.IO.Compression;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    public class UcasZipExtractor
    {
        public void Extract(string zipPath, string extractPath) {
            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }
    }
}