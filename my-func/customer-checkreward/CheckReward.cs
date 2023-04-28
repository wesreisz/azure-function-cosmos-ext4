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
// More like Update reward, need another function to count the number of rewards
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
    ILogger log,
    string email)

        {
            {
                log.LogInformation("CheckReward processed a request.");
                log.LogInformation($"Found {punches.Count()} unclaimed punches for customer {email}: {string.Join(",", punches.Select(p => p.Id))}");
                int punchesClaimed = punches.ToList().Count;
                //Might need to change this later- just counts punches/10- doesn't account for claimed rewards
                int rewardsClaimed = punchesClaimed / 10;

                return Task.FromResult((IActionResult)new OkObjectResult("Punches returned"));
            }
        }
    }
}