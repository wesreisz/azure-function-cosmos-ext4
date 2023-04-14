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

//Example Post: curl -X POST http://localhost:7071/api/PostCustomer -d "{'name':'Wesley Reisz', 'email':'wes@wesleyreisz.com', 'phone':'502-802-2361'}"
// TODO: Post needs to check to see if there is already a Customer matching the description before posting a new one
namespace loyaltyFunctions
{
    public static class PostCustomer
    {
        [FunctionName("PostCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                 databaseName: "%CosmosDbConfigDatabaseName%",
                 containerName: "%CosmosDbConfigContainerName%",
                 Connection = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function PostCustomer processed a request.");

            //this reads the body and deserializes it to json
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            //you can also create using the CosmosDbClient (see delete) and do this
            //var result = await container.CreateItemAsync<Customer>(test, new PartitionKey(test.Id));

            if (requestBody.Contains("name") && 
                requestBody.Contains("email") &&
                requestBody.Contains("phone"))
            {
                // Add a JSON document to the output container.
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    id = System.Guid.NewGuid().ToString(),
                    
                    CustomerEmail=data.email,
                    CustomerName=data.name,
                    CustomerPhone=data.phone
                });
            }

            string responseMessage = $"This HTTP triggered function executed successfully and preserved in cosmosdb using {data.name},{data.phone},{data.email} .";

            return new OkObjectResult(responseMessage);
        }
    }
}
