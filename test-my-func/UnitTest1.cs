using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using com.wesleyreisz.example;
using CosmosDBSamplesV2;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections;
//using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Net;

namespace test_my_func;

public class UnitTest1
{
    [Fact]
    public void TestBasicFunction()
    {
        Assert.Equal(4,2+2);
    }

    [Fact]
    public async Task RequestGetTest()
    {
        var query = new Dictionary<String, StringValues>();
        //query.TryAdd("name", "ushio");
        var body = "";

        ILogger log = Mock.Of<ILogger>();
        IEnumerable<Customer> customers = Mock.Of<IEnumerable<Customer>>();

        var result = com.wesleyreisz.example.GetCustomer.Run(
            req: HttpRequestSetup(query, body),
            customers: customers,
            id: "",
            log: log);
        Customer wes = customers.Last<Customer>();
        Assert.Equal("Wesley Reisz", wes.CustomerName);
        
    }

    /*
using Microsoft.AspNetCore.TestHost;

[Fact]
public async Task RequestGetTest()
{
   // Set up the test server
   var builder = new WebHostBuilder()
       .ConfigureServices(services =>
       {
           // Add any necessary services to the service collection
       })
       .UseStartup<Startup>();
   var server = new TestServer(builder);

   // Set up the HTTP request
   var client = server.CreateClient();
   var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetCustomer");

   // Send the request and get the response
   var response = await client.SendAsync(request);
   var content = await response.Content();

   // Verify the response
   response.EnsureSuccessStatusCode();
   Assert.IsType<OkObjectResult>(response.Content);
}

          {
            // Arrange
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/GetCustomer");

            // Act
            var response = await httpClient.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.IsType<OkObjectResult>(response.Content);
        }
*/

    public HttpRequest HttpRequestSetup(Dictionary<String, StringValues> query, string body)
    {
        var reqMock = new Mock<HttpRequest>();

        reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(body);
        writer.Flush();
        stream.Position = 0;
        reqMock.Setup(req => req.Body).Returns(stream);
        return reqMock.Object;
    }
}
