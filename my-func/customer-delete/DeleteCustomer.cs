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
        Container container = cosmosClient.GetDatabase("my-database").GetContainer("my-container");

        try
        {
            var response = await container.DeleteItemAsync<Customer>(id: id, partitionKey: PartitionKey.None);
            return new OkObjectResult(response.Resource);
        }
        catch
        {
            log.LogError("Error deleting item: {Error}");
            return new NotFoundResult();
        }
//        return new OkResult();
    }
}   
}