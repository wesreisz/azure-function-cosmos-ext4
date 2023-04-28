using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CosmosDBSamplesV2;
using Microsoft.Azure.Cosmos;

//NOTE: PartitionId is required, but if you don't create a partitian id you set it equal to the id.

namespace loyaltyFunctions
{
public static class DeleteCustomer
{
    [FunctionName("DeleteCustomer")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteCustomer/{id}")] HttpRequest req,
        string id,
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
            var findId = $"{id}";
            Customer customer = new Customer(){Id=id};
            var result = await container.DeleteItemAsync<Customer>(customer.Id, new PartitionKey(customer.Id));         
        }
        catch//(Exception ex)
        {
            //log.LogError("Error deleting item: " + ex.Message);
            return new NotFoundResult();
        }
        return new OkResult();
    }
}   
}