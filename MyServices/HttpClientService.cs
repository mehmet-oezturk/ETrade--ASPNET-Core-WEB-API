
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyServices
{
    public class HttpClientService
    {
        public string Domain { get; set; }

        public HttpClientService (string domain)
        {
            Domain = domain;
        }

        public HttpClientServiceReponse<TResponse> Post<TRequest,TResponse>(string fragment,TRequest data,string token="")
        {
            //TÜKETME
            HttpClientServiceReponse<TResponse> result = new HttpClientServiceReponse<TResponse>();
            HttpClient client = new HttpClient();

            if(string.IsNullOrEmpty(token)==false)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            }
 

            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync($"{Domain}{fragment}", content).Result;

            result.StatusCode = response.StatusCode;
            result.ResponseContent = response.Content.ReadAsStringAsync().Result;

            if(response.StatusCode==HttpStatusCode.OK)
            {
                result.Data = JsonSerializer.Deserialize<TResponse>(result.ResponseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return result;
        }
    }
}
