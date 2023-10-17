using Google.Protobuf.WellKnownTypes;

namespace MoviesRpc.Protos
{
  public partial class Movie
  {
    public static Movie FromEntity(global::Movies.Data.Movie entity)
    {
      return new Movie
      {
        Id = entity.Id,
        Title = entity.Title,
        OriginalTitle = entity.OriginalTitle,
        Overview = entity.Overview,
        Popularity = entity.Popularity,
        ReleaseDate = entity.ReleaseDate.ToUniversalTime().ToTimestamp(),
        Revenue = entity.Revenue,
        Runtime = entity.Runtime,
        Status = (Protos.MovieStatus)entity.Status,
        TagLine = entity.TagLine,
        VoteAverage = entity.VoteAverage,
        VoteCount = entity.VoteCount,
        Genres =
        {
          entity.Genres.Select(g => g.Name)
        }
      };
    }
  }
}
