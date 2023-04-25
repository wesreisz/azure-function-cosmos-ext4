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
using Azure.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using System.Net;
using System.Web.Http;
using Microsoft.Azure.Cosmos;

//example call:  curl "http://localhost:7071/api/GetCustomerByEmail/Wes@wesleyreisz.com" 

namespace loyaltyFunctions
{
    public static class GetCustomerEmail
    {
        [FunctionName("GetCustomerByEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",
                Route = "GetCustomerByEmail/{email}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.CustomerEmail = {email}")]
                IEnumerable<Customer> customers,
            ILogger log)
        {

            log.LogInformation("Triggering Get Customer");
            int count = 0;
            foreach (Customer customer in customers)
            {
                count++;
                log.LogInformation($"Found Customer: {customer.CustomerEmail} {customer.Id})");

            }

            //I need this to return 404 if customer not found

            if (count > 0)
            {
                return Task.FromResult((IActionResult)new OkObjectResult(JsonConvert.SerializeObject(customers)));
            }
            else
            {
                return Task.FromResult((IActionResult) new NotFoundResult());
            }
        }
    }
}
