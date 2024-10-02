using Grpc.Net.Compression;
using GrpcIn.Net.Interceptors;
using GrpcIn.Net.Services;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(option =>
{
    option.Interceptors.Add<ServerLoggingInterceptor>();
    option.ResponseCompressionAlgorithm = "gzip";
    option.ResponseCompressionLevel = CompressionLevel.SmallestSize;
    option.CompressionProviders = new List<ICompressionProvider>()
    {
        new GzipCompressionProvider(CompressionLevel.SmallestSize)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();

app.MapGrpcService<FirstService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
