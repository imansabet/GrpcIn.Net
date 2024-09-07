using Basics;
using Grpc.Core;

namespace GrpcIn.Net.Services;

public class FirstService : FirstServiceDefinition.FirstServiceDefinitionBase
{
    public override Task<Response> Unary(Request request, ServerCallContext context)
    {
        var response = new Response() { Message = request.Content + "from Server " };
        return Task.FromResult(response);
    }
}
