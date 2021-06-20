using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tweetinvi;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace FunctionAppTwitter
{
    public static class FunctionTwitterUserGet
    {
        [FunctionName("FunctionTwitterUserGet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string name = data?.name;

            var credential = new DefaultAzureCredential();
            var client = new SecretClient(new Uri("https://keyvault004a.vault.azure.net/"), credential);
            var APIKey = await client.GetSecretAsync("APIKey");
            var APIKeySecret = await client.GetSecretAsync("APIKeySecret");
            var AccessToken = await client.GetSecretAsync("AccessToken");
            var AccessTokenSecret = await client.GetSecretAsync("AccessTokenSecret");

            var authClient = new TwitterClient(APIKey.Value.Value, APIKeySecret.Value.Value, AccessToken.Value.Value, AccessTokenSecret.Value.Value);

            if (name == null || name == "")
                //return new BadRequestObjectResult("Pass a twiter name into the request body");
                //return null;
                return (ActionResult)new OkObjectResult(new TwitterUser());

            try
            {
                var userV2  = await authClient.UsersV2.GetUserByNameAsync(name);
                var friendsList = await authClient.Users.GetFriendIdsAsync(name);
                var followersList = await authClient.Users.GetFollowerIdsAsync(name);

                TwitterUser twitterUser = new TwitterUser()
                { Name = name, Id = userV2.User.Id, NumFriends = friendsList.Length.ToString(), NumFollowers = followersList.Length.ToString(), };

                return (ActionResult)new OkObjectResult(twitterUser);
            }
            catch (Exception)
            {
                //return new BadRequestObjectResult("Not a valid twitter user");
                //return null;
                return (ActionResult)new OkObjectResult(new TwitterUser());
            }

        }
    }
}
