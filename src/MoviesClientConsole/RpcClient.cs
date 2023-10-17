using Grpc.Core;
using Grpc.Net.Client;
using MoviesRpcClient.Protos;
using System.Diagnostics;

namespace MoviesClientConsole;

internal class RpcClient
{
  public static async Task Get()
  {
    var channel = GrpcChannel.ForAddress("https://localhost:7151");

    var client = new Movies.MoviesClient(channel);

    var timer = Stopwatch.StartNew();

    var request = new ByGenreRequest { Genre = "Comedy" };

    var list = await client.ByGenreAsync(request);

    timer.Stop();

    Console.WriteLine($"Retrieved {list.Movies.Count} movies in {timer.Elapsed}");
  }

  public static async Task GetStreaming()
  {
    var channel = GrpcChannel.ForAddress("https://localhost:7151");

    var client = new Movies.MoviesClient(channel);

    var timer = Stopwatch.StartNew();

    var request = new ByGenreRequest { Genre = "Comedy" };

    var stream = client.ByGenreStreaming(request);

    var list = new List<Movie>();

    await foreach (var movieList in stream.ResponseStream.ReadAllAsync())
    {
      list.AddRange(movieList.Movies);
    }

    timer.Stop();

    Console.WriteLine($"Retrieved {list.Count} movies in {timer.Elapsed}");
  }
}
