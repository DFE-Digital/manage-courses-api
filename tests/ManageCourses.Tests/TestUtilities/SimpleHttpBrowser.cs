using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    public partial class DfeSignInCommunicator
    {
        public class SimpleHttpBrowser
        {
            public async Task<HttpResponseMessage> GetAsync(string url)
            {
                var res =  await GetHttpClient(GetCookieHeader(url)).GetAsync(url);
                
                StoreCookies(new Uri(url), res.Headers);
                return res;
            }

            public async Task<HttpResponseMessage> PostFormAsync(string originalUrl, string html, IDictionary<string, string> body)
            {                
                var formBody = new Dictionary<string,string>();
                // get inputs 
                var regex = new Regex(@"<input [^>]*name=""([^""]+)\""[^>]+value=""([^""]+)""");
                var formActionRegex = new Regex(@"<form [^>]*action=""([^""]+)""");
                foreach (Match item in regex.Matches(html))
                {
                    formBody[item.Groups[1].Value]= item.Groups[2].Value;                
                }

                foreach(var pair in body)
                {
                    formBody[pair.Key] = pair.Value;
                }

                var formActionMatch = formActionRegex.Match(html);
                var originalUri = new Uri(originalUrl);
                var formAction = string.IsNullOrEmpty(formActionMatch.Groups[1].Value)
                    ? originalUrl
                    : formActionMatch.Groups[1].Value.StartsWith("http")
                    ? formActionMatch.Groups[1].Value
                    : $"{originalUri.Scheme}://{originalUri.Host}{formActionMatch.Groups[1].Value}";

                var res = await GetHttpClient(GetCookieHeader(formAction)).PostAsync(formAction, new FormUrlEncodedContent(formBody));
            
                StoreCookies(new Uri(formAction), res.Headers);

                return res;
            }

            private static HttpClient GetHttpClient(string cookie = null)
            {
                var res = new HttpClient(new HttpClientHandler {
                    AutomaticDecompression = DecompressionMethods.GZip,
                    AllowAutoRedirect = false
                });
                res.Timeout = TimeSpan.FromSeconds(10);
                res.DefaultRequestHeaders.Add("Connection", "keep-alive");
                res.DefaultRequestHeaders.Add("Pragma", "no-cache");
                res.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                res.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                res.DefaultRequestHeaders.Add("DNT", "1");
                res.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
                res.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                res.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                res.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

                if (cookie != null)
                {
                    res.DefaultRequestHeaders.Add("Cookie", cookie);
                }

                return res;
            }        
            private readonly Dictionary<string, List<string>> _hostToCookie = new Dictionary<string, List<string>>();

            private void StoreCookies(Uri uri, HttpResponseHeaders headers)
            {
                if (!_hostToCookie.ContainsKey(uri.Host)) _hostToCookie[uri.Host] = new List<string>();

                if (headers.Any(kvp => kvp.Key == "Set-Cookie"))
                {
                    _hostToCookie[uri.Host] = _hostToCookie[uri.Host]
                        .Concat(headers.GetValues("Set-Cookie").Select(x => x.Substring(0, x.IndexOf(';'))))
                        .Distinct().ToList();
                }
            }

            private string GetCookieHeader(string url)
            {
                var host = new Uri(url).Host;
                return _hostToCookie.ContainsKey(host) ? String.Join("; ", _hostToCookie[host]) : null;
            }       
        }
    }
}