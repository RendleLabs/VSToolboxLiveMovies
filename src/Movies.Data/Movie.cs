namespace Movies.Data;

public class Movie
{
  public int Id { get; set; }
  public required string Title { get; set; }
  public string? OriginalTitle { get; set; }
  public string? Overview { get; set; }
  public string? Homepage { get; set; }
  public float Popularity { get; set; }
  public DateTime ReleaseDate { get; set; }
  public long Revenue { get; set; }
  public int Runtime { get; set; }
  public MovieStatus Status { get; set; }
  public string? TagLine { get; set; }
  public float VoteAverage { get; set; }
  public int VoteCount { get; set; }
  public MovieCollection? Collection { get; set; }
  public List<Genre> Genres { get; } = new();
  public List<ProductionCompany> ProductionCompanies { get; } = new();
  public List<ProductionCountry> ProductionCountries { get; } = new();
  public List<SpokenLanguage> SpokenLanguages { get; } = new();
  public int Budget { get; set; }
  public string OriginalLanguage { get; set; }
}
