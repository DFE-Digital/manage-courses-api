using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Serilog;

namespace GovUk.Education.ManageCourses.UcasCourseImporter
{
    internal class UcasZipDownloader
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly string _blobContainerUrl;
        private readonly string _sharedAccessSignatureQueryString;

        public UcasZipDownloader(ILogger logger, string blobContainerUrl, string sharedAccessSignature)
        {
            _client = new HttpClient();
            _logger = logger;
            _blobContainerUrl = blobContainerUrl;
            _sharedAccessSignatureQueryString = sharedAccessSignature;
        }

        public async Task<string> DownloadLatestToFolder(string folder, string prefix)
        {

            // list files
            var listUrl = $"{_blobContainerUrl}?restype=container&comp=list&{_sharedAccessSignatureQueryString}";
            _logger.Debug($"Getting {listUrl}");
            var listResponse = await _client.GetAsync(listUrl);
            listResponse.EnsureSuccessStatusCode();
            var list = XElement.Parse(await listResponse.Content.ReadAsStringAsync());

            var filenames = new List<AzureFile>();
            var fileNameRegexString = "^" + prefix + "_([0-9]{2})([0-9]{2})([0-9]{4})_([0-9]{2})([0-9]{2})\\.zip$";
            var fileNameRegex = new Regex(fileNameRegexString);
            
            foreach (var blobElement in list.Element("Blobs").Elements())
            {
                string name = blobElement.Element("Name").Value;
                var match = fileNameRegex.Match(name);
                if (match.Success) 
                {
                    var timestamp = new DateTime(
                        int.Parse(match.Groups[3].Value),
                        int.Parse(match.Groups[2].Value),
                        int.Parse(match.Groups[1].Value),
                        int.Parse(match.Groups[4].Value),
                        int.Parse(match.Groups[5].Value),
                        0);
                    
                    filenames.Add(new AzureFile(name, timestamp));
                }
            }

            // determine best file
            var matchingFiles = filenames.Where(x => fileNameRegex.IsMatch(x.Name)).ToList();
            _logger.Information($"Found {matchingFiles.Count()} blobs matching '{fileNameRegex}' in azure blob storage");
            var bestFileName = matchingFiles.OrderByDescending(x => x.Timestamp).FirstOrDefault();

            if (bestFileName == null)
            {
                throw new Exception($"Couldn't find any files matching regex '{fileNameRegexString}'");
            }

            // download best file
            string fileToWriteTo = Path.Combine(folder, $"{prefix}.zip");

            _logger.Information($"Downloading {bestFileName} to {fileToWriteTo}");

            using (var response = await _client.GetAsync($"{_blobContainerUrl}/{bestFileName.Name}?{_sharedAccessSignatureQueryString}"))
            using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
            using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
            {
                await streamToReadFrom.CopyToAsync(streamToWriteTo);
            }
            _logger.Verbose($"Downloading of {bestFileName} to {fileToWriteTo} complete");
            return fileToWriteTo;
        }

        private class AzureFile
        {
            public readonly string Name;
            public readonly DateTime Timestamp;

            public AzureFile(string name, DateTime timestamp)
            {
                Name = name;
                Timestamp = timestamp;
            }

            public override string ToString()
            {
                return $"Azure file {Name} with timestamp {Timestamp}";
            }
        }
    }
}