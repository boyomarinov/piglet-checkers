namespace PigletCheckers.Server.Models
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class ErrorResponseException : HttpResponseException
    {
        public ErrorResponseException(HttpResponseMessage msg) : base(msg)
        {
        }

        public ErrorResponseException(HttpStatusCode statusCode) : base(statusCode)
        {
        }
    }
}