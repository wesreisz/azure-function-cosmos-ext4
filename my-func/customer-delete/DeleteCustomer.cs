using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CosmosDBSamplesV2;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System;

// Example post: curl -X DELETE http://localhost:7071/api/DeleteCustomer/wes%40wesleyreisz.com

namespace loyaltyFunctions
{
    public static class DeleteCustomer
    {
        [FunctionName("DeleteCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteCustomer/{email}")] HttpRequest req,
            string email,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
            ILogger log)
        {
            log.LogInformation("Delete function processed a request.");

            var container = cosmosClient.GetContainer("LoyaltyDatabase", "Customers");

            try
            {
                // Query for the customer by email
                var query = new QueryDefinition("SELECT * FROM c WHERE c.CustomerEmail = @CustomerEmail")
                    .WithParameter("@CustomerEmail", email);

                var results = container.GetItemQueryIterator<Customer>(query);

                while (results.HasMoreResults)
                {
                    var response = await results.ReadNextAsync();

                    foreach (var customer in response)
                    {
                        // Delete the customer and all associated items by id
                        await container.DeleteItemAsync<Customer>(customer.Id, new PartitionKey(customer.Id));

                        // Query for punches with the customer id as the partition key
                        var punchQuery = new QueryDefinition("SELECT * FROM c WHERE c.Type = 'PUNCH' and c.CustomerId = @CustomerId")
                            .WithParameter("@CustomerId", customer.Id);

                        var punchResults = container.GetItemQueryIterator<Punch>(punchQuery);

                        while (punchResults.HasMoreResults)
                        {
                            var punchResponse = await punchResults.ReadNextAsync();

                            foreach (var punch in punchResponse)
                            {
                                await container.DeleteItemAsync<Punch>(punch.Id, new PartitionKey(punch.Id));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error deleting item: " + ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkResult();
        }
    }
}
