namespace Movies.Data;

public class MovieCollection
{
  public int Id { get; set; }
  public string Name { get; set; }
  public List<Movie> Movies { get; } = new();
}
