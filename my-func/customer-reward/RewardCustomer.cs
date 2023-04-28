using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


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
    string email,
    ILogger log)
        {
            log.LogInformation("RewardCustomer function processed a request.");

            rewardDocument = new
            {
                id = Guid.NewGuid().ToString(),
                Type = "REWARD",
                CustomerEmail = email,
            };

            return Task.FromResult((IActionResult)new OkObjectResult(JsonConvert.SerializeObject(rewardDocument)));
        }
    }
}