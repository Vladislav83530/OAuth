using Newtonsoft.Json;
using System;
using System.Text;

namespace lab2
{
    public class AuthClient
    {
        private readonly string _domain;

        public AuthClient(string domain)
        {
            _domain = domain;
        }

        public async Task<string> GetToken(string audience, string clientId, string clientSecret)
        {
            using (var client = new HttpClient())
            {
                var tokenEndpoint = $"https://{_domain}/oauth/token";
                var requestBody = new
                {
                    audience = audience,
                    grant_type = "client_credentials",
                    client_id = clientId,
                    client_secret = clientSecret
                };


                var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(tokenEndpoint, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    dynamic responseData = JsonConvert.DeserializeObject(responseContent);
                    return responseData.access_token;
                }
                else
                {
                    throw new HttpRequestException($"Failed to get token: {response.StatusCode}");
                }
            }
        }

        public async Task<dynamic> CreateUser(User user, string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                string content = JsonConvert.SerializeObject(user);

                var requestContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://{_domain}/api/v2/users", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
                }
                else
                {
                    throw new HttpRequestException($"Failed to create user: {response.StatusCode}");
                }
            }
        }

        public async Task<dynamic> GetUserToken(string audience, string clientId, string clientSecret, string username, string userPassword)
        {
            if (string.IsNullOrEmpty(audience))
            {
                throw new ArgumentNullException(nameof(audience));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrEmpty(userPassword))
            {
                throw new ArgumentNullException(nameof(userPassword));
            }

            string url = $"https://{_domain}/oauth/token";

            using (HttpClient client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "scope", "offline_access" },
                    { "username", username },
                    { "password", userPassword },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "audience", audience }
                };

                HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(parameters));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Success Response:");
                    Console.WriteLine(responseBody);
                    return responseBody;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Exception: {response.StatusCode}");
                    throw new HttpRequestException();
                }
            }
        }

        public async Task<dynamic> GetRefreshToken(string audience, string clientId, string clientSecret, string refreshToken)
        {
            if (string.IsNullOrEmpty(audience))
            {
                throw new ArgumentNullException(nameof(audience));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            string url = $"https://{_domain}/oauth/token";

            using (HttpClient client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "audience", audience },
                    { "refresh_token", refreshToken },
                };

                HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(parameters));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Success Response:");
                    Console.WriteLine(responseBody);
                    return responseBody;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Exception: {response.StatusCode}");
                    throw new HttpRequestException();
                }
            }
        }
    }
}
