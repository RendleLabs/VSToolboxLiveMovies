using Import;
using Microsoft.EntityFrameworkCore;
using Movies.Data;

var csv = Path.GetFullPath(args[0]);
var dbPath = Path.GetFullPath(args[1]);

var options = new DbContextOptionsBuilder<MovieContext>()
  .UseSqlite($"Data Source={dbPath}")
  .Options;

var context = new MovieContext(options);

await context.Database.MigrateAsync();

var loader = new MovieLoader(context);
await loader.Load(csv);

