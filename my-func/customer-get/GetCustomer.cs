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

//TODO: Information on how to talk to CosmosDB using Extensions 4.0
//check this page: https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=in-process%2Cfunctionsv2&pivots=programming-language-csharp#http-trigger-get-multiple-docs-using-sqlquery-c

//example call to list customers: curl http://localhost:7071/api/GetCustomer | jq
//example call to get customer by id: curl http://localhost:7071/api/GetCustomer/9333a401-2881-4e94-84c5-962c319dcd8c | jq
namespace com.wesleyreisz.example
{
    public static class GetCustomer
    {
        [FunctionName("GetCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCustomer/{id?}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c order by c._ts desc")]
                IEnumerable<Customer> customers,
                String id,
                ILogger log)
            {
            //TODO: Some queries to look into and think about. How would you adapt this method to take some of this
            //into consideration:
            // - how do you limit the results returned? How do you paginate? Look at OFFSET 1 LIMIT 1
            // - how do you query for a subset of data? look at using where c.CustomerName = 'Justin Reisz'
            // - what are the ramifications of using a * in the select statement?
            var findId = $"{id}";
            if (findId != ""){
                log.LogInformation("Triggering Get Customer By ID %s", findId);
                foreach (Customer customer in customers){
                    if (findId==customer.Id){
                        log.LogInformation($"Found Customer: {customer.CustomerName} {customer.Id})");
                        return new OkObjectResult(customer);
                    }
                }
                return new NotFoundResult();
            }   
            else{
                log.LogInformation("Triggering Get Customer");
                return new OkObjectResult(customers);
            }
        }
    }
}
