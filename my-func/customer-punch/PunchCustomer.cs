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

//Example Post: curl http://localhost:7071/api/PunchCustomer/wes%40wesleyreisz.com
namespace loyaltyFunctions
{
    public static class PunchCustomer
    {
        [FunctionName("PunchCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",  Route = "PunchCustomer/{email}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log,
            String email)
        {
            log.LogInformation("C# HTTP trigger function PunchCustomer processed a request.");

            string punchEmail = $"{email}";

            try
            {
                // Add a JSON document to the output container
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    id = System.Guid.NewGuid().ToString(),
                    Type = "PUNCH",
                    CustomerEmail = punchEmail,
                    IsClaimed = false
                });

                string responseMessage = $"Punch added successfully to customer with {email}";
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to update customer: {ex.Message}");
                return new BadRequestObjectResult($"Failed to insert punch for customer with {email}");
            }

        }
    }
}
