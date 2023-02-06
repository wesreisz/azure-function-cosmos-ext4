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

namespace com.wesleyreisz.example
{
    public static class PostItem
    {
        [FunctionName("PostItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                 databaseName: "my-database",
                 containerName: "my-container",
                 Connection = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //this gets a value from the query string
            string name = req.Query["name"];

            //this reads the body and deserializes it to json
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            //this is a shortcut to creating an object. You should create an object
            //structure in the src folder and then use that in your logic. I did this
            //for validation purposes.
            if (!string.IsNullOrEmpty(name))
            {
                var item = new ToDoItem();
                // Add a JSON document to the output container.
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    id = System.Guid.NewGuid().ToString(),
                    task = "Create New Record",
                    assignee = "Justin",
                    name = name
                });
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully and preserved in cosmosdb.";

            return new OkObjectResult(responseMessage);
        }
    }
}
