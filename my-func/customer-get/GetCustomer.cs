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
                SqlQuery = "SELECT * FROM c order by c._ts desc")]
                IEnumerable<Customer> customers,
            ILogger log)
            {
            //TODO: Some queries to look into and think about. How would you adapt this method to take some of this
            //into consideration:
            // - how do you limit the results returned? How do you paginate? Look at OFFSET 1 LIMIT 1
            // - how do you query for a subset of data? look at using where c.CustomerName = 'Justin Reisz'
            // - what are the ramifications of using a * in the select statement?
           
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
