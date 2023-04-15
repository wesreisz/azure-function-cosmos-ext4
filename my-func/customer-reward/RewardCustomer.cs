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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "RewardCustomer/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id = {id}")] IEnumerable<Customer> customers,
            String id,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function RewardCustomer processed a request.");

            var findId = $"{id}";
            foreach (Customer customer in customers)
            {
                if (findId == customer.Id)
                {
                    log.LogInformation($"Found Customer: {customer.CustomerName} {customer.Id})");

                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);

                    log.LogInformation(customer.CustomerName);

                    customer.CustomerEmail = data.customerEmail;
                    customer.CustomerName = data.customerName;
                    customer.CustomerPhone = data.customerPhone;
                        
                    await documentsOut.AddAsync(new
                    {
                        customer.Id,
                        customer.CustomerEmail,
                        customer.CustomerName,
                        customer.CustomerPhone
                    });
                    return new OkObjectResult("Successfully updated Customer");
                }
            }

            return new NotFoundResult();
        }
    }
}
