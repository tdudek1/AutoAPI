using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutoAPI.IntegrationTests
{
    public static class Helper
    {
        public static async Task<(T Object, HttpStatusCode StatusCode)> Json<T>(HttpRequestMessage message)
        {
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var client = new HttpClient();

            var response = await client.SendAsync(message);

            var results = await response.Content.ReadAsAsync<T>();

            return (results, response.StatusCode);

        }

        public static async Task<HttpResponseMessage> Response(HttpRequestMessage message)
        {
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var client = new HttpClient();

            var response = await client.SendAsync(message);

            return response;

        }

        public static async Task<HttpResponseMessage> Response(HttpRequestMessage message, object content)
        {
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.SendAsync(message);

            return response;

        }



        public static Task<(T Object, HttpStatusCode StatusCode)> Json<T>(HttpRequestMessage message, object content)
        {
            message.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return Json<T>(message);
        }
    }
}
