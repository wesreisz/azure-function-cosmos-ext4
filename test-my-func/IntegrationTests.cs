using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using loyaltyFunctions;
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
        var message = getCustomersResponse.EnsureSuccessStatusCode();
        Assert.True(message.StatusCode == System.Net.HttpStatusCode.OK, "Got an invalid response code calling '/GetCustomer'");

        var initialCustomerList = JsonConvert.DeserializeObject<List<Customer>>(await getCustomersResponse.Content.ReadAsStringAsync());

        //post customer
        await client.PostAsync($"{FunctionBaseUrl}/PostCustomer", content);
        var getPostResponse = await client.GetAsync($"{FunctionBaseUrl}/GetCustomer");
        var newCustomerList = JsonConvert.DeserializeObject<List<Customer>>(await getPostResponse.Content.ReadAsStringAsync());
        Assert.True(
            initialCustomerList.ToArray<Customer>().Length + 1 == newCustomerList.ToArray<Customer>().Length,
            "New Customer List does not container new customer record");


        //customer by name/delete customer
        var getResponse = await client.GetAsync($"{FunctionBaseUrl}/GetCustomerByName/Wesley%20Reisz");
        Assert.True(message.StatusCode == System.Net.HttpStatusCode.OK, "Couldn't find Customer by Name");
        var customer = JsonConvert.DeserializeObject<List<Customer>>(await getResponse.Content.ReadAsStringAsync()).FirstOrDefault();
        Assert.IsType<List<Customer>>(newCustomerList);
        Assert.NotNull(customer);
        Assert.Equal("Wesley Reisz", customer.CustomerName);
        Assert.NotNull(customer.Id);
        var deleteResponse = await client.DeleteAsync($"{FunctionBaseUrl}/DeleteCustomer/{customer.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        var getDeleteResponse = await client.GetAsync($"{FunctionBaseUrl}/GetCustomer");
        var lastCustomerList = JsonConvert.DeserializeObject<List<Customer>>(await getDeleteResponse.Content.ReadAsStringAsync());

        Assert.True(
            initialCustomerList.ToArray<Customer>().Length == lastCustomerList.ToArray<Customer>().Length,
            "Delete Customer did not execute correctly.");
    }


}