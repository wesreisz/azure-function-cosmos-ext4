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

namespace com.wesleyreisz.example
{
    public static class UpdateCustomer
    {
        [FunctionName("UpdateCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            [CosmosDB(
                 databaseName: "%CosmosDbConfigDatabaseName%",
                 containerName: "%CosmosDbConfigContainerName%",
                 Connection = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (requestBody.Contains("id") &&
                requestBody.Contains("name") &&
                requestBody.Contains("email") &&
                requestBody.Contains("phone"))
            {
                string customerId = data.id;

                // Update the customer in Cosmos DB
                await documentsOut.ReplaceAsync(customerId, new
                {
                    id = customerId,
                    CustomerEmail = data.email,
                    CustomerName = data.name,
                    CustomerPhone = data.phone
                });

                string responseMessage = $"This HTTP triggered function executed successfully and updated the customer with ID {customerId}.";
                return new OkObjectResult(responseMessage);
            }
            else
            {
                string responseMessage = $"Error: The request body is missing required fields.";
                return new BadRequestObjectResult(responseMessage);
            }
        }
    }
}
