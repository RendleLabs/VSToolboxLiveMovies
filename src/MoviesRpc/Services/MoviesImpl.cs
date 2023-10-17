using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using MoviesRpc.Protos;

namespace MoviesRpc.Services
{
  public class MoviesImpl : Protos.Movies.MoviesBase
  {
    private readonly MovieContext _context;

    public MoviesImpl(MovieContext context)
    {
      _context = context;
    }

    public override async Task<MovieList> ByGenre(ByGenreRequest request, ServerCallContext context)
    {
      var skip = ((request.Page ?? 1) - 1) * 100;

      var movies = await _context.Movies
        .Include(m => m.Genres)
        .Where(m => m.Genres.Any(g => g.Name == request.Genre))
        .OrderBy(m => m.Title)
        .Skip(skip)
        .Take(100)
        .ToListAsync();

      var response = new MovieList
      {
        Movies =
        {
          movies.Select(m => Protos.Movie.FromEntity(m))
        }
      };

      return response;
    }

    public override async Task ByGenreStreaming(ByGenreRequest request, IServerStreamWriter<MovieList> responseStream, ServerCallContext context)
    {
      var movies = _context.Movies
        .Include(m => m.Genres)
        .Where(m => m.Genres.Any(g => g.Name == request.Genre))
        .AsAsyncEnumerable();

      var list = new MovieList();
      await foreach (var movie in movies.WithCancellation(context.CancellationToken))
      {
        list.Movies.Add(Protos.Movie.FromEntity(movie));
        if (list.Movies.Count > 100)
        {
          await responseStream.WriteAsync(list);
          list = new MovieList();
        }
      }
      if (list.Movies.Count > 0)
      {
        await responseStream.WriteAsync(list);
      }
    }
  }
}
