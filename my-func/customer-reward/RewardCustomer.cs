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

// Example Post: curl -X POST http://localhost:7071/api/RewardCustomer/5599bb98-f8ae-4781-9e80-b27325d07bb6
namespace com.wesleyreisz.example
{
    public static class RewardCustomer
    {
        [FunctionName("RewardCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string id = data.id;

            try
            {
                dynamic customerDocument = await documentsOut.ReadDocumentAsync(id);
                int customerPunches = customerDocument.customerPunches;

                if (customerPunches >= 10)
                {
                    customerDocument.customerPunches -= 10;

                    await documentsOut.ReplaceDocumentAsync(customerDocument.id, customerDocument);

                    string responseMessage = $"Reward claimed successfully for customer with ID: {id}. Remaining punches: {customerDocument.customerPunches}";
                    return new OkObjectResult(responseMessage);
                }
                else
                {
                    string responseMessage = $"Not enough punches for a reward for customer with ID: {id}. Punches required: 10, Remaining punches: {customerPunches}";
                    return new OkObjectResult(responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to reward customer: {ex.Message}");
                return new BadRequestObjectResult("Failed to reward customer");
            }
        }
    }
}
