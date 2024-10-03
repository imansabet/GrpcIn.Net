using Basics;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace firstGrpc.test.Integration;

public class FirstServiceTests : IClassFixture<MyFactory<Program>>
{
    private readonly MyFactory<Program> factory;

    public FirstServiceTests(MyFactory<Program> factory)
    {
        this.factory = factory;
    }
    [Fact]
    public void GetUnaryMessage()
    {
        //Arange
        var client = factory.CreateGrpcClient();
        var expectedResponse = new Response() { Message = "messagefrom server" };
        //Act
        var actualResponse = client.Unary(new Request() { Content = "message" });
        //Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
}