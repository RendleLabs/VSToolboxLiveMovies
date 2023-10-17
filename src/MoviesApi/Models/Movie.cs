namespace MoviesApi.Models;

public class Movie
{
  public required string Title { get; set; }
  public string? OriginalTitle { get; set; }
  public string? Overview { get; set; }
  public string? Homepage { get; set; }
  public float Popularity { get; set; }
  public DateTime ReleaseDate { get; set; }
  public long Revenue { get; set; }
  public int Runtime { get; set; }
  public required string Status { get; set; }
  public string? TagLine { get; set; }
  public float VoteAverage { get; set; }
  public int VoteCount { get; set; }
  public string[] Genres { get; set; } = [];

  public static Movie FromEntity(Movies.Data.Movie entity)
  {
    var model = new Movie
    {
      Title = entity.Title,
      OriginalTitle = entity.OriginalTitle,
      Overview = entity.Overview,
      Homepage = entity.Homepage,
      Popularity = entity.Popularity,
      ReleaseDate = entity.ReleaseDate,
      Revenue = entity.Revenue,
      Runtime = entity.Runtime,
      Status = entity.Status.ToString(),
      TagLine = entity.TagLine,
      VoteAverage = entity.VoteAverage,
      VoteCount = entity.VoteCount,
      Genres = entity.Genres.Select(g => g.Name).ToArray(),
    };

    return model;
  }
}
