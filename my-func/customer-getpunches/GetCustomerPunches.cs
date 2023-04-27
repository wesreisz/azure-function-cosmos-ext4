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

namespace my_func.customergetpunches
{
    public static class GetCustomerPunches
    {
        [FunctionName("GetCustomerPunches")]
        public static Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",
                Route = "GetCustomerPunches/{email}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbConfigDatabaseName%",
                containerName: "%CosmosDbConfigContainerName%",
                Connection = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c where c.Type='PUNCH' AND c.CustomerEmail={email} order by c._ts desc")]
            IEnumerable<Punch> punches,
            ILogger log)
        {

            log.LogInformation("Triggering Get Customer");
            var totalPunches = 0;
            // Each time a punch event is found totalPunches increments
            foreach (Punch punch in punches)
            {
                log.LogInformation($"Found Customer: {punch.CustomerEmail} {punch.Id})");
                totalPunches++;
            }
            var punchTotal = 10;//replace with a config value from cosmos
            var result = new{
                punchNumber = totalPunches%punchTotal,
                rewardTotal= totalPunches/punchTotal,
                punchTotal = punchTotal
            };

            return Task.FromResult(JsonConvert.SerializeObject(result));
        }
    }
}