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
    public static class RewardCustomer
    {
        [FunctionName("RewardCustomer")]
        public static Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "RewardCustomer/{email}")] HttpRequest req,
    //This lets the function write to CosmosDb- writes out the rewardDocument
    [CosmosDB(
        databaseName: "%CosmosDbConfigDatabaseName%",
        containerName: "%CosmosDbConfigContainerName%",
        Connection = "CosmosDbConnectionString")] out dynamic rewardDocument,
        string email)
        {

            rewardDocument = new
            {
                id = Guid.NewGuid().ToString(),
                Type = "REWARD",
                CustomerEmail = email,
            };

            return Task.FromResult(JsonConvert.SerializeObject(rewardDocument));

        }
    }
}