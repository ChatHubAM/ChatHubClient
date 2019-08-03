using ChatHubWPF.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubWPF
{
    public class MessageClient
    {
        private static string Token { get; set; }
        public MessageClient(string token)
        {
            Token = token;
        }
        public async Task<bool> SendMessage(Message message)
        {
            string uri = "http://192.168.0.10:5003/api/message";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(Token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(message), UnicodeEncoding.UTF8, "application/json")
                };
                responseMessage = await httpClient.SendAsync(request);
            }
            return true;
        }
        public async Task<IEnumerable<Message>> ReadAllMessages(MessageToFrom notif)
        {
            string uri = "http://192.168.0.10:5003/api/message/readallmessages";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(Token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(notif), UnicodeEncoding.UTF8, "application/json")
                };
                responseMessage = await httpClient.SendAsync(request);
            }
            var allMessages = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Message>>(allMessages);
        }

        public async Task MakeMessageRead(MessageToFrom notif)
        {
            string uri = "http://192.168.0.10:5003/api/message/read";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(Token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(notif), UnicodeEncoding.UTF8, "application/json")
                };
                responseMessage = await httpClient.SendAsync(request);
            }
        }

        public async Task<IEnumerable<Message>> UnreadMessages(MessageToFrom notif)
        {
            string uri = "http://192.168.0.10:5003/api/message/unreadmessages";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(Token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(notif), UnicodeEncoding.UTF8, "application/json")
                };
                responseMessage = await httpClient.SendAsync(request);
            }
            var unreadMessage = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Message>>(unreadMessage);
        }

        //public async Task<IEnumerable<Message>> GetAllUnreadMessages()
        //{
        //    string uri = "http://192.168.0.10:5003/api/message/allunread";

        //    HttpResponseMessage responseMessage;
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        httpClient.SetBearerToken(Token);
        //        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
        //        responseMessage = await httpClient.SendAsync(request);
        //    }
        //    var ureadMessages = await responseMessage.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<IEnumerable<Message>>(ureadMessages);
        //}
    }
}
