using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    public class DfeSignInCommunicator
    {
        
        private readonly string dfeSignInDomain;
        private readonly string redirectUriHostAndPort;
        private readonly string clientId;
        private readonly string clientSecret;

        public DfeSignInCommunicator(string dfeSignInDomain, string redirectUriHostAndPort, string clientId, string clientSecret)
        {
            this.dfeSignInDomain = dfeSignInDomain;
            this.redirectUriHostAndPort = redirectUriHostAndPort;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }


        public async Task<string> GetAccessTokenAsync(string username, string password) 
        {
            var startUrl = $"https://{dfeSignInDomain}/auth?redirect_uri=https://{redirectUriHostAndPort}/auth/cb&scope=openid profile email&response_type=code&state=1238&client_id={clientId}";
            var loginPage = await GetRedirectUrl(startUrl);

            var form1 = await PostForm(loginPage, new Dictionary<string, string>{
                    {"username", username},
                    {"password", password},
                }, true);

            var form2 = await PostForm(form1, new Dictionary<string, string>(), false);
            var cb = await GetRedirectUrl(form2.Url, false);

            var authCode = new Regex(@"[?&]code=([^&]+)").Match(cb.Url).Groups[1].Value;

            var acHttpClient = await GetHttpClient().PostAsync($"https://{dfeSignInDomain}/token", new FormUrlEncodedContent(new Dictionary<string, string>() {
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"redirect_uri", $"https://{redirectUriHostAndPort}/auth/cb"},
                    {"grant_type", "authorization_code"},
                    {"code",  authCode}
                }));


            var json = await acHttpClient.Content.ReadAsStringAsync();
            
            string accessToken = JObject.Parse(json)["access_token"].Value<string>();
            return accessToken;
        }

         private class NavigationResult 
        {
            public string Url {get;set;}
            public string Content {get;set;}
        }
        private async Task<NavigationResult> GetRedirectUrl(string url, bool shouldRedirect = true)
        {

            var uri = new Uri(url);
            var res = await GetHttpClient(GetCookieHeader(url)).GetAsync(url);

            StoreCookies(uri, res.Headers);

            return await Navigate(url, shouldRedirect, res);

        }

        private async Task<NavigationResult> Navigate(string url, bool shouldRedirect, HttpResponseMessage res)
        {
            var isRedirect = res.Headers.Any(kvp => kvp.Key == "Location");
            if (isRedirect)
            {                
                var uri=new Uri(url);
                var location = res.Headers.Location.OriginalString;
                var newLocation = location.StartsWith("/")
                    ? $"{uri.Scheme}://{uri.Host}{location}"
                    : location;

                if (shouldRedirect)
                {
                    return await GetRedirectUrl(newLocation, shouldRedirect);
                }
                else
                {
                    return new NavigationResult
                    {
                        Url = newLocation
                    };
                }
            }
            else
            {

                var content = await res.Content.ReadAsStringAsync();
                return new NavigationResult
                {
                    Url = url,
                    Content = content
                };
            }
        }

        private void StoreCookies(Uri uri, HttpResponseHeaders headers)
        {
            if (!hostToCookie.ContainsKey(uri.Host)) hostToCookie[uri.Host] = new List<string>();

            if (headers.Any(kvp => kvp.Key == "Set-Cookie"))
            {
                hostToCookie[uri.Host] = hostToCookie[uri.Host]
                    .Concat(headers.GetValues("Set-Cookie").Select(x => x.Substring(0, x.IndexOf(';'))))
                    .Distinct().ToList();
            }
        }

        private string GetCookieHeader(string url)
        {
            var host = new Uri(url).Host;
            return hostToCookie.ContainsKey(host) ? String.Join("; ", hostToCookie[host]) : null;
        }

        private async Task<NavigationResult> PostForm(NavigationResult navResult, IDictionary<string,string> body, bool shouldRedirect = true)
        {
            var formBody = new Dictionary<string,string>();
            // get inputs 
            var html = navResult.Content;
            var regex = new Regex(@"<input [^>]*name=""([^""]+)\""[^>]+value=""([^""]+)""");
            var formActionRegex = new Regex(@"<form [^>]*action=""([^""]+)""");
            foreach (Match item in regex.Matches(html))
            {
                formBody[item.Groups[1].Value]= item.Groups[2].Value;                
            }

            foreach(KeyValuePair<string,string> pair in body)
            {
                formBody[pair.Key] = pair.Value;
            }

            var formActionMatch = formActionRegex.Match(html);
            var originalUri = new Uri(navResult.Url);
            var formAction = formActionMatch == null || String.IsNullOrEmpty(formActionMatch.Groups[1].Value)
                ? navResult.Url
                : formActionMatch.Groups[1].Value.StartsWith("http")
                ? formActionMatch.Groups[1].Value
                : $"{originalUri.Scheme}://{originalUri.Host}{formActionMatch.Groups[1].Value}";

            var res = await GetHttpClient(GetCookieHeader(formAction)).PostAsync(formAction, new FormUrlEncodedContent(formBody));
           
            StoreCookies(new Uri(formAction), res.Headers);

            return await Navigate(formAction, shouldRedirect, res);

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

        private Dictionary<string, List<string>> hostToCookie = new Dictionary<string, List<string>>();

    }

}