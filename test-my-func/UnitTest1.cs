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

   //     var result = await com.wesleyreisz.example.GetCustomer.Run(
     //       req: HttpRequestSetup(query, body),
       //     customers: customers,
         //   id: "",
           // log: log);
        Customer wes = customers.Last<Customer>();
        Assert.Equal("Wesley Reisz", wes.CustomerName);
        
    }

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