using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Movies.Data;

public class MovieContextFactory : IDesignTimeDbContextFactory<MovieContext>
{
  public MovieContext CreateDbContext(string[] args)
  {
    var options = new DbContextOptionsBuilder<MovieContext>()
      .UseSqlite("Data Source=movies.db")
      .Options;

    return new MovieContext(options);
  }
}

