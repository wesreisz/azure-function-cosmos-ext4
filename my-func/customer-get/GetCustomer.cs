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
    public static class GetCustomer
    {
        [FunctionName("GetCustomer")]
         public static Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "my-database",
                containerName: "my-container",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c where c.CustomerName = 'Justin Reisz'")]
                IEnumerable<Customer> customers,
            ILogger log)
            {
           
            log.LogInformation("Triggering Get Customer");
            foreach (Customer customer in customers)
            {
                log.LogInformation($"Found Customer: {customer.CustomerName} {customer.Id})");
            }
            //TODO: Information on how to talk to CosmosDB using Extensions 4.0
            //check this page: https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=in-process%2Cfunctionsv2&pivots=programming-language-csharp#http-trigger-get-multiple-docs-using-sqlquery-c
            return Task.FromResult(JsonConvert.SerializeObject(customers));
        }
    }
}
