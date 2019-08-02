using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clients
{
    public class VideoClient
    {
        public async Task<string> SendVideoRequest(VideoToFrom videoToFrom, string Url)
        {
            string uri = Url + "/api/VideoRequest/send";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(videoToFrom), UnicodeEncoding.UTF8, "application/json")
                };
                responseMessage = await httpClient.SendAsync(request);
            }
            var unreadMessage = await responseMessage.Content.ReadAsStringAsync();
            return unreadMessage;
        }

        public async Task<string> ApproveVideoRequest(VideoToFrom videoToFrom, string Url)
        {
            string uri = Url + "/api/VideoRequest/approve";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(videoToFrom), UnicodeEncoding.UTF8, "application/json")
                };
                responseMessage = await httpClient.SendAsync(request);
            }
            var unreadMessage = await responseMessage.Content.ReadAsStringAsync();
            return unreadMessage;
        }
    }
}
