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
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;


namespace com.wesleyreisz.example
{
    //8c058f7c-f94d-4ae0-8d86-78de0bd89662
public static class DeleteCustomer
{
    [FunctionName("DeleteCustomer")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteCustomer/{id}")] HttpRequest req,
        string id,
        [CosmosDB(
            databaseName: "my-database",
            containerName: "my-container",
            Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        ILogger log)
    {
        log.LogInformation("Delete function processed a request.");

        var database = cosmosClient.GetDatabase("my-database");
        var container = database.GetContainer("my-container");

        try
        {
            var response = await container.DeleteItemAsync<object>(id, new PartitionKey("partition-key"));
            return new OkObjectResult(response.Resource);
        }
        catch (CosmosException ex)
        {
            log.LogError("Error deleting item: {Error}", ex);
            return new NotFoundResult();
        }
    }
}
}