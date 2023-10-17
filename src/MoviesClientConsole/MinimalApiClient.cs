using MoviesApi.Models;
using System.Diagnostics;
using System.Net.Http.Json;

namespace MoviesClientConsole;

internal class MinimalApiClient
{
  public static async Task Get()
  {
    using var http = new HttpClient();

    var timer = Stopwatch.StartNew();

    using var response = await http.GetAsync("https://localhost:7232/movies/genre/Comedy");

    var movies = await response.Content.ReadFromJsonAsync<Movie[]>();

    timer.Stop();

    movies ??= [];

    Console.WriteLine($"Retrieved {movies.Length} movies in {timer.Elapsed}");
  }

  public static async Task GetAll()
  {
    using var http = new HttpClient();
    var list = new List<Movie>();

    var timer = Stopwatch.StartNew();
    int page = 1;

    using var response = await http.GetAsync($"https://localhost:7232/movies/genre/Comedy?page={page}");

    var movies = await response.Content.ReadFromJsonAsync<Movie[]>();

    while (movies?.Length == 100)
    {
      list.AddRange(movies);
      ++page;
      using var response2 = await http.GetAsync($"https://localhost:7232/movies/genre/Comedy?page={page}");
      movies = await response2.Content.ReadFromJsonAsync<Movie[]>();
    }

    if (movies is { Length: > 0 })
    {
      list.AddRange(movies);
    }

    timer.Stop();

    Console.WriteLine($"Retrieved {list.Count} movies in {timer.Elapsed}");
  }
}
