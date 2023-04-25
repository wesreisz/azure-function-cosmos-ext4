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

//example call to list customers: curl http://localhost:7071/api/GetCustomer | jq
namespace loyaltyFunctions
{
    public static class GetAllCustomers
    {
        [FunctionName("GetAllCustomers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetAllCustomers/")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c where c.Type='CUSTOMER' order by c._ts desc")]
                IEnumerable<Customer> customers,
                ILogger log)
            {

                
                log.LogInformation("Triggering Get Customer");
                return new OkObjectResult(customers);
            }
        }
}
