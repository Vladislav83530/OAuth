using Newtonsoft.Json;

namespace lab2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var domain = "kpi.eu.auth0.com";

            var auth0Client = new AuthClient(domain);

            var audience = "https://kpi.eu.auth0.com/api/v2/";
            var clientId = "JIvCO5c2IBHlAe2patn6l6q5H35qxti0";
            var clientSecret = "ZRF8Op0tWM36p1_hxXTU-B0K_Gq_-eAVtlrQpY24CasYiDmcXBhNS6IJMNcz1EgB";
            var token = await auth0Client.GetToken(audience, clientId, clientSecret);
            Console.WriteLine("Access Token: " + token);

            User user = new User()
            {
                email = "myuser21124r748@example.com",
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

            var createdUser = await auth0Client.CreateUser(user, token);
            Console.WriteLine("User created successfully!");
            Console.WriteLine("User ID: " + createdUser.user_id);
        }
    }
}
