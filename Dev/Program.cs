using System.Text.Json;
using System.Text.RegularExpressions;
using Buildings.Utils;
using Dev.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Dev;

internal static partial class Program
{
    private static readonly IServiceProvider serviceProvider;

    static Program()
    {
        serviceProvider = new ServiceCollection()
            .AddDbContext<ContentDbContent>(options =>
            {
                options.UseSqlite(
                    "Data Source=/home/qianxi/source/repos/Web/Buildings/.data/content/contents.sqlite");
            })
            .AddDbContext<BuildingDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Database=buildings;Username=admin;Password=123456");
            })
            .BuildServiceProvider();
    }

    [GeneratedRegex(@"[\S]+/[\S]+/[\S]+/(?<path>[\S]+).md")]
    private static partial Regex NameRegex { get; }

    public static async Task Main(string[] args)
    {
        await Import();
    }

    private static async Task Import()
    {
        var db = serviceProvider.GetRequiredService<BuildingDbContext>();
        foreach (var file in Directory.GetFiles("/home/qianxi/source/repos/Web/Buildings.WebApi/tmp/output"))
        {
            var obj = JObject.Parse(await File.ReadAllTextAsync(file));
            foreach (var pair in obj)
            {
                var id = long.Parse(pair.Key);
                var building = db.BuildingArticleDatas.First(e => e.ArticleId == id);
                building.Dynasties = ((JArray)pair.Value?["Dynasties"]).Select(e => e.ToString()).ToArray();
                db.BuildingArticleDatas.Update(building);
                Console.WriteLine($"{pair.Value["Title"]} - {pair.Value["Dynasties"]}");
            }
        }

        await db.SaveChangesAsync();
    }
    private static async Task Export()
    {
        const int split = 400;
        var db = serviceProvider.GetRequiredService<BuildingDbContext>();
        var total = db.BuildingArticleDatas.Count();
        for (var i = 0; i < total; i += split)
        {
            var buildings = db.BuildingArticleDatas.OrderBy(e => e.ArticleId)
                .Skip(i)
                .Take(split)
                .Select(e => new
                {
                    e.ArticleId,
                    e.Dynasties
                })
                .ToArray();
            var obj = new JObject();
            foreach (var building in buildings)
            {
                var pair = new JObject
                {
                    ["Dynasties"] = JArray.FromObject(building.Dynasties),
                };
                obj[building.ArticleId.ToString()] = pair;
            }

            await File.WriteAllTextAsync(
                $"/home/qianxi/source/repos/Web/Buildings.WebApi/tmp/building-split{i}-{i + split}.json",
                obj.ToString());
        }
    }

    private static IEnumerable<BuildingArticleData> Parse(IEnumerable<ContentBuilding> data)
    {
        var generator = new SnowflakeIdGenerator();
        foreach (var contentBuilding in data)
            yield return new BuildingArticleData
            {
                ArticleId = generator.NextId(),
                Title = contentBuilding.Title,
                SubTitle = contentBuilding.Subtitle,
                Seo = JsonDocument.Parse(contentBuilding.Seo),
                Image = contentBuilding.Img,
                Path = NameRegex.Match(contentBuilding.Id)
                    .Groups["path"].Value,
                Hash = contentBuilding.Hash,
                Data = JsonDocument.Parse(contentBuilding.Body),
                Description = contentBuilding.Description,
                DisplayName = contentBuilding.Name,
                Provinces =
                [
                    contentBuilding.Provinces.Replace("[\"",
                            "")
                        .Replace("\"]",
                            "")
                ],
                Categories =
                [
                    contentBuilding.Categories.Replace("[\"",
                            "")
                        .Replace("\"]",
                            "")
                ],
                Dynasties =
                [
                    contentBuilding.Dynasties.Replace("[\"",
                            "")
                        .Replace("\"]",
                            "")
                ],
                IsRed = false
            };
    }

    private static async Task Migrations()
    {
        var sqlite = serviceProvider.GetRequiredService<ContentDbContent>();
        var pg = serviceProvider.GetRequiredService<BuildingDbContext>();

        var data = Parse(await sqlite.ContentBuildings.AsNoTracking()
            .ToArrayAsync());

        await pg.BuildingArticleDatas.AddRangeAsync(data);
        await pg.SaveChangesAsync();
    }
}