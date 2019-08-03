using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubWPF
{
    public class ActiveUserClient
    {
        public async Task<IEnumerable<ActiveUserInfo>> ActiveUsers(string Token)
        {
            return await ActiveUsersRequest(Token);
        }
        private async static Task<IEnumerable<ActiveUserInfo>> ActiveUsersRequest(string token)
        {

            string uri = "http://192.168.0.10:51711/api/OnlineUsers";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                responseMessage = await httpClient.SendAsync(request);
            }
            var activeUsers = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<ActiveUserInfo>>(activeUsers);
        }
    }
}
