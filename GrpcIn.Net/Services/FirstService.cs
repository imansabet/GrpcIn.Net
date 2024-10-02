﻿using Basics;
using Grpc.Core;

namespace GrpcIn.Net.Services;

public class FirstService : FirstServiceDefinition.FirstServiceDefinitionBase
{
    public override Task<Response> Unary(Request request, ServerCallContext context)
    {
        context.WriteOptions = new WriteOptions(WriteFlags.NoCompress);
        var response = new Response() { Message = request.Content + $"from Server{context.Host} " };
        return Task.FromResult(response);
    }
    public override async Task<Response> ClientStream(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
    {
        var response = new Response() { Message = "I Got " };
        while( await requestStream.MoveNext())
        {
            var requestPayload = requestStream.Current;
            Console.WriteLine(requestPayload);
            response.Message = requestPayload.ToString();
        }
        return response;
    }
    public override async Task ServerStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        var headerFirst = context.RequestHeaders.Get("first-key");

        var myTrailer = new Metadata.Entry("a-trailer", "a-trailer-value");
        context.ResponseTrailers.Add(myTrailer);


        for(var i= 0;i< 100; i++)
        {
            if (context.CancellationToken.IsCancellationRequested) return; 

            var response = new Response() { Message =  i.ToString() };
            await responseStream.WriteAsync(response);
        }
    }

    public override async Task BiDirectionalStream(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        Response response = new Response() { Message = "" };
        while(await requestStream.MoveNext())
        {
            var requestPayload = requestStream.Current;
            response.Message = requestPayload.ToString();
            await responseStream.WriteAsync(response);
        }
    }
}
