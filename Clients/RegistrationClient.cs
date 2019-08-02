using ChatHubWPF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clients
{
    public class RegistrationClient
    {
        public async Task<bool> RegisterUser(User newUser)
        {
            string uri = "http://192.168.0.10:45455/api/users";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json")
                };
                responseMessage = await httpClient.SendAsync(request);

                if(responseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                ////Equivalent to above three lines (lines 65-67)
                //httpClient.SetBearerToken(authorizationServerToken.AccessToken);
                //var response = await httpClient.GetAsync("uri");
            }
            return false;
        }
    }
}
