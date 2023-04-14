using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using CosmosDBSamplesV2;


namespace loyaltyFunctions
{
    // Example Post: curl -X POST http://localhost:7071/api/UpdateCustomer/5599bb98-f8ae-4781-9e80-b27325d07bb6 -d "{'id':'5599bb98-f8ae-4781-9e80-b27325d07bb6', 'name':'Wesley Reisz', 'email':'wes@wesleyreisz.com', 'phone':'502-802-2361'}"
    public static class UpdateCustomer
    {
        [FunctionName("UpdateCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateCustomer/{id}")] HttpRequest req,
            [CosmosDB(
                 databaseName: "%CosmosDbConfigDatabaseName%",
                 containerName: "%CosmosDbConfigContainerName%",
                 Connection = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function UpdateCustomer processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (requestBody.Contains("id") &&
                requestBody.Contains("name") &&
                requestBody.Contains("email") &&
                requestBody.Contains("phone"))
            {
                string customerId = data.id;

                // Update the customer in Cosmos DB
                var container = documentsOut as Container;
                var partitionKey = new PartitionKey(customerId);
                var updatedCustomer = new Customer
                {
                    Id = customerId,
                    CustomerEmail = data.email,
                    CustomerName = data.name,
                    CustomerPhone = data.phone
                };
                await container.ReplaceItemAsync(updatedCustomer, customerId, partitionKey);

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
