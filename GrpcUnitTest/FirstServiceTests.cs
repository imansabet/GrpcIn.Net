using Basics;
using FluentAssertions;
using GrpcIn.Net.Services;
using GrpcUnitTest.Helpers;

namespace GrpcUnitTest;

public class FirstServiceTests
{
    //subject under test
    private readonly IFirstService sut;
    public FirstServiceTests()
    {
        sut = new FirstService();
    }
    [Fact]
    public async void Unary_ShouldReturn_an_Object()
    {
        //Arange
        var request = new Request()
        {
            Content = "message"
        };
        var callContext = TestServerCallContext.Create();
        var expectedResponse = new Response()
        {
            Message = "messagefrom server"
        };
        //Act
        var actualResponse = await sut.Unary(request, callContext);
        //Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
}