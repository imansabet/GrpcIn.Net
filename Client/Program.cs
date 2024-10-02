// See https://aka.ms/new-console-template for more information
using Basics;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using static Grpc.Core.Metadata;

var retryPolicy = new MethodConfig
{
    Names = { MethodName.Default },
    RetryPolicy = new RetryPolicy()
    {
        MaxAttempts = 5,
        BackoffMultiplier =1,
        InitialBackoff = TimeSpan.FromSeconds(0.5),
        MaxBackoff = TimeSpan.FromSeconds(0.5),
        RetryableStatusCodes = { StatusCode.Internal }
    }
};
var hedging = new MethodConfig
{
    Names = { MethodName.Default },
    HedgingPolicy = new HedgingPolicy()
    {
        MaxAttempts = 5,
        NonFatalStatusCodes = { StatusCode.Internal },
        HedgingDelay = TimeSpan.FromSeconds(0.5),
    }
};


var options = new GrpcChannelOptions()
{
    ServiceConfig = new ServiceConfig()
    {
        MethodConfigs = { hedging }
    }
};
using var channel = GrpcChannel.ForAddress("https://localhost:7135", options);
var client = new FirstServiceDefinition.FirstServiceDefinitionClient(channel);
ServerStreaming(client);

var services = new ServiceCollection();


var option = new GrpcChannelOptions() { };


Unary(client);

//ClientStreaming(client);
//ServerStreaming(client);
//BiDirectionalStreaming(client);

Console.ReadLine();


void Unary(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    var metadata = new Metadata { { "grpc-accept-encoding", "gzip" } };
    var request = new Request() { Content = "Hello you !" };
    var response = client.Unary(request,deadline:DateTime.UtcNow.AddMilliseconds(3));
}
async void ClientStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    using var call = client.ClientStream();
    for(var i=0 ; i<1000 ; i++)
    {
        await call.RequestStream.WriteAsync(new Request() { Content = i.ToString() });
    }
    await call.RequestStream.CompleteAsync();
    Response response = await call;
    Console.WriteLine($"{response.Message}");

}
async void ServerStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
        var cancellationToken = new CancellationTokenSource();

        var metadata = new Metadata();
        //metadata.Add(new Entry("first-key", "first-value"));
        //metadata.Add(new Entry("second-key", "second-value"));
        

    try
    {
        using var streamingCall = client.ServerStream(new Request()
        { Content = "Hello!" }
          ,headers: metadata
        );

        await foreach (var response in streamingCall.ResponseStream.ReadAllAsync(cancellationToken.Token))
        {
            Console.WriteLine(response.Message);
            if (response.Message.Contains("2"))
            {
                cancellationToken.Cancel();
            }
        }
        //var myTrailers = streamingCall.GetTrailers();
        //var myValue = myTrailers.GetValue("a-trailer"); 
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {

    }
   

}
async void BiDirectionalStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    using (var call = client.BiDirectionalStream())
    {
        var request = new Request();
        for (var i = 0; i < 10; i++)
        {
            request.Content = i.ToString();
            Console.WriteLine(request.Content);
            await call.RequestStream.WriteAsync(request);
        }
        while(await call.ResponseStream.MoveNext())
        {
            var message = call.ResponseStream.Current;
            Console.WriteLine(message);
        }
        await call.RequestStream.CompleteAsync();
    }
}
