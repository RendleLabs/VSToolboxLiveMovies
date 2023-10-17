using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorMovies;
using Grpc.Net.Client.Web;
using Grpc.Net.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton(services =>
{
  var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
  var baseUri = "https://localhost:7151";
  var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions {HttpClient = httpClient});
  return new MoviesRpcClient.Protos.Movies.MoviesClient(channel);
});

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
