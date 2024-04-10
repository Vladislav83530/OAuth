using Newtonsoft.Json;
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
                    var responseContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to create user: {response.StatusCode}");
                }
            }
        }
    }
}
