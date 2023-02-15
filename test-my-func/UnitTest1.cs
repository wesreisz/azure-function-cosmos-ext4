using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using com.wesleyreisz.example;
using CosmosDBSamplesV2;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections;
using Microsoft.AspNetCore.TestHost;
//Need to use version six of testhost for dotnet 6
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
        // Arrange
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/GetCustomer");

        // Act
        var response = await httpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<List<Customer>>(content);
        Assert.IsType<List<Customer>>(customers);
        foreach (var customer in customers){
            if (customer.CustomerName.Contains("Wesley")) {
                Assert.Equal("Wesley Reisz", customer.CustomerName);
            }
        } 
    }
}
