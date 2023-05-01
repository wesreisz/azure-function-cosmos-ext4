using CosmosDBSamplesV2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//example call:  curl "http://localhost:7071/api/CheckReward/Wes@wesleyreisz.com" 
// This function checks punches to see if any new rewards can be generated and generates them.
namespace loyaltyFunctions
{
    public static class CheckReward
    {
            [FunctionName("CheckReward")]
            public static Task<IActionResult> Run(
        //This is the query that returns stuff
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "CheckReward/{email}")] HttpRequest req,
        [CosmosDB(
            databaseName: "%CosmosDbConfigDatabaseName%",
            containerName: "%CosmosDbConfigContainerName%",
            Connection = "CosmosDbConnectionString",
            SqlQuery = "SELECT * FROM c WHERE c.Type = 'PUNCH' AND c.CustomerEmail = {email}")]
             IEnumerable<Punch> punches,
        //This lets the function write to CosmosDb- writes out the rewardDocument
        [CosmosDB(
            databaseName: "%CosmosDbConfigDatabaseName%",
            containerName: "%CosmosDbConfigContainerName%",
            Connection = "CosmosDbConnectionString")] IAsyncCollector<dynamic> rewardDocument,
        // This lets the function to collect a bunch of Punch objects and update them
        [CosmosDB(
            databaseName: "%CosmosDbConfigDatabaseName%",
            containerName: "%CosmosDbConfigContainerName%",
            Connection = "CosmosDbConnectionString")] IAsyncCollector<Punch> documentsOut,
        ILogger log,
        string email)
        {
            {
                log.LogInformation("CheckReward processed a request.");
                log.LogInformation($"Found {punches.Count()} unclaimed punches for customer {email}: {string.Join(",", punches.Select(p => p.Id))}");
                int punchesClaimed = punches.ToList().Count;
                //Might need to change this later- just counts punches/10- doesn't account for claimed rewards
                int rewardsClaimed = punchesClaimed / 10;

                if (punchesClaimed < 10)
                {
                    return Task.FromResult((IActionResult)new BadRequestObjectResult($"Customer {email} needs to collect more punches to claim a reward."));

                }
                else if (punchesClaimed >= 10)
                {
                    for (int i=0; i<rewardsClaimed; i++)
                    { 
                    rewardDocument.AddAsync(new
                    {
                        id = Guid.NewGuid().ToString(),
                        Type = "REWARD",
                        CustomerEmail = email
                    });

                    // Update the punches to mark them as claimed.
                    foreach (var punch in punches)
                    {
                        punch.IsClaimed = true;
                        documentsOut.AddAsync(punch);
                    }

                    return Task.FromResult((IActionResult)new OkObjectResult(JsonConvert.SerializeObject(rewardDocument)));
                    }
                    return Task.FromResult((IActionResult)new StatusCodeResult(StatusCodes.Status500InternalServerError));
                }
                else
                    return Task.FromResult((IActionResult)new StatusCodeResult(StatusCodes.Status500InternalServerError));
            }
        }
    }
}