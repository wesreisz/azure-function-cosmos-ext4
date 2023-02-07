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

namespace com.wesleyreisz.example
{
    public static class GetCustomerName
    {
        [FunctionName("GetCustomerName")]
         public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", 
                Route = "GetCustomerName/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "my-database",
                containerName: "my-container",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id = {id}")]
                IEnumerable<Customer> customers,
            ILogger log)
            {

            log.LogInformation("Triggering Get Customer");
            foreach (Customer customer in customers)
            {
                log.LogInformation($"Found Customer: {customer.CustomerName} {customer.Id})");
                
            }

            return new OkResult();        }
    }
}
