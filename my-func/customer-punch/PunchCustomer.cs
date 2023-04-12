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

//Example Post: curl -X POST http://localhost:7071/api/PunchCustomer/5599bb98-f8ae-4781-9e80-b27325d07bb6
namespace com.wesleyreisz.example
{
    public static class PunchCustomer
    {
        [FunctionName("PunchCustomer")]
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
                // Add a JSON document to the output container
                await documentsOut.AddAsync(new
                {
                    id,
                    customerPunches = 1 // Increase customerPunches by one?
                });

                string responseMessage = $"Punch added successfully to customer with ID: {id}";
                return new OkObjectResult(responseMessage);
            }


            catch (Exception ex)
            {
                log.LogError($"Failed to update customer: {ex.Message}");
                return new BadRequestObjectResult("Failed to update customer");
            }
        }
    }
}
