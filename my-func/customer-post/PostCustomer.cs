using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

//Example Post: curl -X POST http://localhost:7071/api/PostCustomer -d "{'name':'Wesley Reisz', 'email':'wes@wesleyreisz.com', 'phone':'502-802-2361'}"
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

            if (requestBody.Contains("name") && 
                requestBody.Contains("email") &&
                requestBody.Contains("phone"))
            {
                // Add a JSON document to the output container.
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    id = System.Guid.NewGuid().ToString(),

                    Type = "CUSTOMER",
                    CustomerEmail =data.email,
                    CustomerName=data.name,
                    CustomerPhone=data.phone
                });
            }
            else if (requestBody.Contains("email"))
            {
                await documentsOut.AddAsync(new
                {
                    id = System.Guid.NewGuid().ToString(),

                    Type = "CUSTOMER",
                    CustomerEmail = data.email,
                });
            }
            string responseMessage = $"This HTTP triggered function executed successfully and preserved in cosmosdb using {data.name},{data.phone},{data.email} .";

            return new OkObjectResult(responseMessage);
        }
    }
}
