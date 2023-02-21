using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Web.Http;
using Microsoft.Extensions.Logging;
using my_func.sampleget;
using com.wesleyreisz.example;
using CosmosDBSamplesV2;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using System.Reflection;

namespace test_my_func
{
	public class SampleTestCases
	{
        [Theory]
        [InlineData("", typeof(BadRequestResult))]
        [InlineData("QueryParamValue", typeof(OkResult))]
        [InlineData("ThisStringCausesTheFunctionToThrowAnError", typeof(InternalServerErrorResult))]
        public async Task Function_Returns_Correct_StatusCode(string queryParam, Type expectedResult)
        {
            //Arrange
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "q", new StringValues(queryParam) } });
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Query)
                .Returns(() => qc);

            var logger = Mock.Of<ILogger>();
            //Act
            var response = await Function1.Run(request.Object, logger);
            //Assert
            Assert.True(response.GetType() == expectedResult);
        }

        [Theory(DisplayName = "Test return object types when getting by id or askign for a list")]
        [InlineData("", typeof(OkObjectResult))]
        [InlineData("123", typeof(OkObjectResult))]
        [InlineData("9999", typeof(NotFoundResult))]
        public async Task Function_Returns_Customer(string queryParam, Type expectedResult)
        {
            //Arrange
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "q", new StringValues(queryParam) } });
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Query)
                .Returns(() => qc);

            var customers = new List<Customer>() {
                new Customer(){Id="1", CustomerName="wes reisz",CustomerEmail="wes@wesleyreisz.com", CustomerPhone="502-802-2361" },
                new Customer(){Id="123", CustomerName="justin reisz",CustomerEmail="justin@wesleyreisz.com", CustomerPhone="502-802-1234" }
            };

            var logger = Mock.Of<ILogger>();
            //Act
            var response = await GetCustomer.Run(request.Object, customers, queryParam, logger);
            //Assert
            Assert.True(response.GetType() == expectedResult);
        }

        [Theory(DisplayName = "Should return a list of two customer objects")]
        [InlineData("", 2)]
        public async Task Function_Returns_CustomerList(string queryParam, int count)
        {
            //Arrange
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "q", new StringValues(queryParam) } });
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Query)
                .Returns(() => qc);

            var customers = new List<Customer>() {
                new Customer(){Id="1", CustomerName="wes reisz",CustomerEmail="wes@wesleyreisz.com", CustomerPhone="502-802-2361" },
                new Customer(){Id="123", CustomerName="justin reisz",CustomerEmail="justin@wesleyreisz.com", CustomerPhone="502-802-1234" }
            };

            var logger = Mock.Of<ILogger>();
            //Act
            var response = await GetCustomer.Run(request.Object, customers, "", logger);
            ObjectResult objectResponse = Assert.IsType<OkObjectResult>(response);
            IEnumerable<Customer> cust = (List<Customer>)objectResponse.Value;         

            //Assert
            Assert.True(cust.Count() == count);
        }

    }
}

