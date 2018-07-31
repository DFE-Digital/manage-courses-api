using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    public partial class DfeSignInCommunicator
    {
        
        private readonly string dfeSignInDomain;
        private readonly string redirectUriHostAndPort;
        private readonly string clientId;
        private readonly string clientSecret;

        private readonly SimpleHttpBrowser httpBrowser = new SimpleHttpBrowser();

        public DfeSignInCommunicator(string dfeSignInDomain, string redirectUriHostAndPort, string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(dfeSignInDomain))
            {
                throw new ArgumentException("required", nameof(dfeSignInDomain));
            }

            if (string.IsNullOrEmpty(redirectUriHostAndPort))
            {
                throw new ArgumentException("required", nameof(redirectUriHostAndPort));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("required", nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("required", nameof(clientSecret));
            }

            this.dfeSignInDomain = dfeSignInDomain;
            this.redirectUriHostAndPort = redirectUriHostAndPort;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        public async Task<string> GetAccessTokenAsync(string username, string password) 
        {
            var startUrl = $"https://{dfeSignInDomain}/auth?redirect_uri=https://{redirectUriHostAndPort}/auth/cb&scope=openid profile email&response_type=code&state=1238&client_id={clientId}";
            var loginPage = await GetUrl(startUrl, true);

            var form1 = await PostForm(loginPage, new Dictionary<string, string>{
                    {"username", username},
                    {"password", password},
                }, true);

            var form2 = await PostForm(form1, new Dictionary<string, string>(), false);
            var cb = await GetUrl(form2.Url, false);

            var authCode = new Regex(@"[?&]code=([^&]+)").Match(cb.Url).Groups[1].Value;

            var acHttpClient = await new HttpClient().PostAsync($"https://{dfeSignInDomain}/token", new FormUrlEncodedContent(new Dictionary<string, string>() {
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"redirect_uri", $"https://{redirectUriHostAndPort}/auth/cb"},
                    {"grant_type", "authorization_code"},
                    {"code",  authCode}
                }));


            var json = await acHttpClient.Content.ReadAsStringAsync();
            
            try 
            {
                string accessToken = JObject.Parse(json)["access_token"].Value<string>();
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception($"could not get access_token with settings: {clientId}, {username}, {clientSecret.Substring(0,3)}, {password.Substring(0,3)}");
                }
                return accessToken;
            }
            catch (JsonReaderException)
            {
                throw new Exception($"could not get access_token with settings: {clientId}, {username}, {clientSecret.Substring(0,3)}, {password.Substring(0,3)}");
            }
        }

        private async Task<NavigationResult> GetUrl(string url, bool shouldRedirect)
        {
            var res = await httpBrowser.GetAsync(url);
            return await Navigate(url, shouldRedirect, res);
        }
        
        private async Task<NavigationResult> PostForm(NavigationResult navResult, IDictionary<string,string> body, bool shouldRedirect)
        {
            var res = await httpBrowser.PostFormAsync(navResult.Url, navResult.Content, body);
            
            return await Navigate(res.RequestMessage.RequestUri.AbsoluteUri, shouldRedirect, res);
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
                    return await GetUrl(newLocation, shouldRedirect);
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
        
        private class NavigationResult 
        {
            public string Url {get;set;}
            public string Content {get;set;}
        }
    }
}