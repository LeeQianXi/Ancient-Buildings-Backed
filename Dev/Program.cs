using System.Text.Json;
using System.Text.RegularExpressions;
using Buildings.Utils;
using Dev.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dev;

internal static partial class Program
{
    [GeneratedRegex(@"[\S]+/[\S]+/[\S]+/(?<path>[\S]+).md")]
    private static partial Regex NameRegex { get; }

    public static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
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

        var sqlite = serviceProvider.GetRequiredService<ContentDbContent>();
        var pg = serviceProvider.GetRequiredService<BuildingDbContext>();

        var data = Parse(await sqlite.ContentBuildings.AsNoTracking()
            .ToArrayAsync());

        await pg.BuildingArticleDatas.AddRangeAsync(data);
        await pg.SaveChangesAsync();
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
                Path = NameRegex.Match(contentBuilding.Id).Groups["path"].Value,
                Hash = contentBuilding.Hash,
                Data = JsonDocument.Parse(contentBuilding.Body),
                Description = contentBuilding.Description,
                DisplayName = contentBuilding.Name,
                Provinces = [contentBuilding.Provinces.Replace("[\"", "").Replace("\"]", "")],
                Categories = [contentBuilding.Categories.Replace("[\"", "").Replace("\"]", "")],
                Dynasties = [contentBuilding.Dynasties.Replace("[\"", "").Replace("\"]", "")]
            };
    }
}