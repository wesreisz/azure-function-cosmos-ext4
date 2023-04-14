using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Collections.Generic;

namespace my_func
{
    public class TokenCreate
    {
        private const String DEFAULT_ROLE = "undefined";

        [FunctionName("TokenCreate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Generating new JWT token.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            String email = data != null ? data?.email : "";
            String role = data != null ? data?.role : DEFAULT_ROLE;

            return new OkObjectResult(new GenerateJWT().IssuingJWT(email, role));
        }
    }
}

