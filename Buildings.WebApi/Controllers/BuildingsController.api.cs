using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Buildings.Commands.Buildings;
using Buildings.Infrastructure.Data;
using Buildings.Responses.Buildings;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
[Tags("Buildings")]
public class BuildingsController(
    ILogger<BuildingsController> logger,
    IDbContextFactory<BuildingDbContext> dbContextFactory
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<BuildingSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummeryAsync()
    {
        if (InternalCache.TryGet(nameof(GetSummeryAsync), out var data)) return Ok(data);
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var total = await dbContext.BuildingArticleData.AsNoTracking().CountAsync();
        var temp = await dbContext.BuildingArticleData.AsNoTracking()
            .Select(d => new { d.Categories, d.Dynasties, d.Provinces })
            .ToArrayAsync();
        HashSet<string> c = [], d = [], p = [];
        foreach (var source in temp)
        {
            c.UnionWith(source.Categories);
            d.UnionWith(source.Dynasties);
            p.UnionWith(source.Provinces);
        }

        var result = new BuildingSummaryResponse
        {
            Total = total,
            Categories = c.WrapAsPair(),
            Dynasties = d.WrapAsPair(),
            Provinces = p.WrapAsPair()
        };
        InternalCache.Cache(nameof(GetSummeryAsync), result);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<SplitPageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSplitPageAsync(
        [FromQuery] SplitPageCommand command,
        [FromServices] IValidator<SplitPageCommand> validator
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        var articles = dbContext.BuildingArticleData.AsNoTracking()
            .Where(e =>
                EF.Functions.JsonContains(e.Categories, command.Categories) &&
                EF.Functions.JsonContains(e.Provinces, command.Provinces) &&
                EF.Functions.JsonContains(e.Dynasties, command.Dynasties));
        articles = command.Searches
            .Aggregate(articles, (current, search) =>
                current.Where(e =>
                    e.DisplayName.Contains(search) ||
                    e.Description.Contains(search)));
        var limit = await articles
            .Skip((command.Page - 1) * command.PageSize)
            .Take(command.PageSize)
            .Select(e => new BuildingSlug
            {
                Name = e.DisplayName,
                Img = e.Image,
                Desc = e.Description,
                Dynasties = e.Dynasties,
                Categories = e.Categories,
                Provinces = e.Provinces,
                Hash = e.Hash
            })
            .ToArrayAsync();
        return Ok(new SplitPageResponse
        {
            Items = limit,
            Total = await articles.CountAsync()
        });
    }

    /// <summary>
    ///     获取hash路径下的文章
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    [HttpGet("{hash}")]
    [ProducesResponseType<BuildingArticle>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticleByHashAsync([FromRoute] string hash)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var article = await dbContext.BuildingArticleData.AsNoTracking()
            .Where(d => d.Hash == hash)
            .Select(d => new BuildingArticle
            {
                Id = d.Path,
                Body = d.Data,
                Categories = d.Categories,
                Desc = d.Description,
                Dynasties = d.Dynasties,
                Img = d.Image,
                Name = d.DisplayName,
                Provinces = d.Provinces,
                Subtitle = d.SubTitle,
                Title = d.Title
            })
            .FirstOrDefaultAsync();
        if (article is null) return NotFound("Article not found");
        return Ok(article);
    }

    /// <summary>
    ///     重定向到文章
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticleByNameAsync([FromRoute] string name)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var article = await dbContext.BuildingArticleData.AsNoTracking()
            .Where(d => d.Path == name)
            .Select(d => new { d.Hash, d.Path })
            .FirstOrDefaultAsync();
        if (article is null) return NotFound("Article not found");
        return RedirectToAction("GetArticleByHash", "Buildings", new { article.Hash });
    }

    private static class InternalCache
    {
        private static readonly ConcurrentDictionary<string, Entry> _cache = new();

        public static void Cache(string key, object data, TimeSpan expiration)
        {
            _cache.AddOrUpdate(key,
                key => new Entry(data, DateTimeOffset.UtcNow + expiration),
                (key, old) =>
                {
                    old.Data = data;
                    old.Time = DateTimeOffset.UtcNow + expiration;
                    return old;
                }
            );
        }

        public static void Cache(string key, object data)
        {
            Cache(key, data, TimeSpan.FromMinutes(15));
        }

        public static bool TryGet(string ket, [NotNullWhen(true)] out object? data)
        {
            data = null;
            if (!_cache.TryGetValue(ket, out var entry)) return false;
            if (entry.Time < DateTimeOffset.UtcNow) return false;
            data = entry.Data;
            return true;
        }

        private record Entry(object Data, DateTimeOffset Time)
        {
            public object Data { get; set; } = Data;
            public DateTimeOffset Time { get; set; } = Time;
        }
    }
}