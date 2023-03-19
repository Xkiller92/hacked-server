using hacked_instance_handler.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.

builder.Services.AddGrpc();
const string corsPolicy = "_corsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy  =>
        {
            policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true).SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding", "X-Grpc-Web", "User-Agent");
        });
});

var app = builder.Build();

//Configure the HTTP request pipeline.
app.UseRouting();
app.Use(
    async (context, next) =>
	{
		context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5274");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "*");  
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "false");              
		await next();
	}
);
app.UseCors(corsPolicy);
app.UseGrpcWeb();
app.MapGrpcService<HackathonService>().RequireCors(corsPolicy).EnableGrpcWeb();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909").RequireCors(corsPolicy).EnableGrpcWeb();

app.Run();