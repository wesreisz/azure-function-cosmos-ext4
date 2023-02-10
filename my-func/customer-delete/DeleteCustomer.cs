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
        var container = cosmosClient.GetContainer("my-database","my-container");

        try
        {
            //https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.container.deleteitemasync?view=azure-dotnet
            var response = await container.DeleteItemStreamAsync(id: id, partitionKey: PartitionKey.None);
            return new OkObjectResult(response);
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
/* 
// Suppose our container is partitioned by tenantId, and we want to delete all the data for a particular tenant Contoso
ResponseMessage deleteResponse = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey("Contoso"));

 if (deleteResponse.IsSuccessStatusCode) {
    Console.WriteLine($"Delete all documents with partition key operation has successfully started");
} */
