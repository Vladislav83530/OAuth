using Newtonsoft.Json;

namespace lab2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var domain = "dev-jeiftr0g0eqxoi0m.us.auth0.com";

            var auth0Client = new AuthClient(domain);

            var audience = "https://dev-jeiftr0g0eqxoi0m.us.auth0.com/api/v2/";
            var clientId = "YTtpQheSXcPkavvQf31K5eqlg4W7EFo9";
            var clientSecret = "X8t8tFe6tebdPPhmtzkV8k0JCucBifB0muQ7Id1toxhgJtG50ObNZg8tMGw3Dxat";

            var token = await auth0Client.GetToken(audience, clientId, clientSecret);
            Console.WriteLine("Access Token: " + token);

            User user = new User()
            {
                email = "testUser@example.com",
                blocked = false,
                email_verified = false,
                given_name = "test given name",
                family_name = "family name",
                name = "test name",
                nickname = "test nickname",
                picture = "https://picture.com/images",
                connection = "Username-Password-Authentication",
                password = "Password1111."
            };

            //var createUserResult = await auth0Client.CreateUser(user, token);
            //Console.WriteLine(createUserResult);

            var getUserTokenResponse = await auth0Client.GetUserToken(audience, clientId, clientSecret, user.email, user.password);
            Console.WriteLine(getUserTokenResponse);

            string refreshToken = JsonConvert.DeserializeObject<UserTokenResponse>(getUserTokenResponse).refresh_token;

            var refreshTokenResponse = await auth0Client.GetRefreshToken(audience, clientId, clientSecret, refreshToken);
            Console.WriteLine(refreshTokenResponse);
        }
    }
}
