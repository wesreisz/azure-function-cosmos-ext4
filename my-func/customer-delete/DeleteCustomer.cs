using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using CosmosDBSamplesV2;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

//NOTE: PartitionId is required, but if you don't create a partitian id you set it equal to the id.

namespace com.wesleyreisz.example
{
public static class DeleteCustomer
{
    [FunctionName("DeleteCustomer")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteCustomer/{id}")] HttpRequest req,
        string id,
        [CosmosDB(
            databaseName: "%CosmosDbConfig:DatabaseName%",
            containerName: "%CosmosDbConfig:ContainerName%",
            Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        ILogger log)
    {
        log.LogInformation("Delete function processed a request.");
        var container = cosmosClient.GetContainer("my-database","my-container");
        try
        {
            var findId = $"{id}";
            Customer customer = new Customer(){Id=id};
            //create
            //var result = await container.CreateItemAsync<Customer>(test, new PartitionKey(test.Id));
            //delete
            var result = await container.DeleteItemAsync<Customer>(customer.Id, new PartitionKey(customer.Id));         
        }
        catch(Exception ex)
        {
            //log.LogError("Error deleting item: " + ex.Message);
            return new NotFoundResult();
        }
        return new OkResult();
    }
}   
}