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
    private const string FunctionBaseUrl = "http://localhost:7071/api";
    private string newCustomerID;

    [Fact]
    public void TestBasicFunction()
    {
        Assert.Equal(4, 2 + 2);
    }

    [Fact]
    public async Task GetAllCustomers()
    {
        // Arrange
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{FunctionBaseUrl}/GetCustomer");

        // Act
        var response = await httpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<List<Customer>>(content);
        Assert.IsType<List<Customer>>(customers);
        foreach (var customer in customers) {
            if (customer.CustomerName.Contains("Wesley")) {
                Assert.Equal("Wesley Reisz", customer.CustomerName);
            }
        }
    }


    [Fact]
    public async Task PostCustomer()
    {
        //Arrange
        var httpClient = new HttpClient();

        // Create a customer/Arrange
        var newCustomer = new { name = "Wesley Reisz", email = "wes@wesleyreisz.com", phone = "502-802-2361" };
        var content = new StringContent(JsonConvert.SerializeObject(newCustomer));

        // Act
        var postResponse = await httpClient.PostAsync($"{FunctionBaseUrl}/PostCustomer", content);
        postResponse.EnsureSuccessStatusCode();

        // Assert?
    }

    [Fact]
    public async Task GetCustomerByName()
        //And Delete, it didn't like me adding delete to the name
    {
        //Arrange
        var httpClient = new HttpClient();

        //Act
        var getResponse = await httpClient.GetAsync($"{FunctionBaseUrl}/GetCustomerByName/Wesley%20Reisz");
        getResponse.EnsureSuccessStatusCode();

        //Assert
        var responseContent = await getResponse.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<List<Customer>>(responseContent);
        Assert.Single(customers);
        var newCustomerID = customers[0].Id;
        Assert.Equal("Wesley Reisz", customers[0].CustomerName);

        //Act
        var deleteResponse = await httpClient.DeleteAsync($"{FunctionBaseUrl}/DeleteCustomer/{newCustomerID}");
        //Assert?
        deleteResponse.EnsureSuccessStatusCode();

    }

    //[Fact]
    //public async Task DeleteCustomer()
    //{
    //    //Arrange
    //    var httpClient = new HttpClient();
       
    //    //Act
    //    var deleteResponse = await httpClient.DeleteAsync($"{FunctionBaseUrl}/DeleteCustomer/{newCustomerID}");

    //    //Assert?
    //    deleteResponse.EnsureSuccessStatusCode();
    //}
}