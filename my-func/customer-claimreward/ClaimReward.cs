using CosmosDBSamplesV2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace loyaltyFunctions
{
    public static class ClaimReward
    {
        [FunctionName("ClaimReward")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ClaimReward/{email}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT TOP 1 * FROM c WHERE c.Type = 'REWARD' AND c.IsClaimed = false AND c.CustomerEmail = {email}")]  IEnumerable<Reward> rewards,
            [CosmosDB(
            databaseName: "%CosmosDbConfigDatabaseName%",
            containerName: "%CosmosDbConfigContainerName%",
            Connection = "CosmosDbConnectionString")] IAsyncCollector<Reward> documentsOut,
        ILogger log)
        {
            var reward = rewards.First();
            log.LogInformation("ClaimReward function processed a request.");

            if (reward == null)
            {
                // There is another error that prevents this code from running
                return new OkObjectResult("No unclaimed rewards found.");
            }

            // Update the reward to mark it as claimed.
            foreach (var Reward in rewards)
            {
                reward.IsClaimed = true;
                await documentsOut.AddAsync(reward);
            }
            reward.IsClaimed = true;

            return new OkObjectResult($"Reward {reward.Id} claimed for customer {reward.CustomerEmail}");
        }
    }
}
