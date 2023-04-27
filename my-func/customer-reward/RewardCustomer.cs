using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CosmosDBSamplesV2;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;

// Example Post: curl -X POST http://localhost:7071/api/RewardCustomer/5599bb98-f8ae-4781-9e80-b27325d07bb6
namespace loyaltyFunctions
{
    public static class RewardCustomer
    {
        [FunctionName("RewardCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "PunchCustomer/{email}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log,
            String email)
        {
            log.LogInformation("C# HTTP trigger function RewardCustomer processed a request.");


            return new OkObjectResult("Successfully updated Customer");

        }
    }
}
