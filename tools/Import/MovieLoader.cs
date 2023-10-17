using Csv;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Import;

internal class MovieLoader
{
  private const int Adult = 0;
  private const int BelongsToCollection = 1;
  private const int Budget = 2;
  private const int Genres = 3;
  private const int Homepage = 4;
  private const int Id = 5;
  private const int ImdbId = 6;
  private const int OriginalLanguage = 7;
  private const int OriginalTitle = 8;
  private const int Overview = 9;
  private const int Popularity = 10;
  private const int PosterPath = 11;
  private const int ProductionCompanies = 12;
  private const int ProductionCountries = 13;
  private const int ReleaseDate = 14;
  private const int Revenue = 15;
  private const int Runtime = 16;
  private const int SpokenLanguages = 17;
  private const int Status = 18;
  private const int Tagline = 19;
  private const int Title = 20;
  private const int Video = 21;
  private const int VoteAverage = 22;
  private const int VoteCount = 23;

  private readonly MovieContext _context;

  public MovieLoader(MovieContext context)
  {
    _context = context;
  }

  public async Task Load(string path)
  {
    using var stream = File.OpenRead(path);

    var options = new CsvOptions
    {
      AllowNewLineInEnclosedFieldValues = true,
    };

    await foreach (var line in CsvReader.ReadFromStreamAsync(stream, options))
    {
      if (!line[Adult].Equals("false", StringComparison.OrdinalIgnoreCase)) continue;

      await Console.Out.WriteLineAsync(line[Title]);

      await AddMovie(line);
    }
  }

  private async Task AddMovie(ICsvLine line)
  {
    int id = ParseIntOrDefault(line[Id]);

    if (await _context.Movies.CountAsync(x => x.Id == id) > 0) return;

    try
    {
      var movie = new Movie
      {
        Id = id,
        Budget = ParseIntOrDefault(line[Budget]),
        Homepage = line[Homepage],
        OriginalLanguage = line[OriginalLanguage],
        OriginalTitle = line[OriginalTitle],
        Overview = line[Overview],
        Popularity = ParseFloatOrDefault(line[Popularity]),
        ReleaseDate = ParseDateOrDefault(line[ReleaseDate]),
        Revenue = ParseLongOrDefault(line[Revenue]),
        Runtime = ParseIntOrDefault(line[Runtime]),
        Status = ParseStatus(line[Status]),
        TagLine = line[Tagline],
        Title = line[Title],
        VoteAverage = ParseFloatOrDefault(line[VoteAverage]),
        VoteCount = ParseIntOrDefault(line[VoteCount]),
      };

      if (await GetGenres(line[Genres]) is { Count: > 0 } genres)
      {
        movie.Genres.AddRange(genres);
      }

      if (await GetProductionCompanies(line[ProductionCompanies]) is { Count: > 0 } companies)
      {
        movie.ProductionCompanies.AddRange(companies);
      }

      if (await GetProductionCountries(line[ProductionCountries]) is { Count: > 0 } countries)
      {
        movie.ProductionCountries.AddRange(countries);
      }

      if (await GetSpokenLanguages(line[SpokenLanguages]) is {Count: > 0 } languages)
      {
        movie.SpokenLanguages.AddRange(languages);
      }

      _context.Movies.Add(movie);
      await _context.SaveChangesAsync();
    }
    catch
    {
    }
  }

  private static int ParseIntOrDefault(string str) => int.TryParse(str, out var value) ? value : 0;
  private static long ParseLongOrDefault(string str) => long.TryParse(str, out var value) ? value : 0;
  private static float ParseFloatOrDefault(string str) => float.TryParse(str, out var value) ? value : 0;
  private static DateTime ParseDateOrDefault(string str) => DateTime.TryParseExact(str, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var date) ? date : default;
  private static MovieStatus ParseStatus(string str) => str switch
  {
    "Planned" => MovieStatus.Planned,
    "In Production" => MovieStatus.InProduction,
    "Post Production" => MovieStatus.PostProduction,
    "Released" => MovieStatus.Released,
    "Canceled" => MovieStatus.Cancelled,
    "Rumored" => MovieStatus.Rumored,
    _ => MovieStatus.None,
  };

  private async Task<MovieCollection> GetCollection(string json)
  {
    var doc = JsonDocument.Parse(json);
    int id = doc.RootElement.GetProperty("id").GetInt32();
    var collection = await _context.MovieCollections.FirstOrDefaultAsync(x => x.Id == id);
    if (collection is null)
    {
      collection = new MovieCollection
      {
        Id = id,
        Name = doc.RootElement.GetProperty("Name").GetString()!,
      };
      _context.MovieCollections.Add(collection);
    }
    return collection;
  }

  private async Task<List<Genre>> GetGenres(string json)
  {
    if (string.IsNullOrWhiteSpace(json)) return [];

    var list = new List<Genre>();

    var doc = JsonDocument.Parse(json.Replace('\'', '"'));
    foreach (var e in doc.RootElement.EnumerateArray())
    {
      var id = e.GetProperty("id").GetInt32();
      var genre = await _context.Genres.FindAsync(id);
      if (genre is null)
      {
        genre = new Genre
        {
          Id = id,
          Name = e.GetProperty("name").GetString()!
        };
        _context.Genres.Add(genre);
      }
      list.Add(genre);
    }
    return list;
  }

  private async Task<List<ProductionCompany>> GetProductionCompanies(string json)
  {
    if (string.IsNullOrWhiteSpace(json)) return [];

    var list = new List<ProductionCompany>();

    var doc = JsonDocument.Parse(json.Replace('\'', '"'));
    foreach (var e in doc.RootElement.EnumerateArray())
    {
      var id = e.GetProperty("id").GetInt32();
      var company = await _context.ProductionCompanies.FindAsync(id);
      if (company is null)
      {
        company = new ProductionCompany
        {
          Id = id,
          Name = e.GetProperty("name").GetString()!
        };
        _context.ProductionCompanies.Add(company);
      }
      list.Add(company);
    }
    return list;
  }

  private async Task<List<ProductionCountry>> GetProductionCountries(string json)
  {
    if (string.IsNullOrWhiteSpace(json)) return [];

    var list = new List<ProductionCountry>();

    var doc = JsonDocument.Parse(json.Replace('\'', '"'));
    foreach (var e in doc.RootElement.EnumerateArray())
    {
      var iso = e.GetProperty("iso_3166_1").GetString();
      if (iso is not { Length: > 0 }) continue;

      var country = await _context.ProductionCountries.FirstOrDefaultAsync(x => x.IsoCode == iso);
      if (country is null)
      {
        country = new ProductionCountry
        {
          IsoCode = iso,
          Name = e.GetProperty("name").GetString()!
        };
        _context.ProductionCountries.Add(country);
      }
      list.Add(country);
    }
    return list;
  }

  private async Task<List<SpokenLanguage>> GetSpokenLanguages(string json)
  {
    if (string.IsNullOrWhiteSpace(json)) return [];

    var list = new List<SpokenLanguage>();

    var doc = JsonDocument.Parse(json.Replace('\'', '"'));
    foreach (var e in doc.RootElement.EnumerateArray())
    {
      var iso = e.GetProperty("iso_639_1").GetString();
      if (iso is not { Length: > 0 }) continue;

      var language = await _context.SpokenLanguages.FirstOrDefaultAsync(x => x.IsoCode == iso);
      if (language is null)
      {
        language = new SpokenLanguage
        {
          IsoCode = iso,
          Name = e.GetProperty("name").GetString()!
        };
        _context.SpokenLanguages.Add(language);
      }
      list.Add(language);
    }
    return list;
  }
}
