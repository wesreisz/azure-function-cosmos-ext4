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
using System.Net.Http;

namespace test_my_func;

public class UnitTest1
{
    private const string FunctionBaseUrl = "http://localhost:7071/api";
    private string newCustomerID;

    [Fact]
    public void TestBasicFunction()
    {
        Assert.Equal(4, 2 + 2);
    }

    [Fact]
    public async Task IntTest()
    {
        // Arrange
        var client = new HttpClient();
        //postcustomer info
        var newCustomer = new { name = "Wesley Reisz", email = "wes@wesleyreisz.com", phone = "502-802-2361" };
        var content = new StringContent(JsonConvert.SerializeObject(newCustomer));

        //Get customers
        var getCustomersResponse = await client.GetAsync($"{FunctionBaseUrl}/GetCustomer");
        getCustomersResponse.EnsureSuccessStatusCode();
        var customers = JsonConvert.DeserializeObject<List<Customer>>(await getCustomersResponse.Content.ReadAsStringAsync());

        //post customer
        var postResponse = await client.PostAsync($"{FunctionBaseUrl}/PostCustomer", content);
        postResponse.EnsureSuccessStatusCode();

        //customer by name
        var getResponse = await client.GetAsync($"{FunctionBaseUrl}/GetCustomerByName/Wesley%20Reisz");
        getResponse.EnsureSuccessStatusCode();
        var customer = JsonConvert.DeserializeObject<List<Customer>>(await getResponse.Content.ReadAsStringAsync()).FirstOrDefault();
        Assert.IsType<List<Customer>>(customers);
        Assert.NotNull(customer);
        Assert.Equal("Wesley Reisz", customer.CustomerName);
        Assert.NotNull(customer.Id);
        var deleteResponse = await client.DeleteAsync($"{FunctionBaseUrl}/DeleteCustomer/{customer.Id}");
        deleteResponse.EnsureSuccessStatusCode();

    }


}