using CloudCustomers.API.config;
using CloudCustomers.API.Models;
using CloudCustomers.API.Services;
using CloudCustomers.UnitTests.Fixtures;
using CloudCustomers.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit.Abstractions;

namespace CloudCustomers.UnitTests.Systems.Services;

public class TestUserService
{
    
    
    private readonly ITestOutputHelper _output;

    public TestUserService(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public async Task GetAllUsers_WhenCalled_InvokesHttpGetRequest()
    { 
        //arrange
        var expectedResponse = UsersFixture.GetTestUsers();
        
        //The mock Http message handler will simulate an HTTP response using the test data we provide.
        //The handler is the generic class where User is the type of the resource being mocked.
        var handlerMock = MockHttpMessageHandler<User>.SetupBasicGetResourceList(expectedResponse);
        
        //An HttpClient instance is created using the mock handler.
        var httpClient = new HttpClient(handlerMock.Object);
        var endpoint = "https://example.com/users";
        var config = Options.Create(new UsersApiOptions
        {
            Endpoint = endpoint
        });
        var sut = new UsersService(httpClient, config);
        // act
        var users = await sut.GetAllUsers();
        // assert
        // verify HTTP request is made
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(req=>req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAllUsers_WhenCalled_ReturnsListOfUsers()
    { 
        //arrange
        var expectedResponse = UsersFixture.GetTestUsers();
        
        var endpoint = "https://example.com/users";
        var config = Options.Create(new UsersApiOptions
        {
            Endpoint = endpoint
        });
        var handlerMock = MockHttpMessageHandler<User>.SetupBasicGetResourceList(expectedResponse);
        var httpClient = new HttpClient(handlerMock.Object);
        var sut = new UsersService(httpClient, config);
        // act
        var result =  await sut.GetAllUsers();
        _output.WriteLine($"Result Count: {result.Count}"); // Output the result count to the test output
        // assert
        // verify HTTP request is made
        result.Count().Should().Be(expectedResponse.Count);
    }
    
    [Fact]
    public async Task GetAllUsers_WhenCalled_InvokesConfiguredExternalUrl()
    { 
        //arrange
        var expectedResponse = UsersFixture.GetTestUsers();
        var endpoint = "https://example.com/users";
        var handlerMock = MockHttpMessageHandler<User>.SetupBasicGetResourceList(expectedResponse, endpoint);
        var httpClient = new HttpClient(handlerMock.Object);
       
        var config = Options.Create(new UsersApiOptions
        {
            Endpoint = endpoint
        });

        var sut = new UsersService(httpClient, config);
        
        // act
        var result =  await sut.GetAllUsers();
        // assert
        // verify HTTP request is made
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(req=>req.Method == HttpMethod.Get && req.RequestUri.ToString() == endpoint), ItExpr.IsAny<CancellationToken>());
    }
}