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

//example call:  curl "http://localhost:7071/api/GetCustomerByEmail/Wes@wesleyreisz.com" 

namespace loyaltyFunctions
{
    public static class GetCustomerEmail
    {
        [FunctionName("GetCustomerByEmail")]
        public static Task<string> Run(
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
            foreach (Customer customer in customers)
            {
                log.LogInformation($"Found Customer: {customer.CustomerEmail} {customer.Id})");

            }

            return Task.FromResult(JsonConvert.SerializeObject(customers));
        }
    }
}
