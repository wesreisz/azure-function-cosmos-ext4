using CosmosDBSamplesV2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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
                SqlQuery = "SELECT TOP 1 * FROM c WHERE c.Type = 'REWARD' AND c.IsClaimed = false AND c.CustomerEmail = {email} ORDER BY c._ts ASC")] Reward reward,
            ILogger log)
        {
            log.LogInformation("ClaimReward function processed a request.");

            if (reward == null)
            {
                return new OkObjectResult("No unclaimed rewards found.");
            }

            // Update the reward to mark it as claimed.
            reward.IsClaimed = true;


            return new OkObjectResult($"Reward {reward.Id} claimed for customer {reward.CustomerEmail}");
        }
    }
}
