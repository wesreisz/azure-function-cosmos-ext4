using CosmosDBSamplesV2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace loyaltyFunctions
{
    public static class RewardCount
    {
        [FunctionName("RewardCount")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "RewardCount/{email}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.Type = 'REWARD' AND c.IsClaimed = false AND c.CustomerEmail = {email}")] IEnumerable<Reward> unclaimedRewards,
            string email,
            ILogger log)
        {
            log.LogInformation("RewardCount function processed a request.");

            // Return the count of unclaimed rewards for the given customer
            return new OkObjectResult($"Customer {email} has {unclaimedRewards.Count()} unclaimed rewards.");
        }
    }
}
