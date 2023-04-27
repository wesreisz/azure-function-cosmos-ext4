using CosmosDBSamplesV2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace loyaltyFunctions
{
    public static class CheckReward
    {
        [FunctionName("CheckReward")]
        static Task<IActionResult> Run(
    //This is the query that returns stuff
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ClaimReward/{email}")] HttpRequest req,
    [CosmosDB(
        databaseName: "%CosmosDbConfigDatabaseName%",
        containerName: "%CosmosDbConfigContainerName%",
        Connection = "CosmosDbConnectionString",
        SqlQuery = "SELECT * FROM c WHERE c.Type = 'PUNCH' AND c.CustomerEmail = {email} AND c.IsClaimed = false")]
    IEnumerable<Punch> punches,
    //This lets the function write to CosmosDb- writes out the rewardDocument
    [CosmosDB(
        databaseName: "%CosmosDbConfigDatabaseName%",
        containerName: "%CosmosDbConfigContainerName%",
        Connection = "CosmosDbConnectionString")] out dynamic rewardDocument,
    // This lets the function to collect a bunch of Punch objects and update them
    [CosmosDB(
        databaseName: "%CosmosDbConfigDatabaseName%",
        containerName: "%CosmosDbConfigContainerName%",
        Connection = "CosmosDbConnectionString")] IAsyncCollector<Punch> documentsOut,
    ILogger log,
    string email)
        {
            int punchesClaimed = punches.ToList().Count;
            int rewardsClaimed = punchesClaimed / 10;

            rewardDocument = new
            {
                id = Guid.NewGuid().ToString(),
                Type = "REWARD",
                CustomerEmail = email,
                RewardTotal = rewardsClaimed
            };

            // Update the punches to mark them as claimed.
            foreach (var punch in punches)
            {
                punch.IsClaimed = true;
                documentsOut.AddAsync(punch);
            }

            return Task.FromResult(JsonConvert.SerializeObject(rewardDocument));

        }
    }
}