namespace Movies.Data;

public class SpokenLanguage
{
  public int Id { get; set; }
  public string IsoCode { get; set; }
  public string Name { get; set; }
  public List<Movie> Movies { get; } = new();
}
