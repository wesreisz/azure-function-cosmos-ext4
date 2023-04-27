using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBSamplesV2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                SqlQuery = "SELECT * FROM c where c.Type='CUSTOMER'")]
        IEnumerable<Customer> customers,
            ILogger log)
        {
            log.LogInformation("GetAllCustomers processed a request.");

            // Create a dictionary to hold the most recent customer data for each unique email
            Dictionary<string, Customer> uniqueCustomers = new Dictionary<string, Customer>();

            // Loop through each customer in the list and check if it is the most recent one for that email
            foreach (Customer customer in customers)
            {
                if (uniqueCustomers.TryGetValue(customer.CustomerEmail, out Customer existingCustomer))
                {
                    if (long.Parse(customer._ts) > long.Parse(existingCustomer._ts))
                    {
                        uniqueCustomers[customer.CustomerEmail] = customer;
                    }
                }
                else
                {
                    uniqueCustomers.Add(customer.CustomerEmail, customer);
                }
            }

            return new OkObjectResult(uniqueCustomers.Values.ToList());
        }
    }
}
