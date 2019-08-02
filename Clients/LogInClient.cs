using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChatHubWPF
{
    public class LogInClient
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        //Get token for the specified clientId, clientSecret, and scope
        static readonly  string Url = "http://192.168.0.10:5000/";

        public LogInClient(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public async Task<string> Authorize()
        {
            string token = await RequestTokenToAuthorizationServer(new Uri(Url), UserName, Password);
            return token;
        }

        /// <summary>
        /// Request token from specified endpoint, by username and password
        /// </summary>
        /// <param name="uriAuthorizationServerUri"></param>
        /// <param name="username">User's Username</param>
        /// <param name="password">User's Password</param>
        /// <returns></returns>
        public static async Task<string> RequestTokenToAuthorizationServer(Uri uriAuthorizationServerUri, string username, string password)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            using (HttpClient client = new HttpClient())
            {
                // returns DiscoveryResponse 
                client.BaseAddress = uriAuthorizationServerUri;
                var disco = client.GetDiscoveryDocumentAsync(uriAuthorizationServerUri.ToString()).Result;
                HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Post, disco.TokenEndpoint);
                HttpContent httpContent = new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("client_id", "client"),
                        new KeyValuePair<string, string>("scope", "ActiveUserService"),
                        new KeyValuePair<string, string>("scope", "MessageService"),
                        new KeyValuePair<string, string>("client_secret", "secret"),
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password)
                    });

                tokenRequest.Content = httpContent;
                responseMessage = await client.SendAsync(tokenRequest);
            }
            return await responseMessage.Content.ReadAsStringAsync();
        } 
    }
}
