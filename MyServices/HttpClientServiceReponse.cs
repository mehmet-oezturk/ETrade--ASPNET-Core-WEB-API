using System.Net;

namespace MyServices
{
    public class HttpClientServiceReponse<T>//ona verilen t tipine göre içinde property barındıracak 
    {
        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }

    }
}
