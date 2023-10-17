using Microsoft.EntityFrameworkCore;

namespace Movies.Data;

public class MovieContext : DbContext
{
  public MovieContext(DbContextOptions<MovieContext> options) : base(options)
  {
  }

  protected MovieContext()
  {
  }

  public DbSet<Movie> Movies { get; internal set; }
  public DbSet<MovieCollection> MovieCollections { get; internal set; }
  public DbSet<Genre> Genres { get; internal set; }
  public DbSet<ProductionCompany> ProductionCompanies { get; internal set; }
  public DbSet<ProductionCountry> ProductionCountries { get; internal set; }
  public DbSet<SpokenLanguage> SpokenLanguages { get; internal set; }
}

