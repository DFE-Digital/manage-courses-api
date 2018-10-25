using System.Net;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageCoursesApiException : System.Exception
    {
        public HttpStatusCode? StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public ManageCoursesApiException(string message)
            : base(message)
        {
        }

        public ManageCoursesApiException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public ManageCoursesApiException(string message, HttpStatusCode? statusCode, string response)
            : base(message)
        {
            StatusCode = statusCode;
            Response = response;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }
}