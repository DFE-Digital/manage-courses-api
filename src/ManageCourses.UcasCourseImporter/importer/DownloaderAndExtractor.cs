using System.IO;
using Serilog.Core;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    internal class DownloaderAndExtractor
    {
        private readonly Logger logger;
        private readonly string workingDirectory;
        private readonly UcasZipDownloader ucasZipDownloader;
        private readonly UcasZipExtractor extractor;

        public DownloaderAndExtractor(Logger logger, string workingDirectory, string azureUrl, string azureSignature)
        {
            this.logger = logger;
            this.workingDirectory = workingDirectory;
            this.ucasZipDownloader = new UcasZipDownloader(logger, azureUrl, azureSignature);
            this.extractor = new UcasZipExtractor();
        }

        public string DownloadAndExtractLatest(string prefix)
        {
            var zipFile = ucasZipDownloader.DownloadLatestToFolder(workingDirectory, prefix).Result;
            var unzipFolder = Path.Combine(workingDirectory, prefix);
            logger.Information($"Unzipping {zipFile} to {unzipFolder}");
            extractor.Extract(zipFile, unzipFolder);
            return unzipFolder;
        }
    }
}