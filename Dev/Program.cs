// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Data.Entities;
using Buildings.Utils;
using Dev;
using Dev.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddDbContext<PgContent>(options =>
    {
        options.UseSqlite("Data Source=/home/qianxi/source/repos/Web/Buildings/.data/content/contents.sqlite");
    })
    .AddDbContext<BuildingDbContext>(options =>
    {
        options.UseNpgsql("Host=localhost;Database=buildings;Username=admin;Password=123456");
    })
    .BuildServiceProvider();

var sqlite = serviceProvider.GetRequiredService<PgContent>();
var pg = serviceProvider.GetRequiredService<BuildingDbContext>();

var data = Parse(await sqlite.ContentBuildings.AsNoTracking()
    .ToArrayAsync());

await pg.BuildingArticleDatas.AddRangeAsync(data);
await pg.SaveChangesAsync();

IEnumerable<BuildingArticleData> Parse(IEnumerable<ContentBuilding> data)
{
    var generator = new SnowflakeIdGenerator();
    var regex = new Regex(@"[\S]+/[\S]+/[\S]+/(?<path>[\S]+).md");
    foreach (var contentBuilding in data)
        yield return new BuildingArticleData
        {
            ArticleId = generator.NextId(),
            Title = contentBuilding.Title,
            SubTitle = contentBuilding.Subtitle,
            Seo = contentBuilding.Seo,
            Image = contentBuilding.Img,
            Path = regex.Match(contentBuilding.Id).Groups["path"].Value,
            Hash = contentBuilding.Hash,
            Data = contentBuilding.Body,
            Description = contentBuilding.Description,
            DisplayName = contentBuilding.Name,
            Provinces = [contentBuilding.Provinces.Replace("[\"", "").Replace("\"]", "")],
            Categories = [contentBuilding.Categories.Replace("[\"", "").Replace("\"]", "")],
            Dynasties = [contentBuilding.Dynasties.Replace("[\"", "").Replace("\"]", "")]
        };
}