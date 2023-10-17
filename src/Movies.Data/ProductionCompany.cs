namespace Movies.Data;

public class ProductionCompany
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public List<Movie> Movies { get; } = new();
}
