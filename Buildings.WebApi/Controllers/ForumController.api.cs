using Buildings.Commands.Forum;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Data.Entities.Forum;
using Buildings.Infrastructure.Repositories;
using Buildings.Responses.Account;
using Buildings.Responses.Forum;
using Buildings.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
[Tags("Forum")]
public class ForumController(
    ILogger<SecureController> logger,
    ISecureRepository secureRepository,
    IAccountRepository accountRepository,
    IDbContextFactory<BuildingDbContext> dbContextFactory
) : ControllerBase
{
    [HttpGet("post/summary")]
    [EndpointSummary("获取帖子统计信息")]
    [ProducesResponseType<ForumSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostSummaryAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var total = await dbContext.BlogPosts.AsNoTracking()
            .CountAsync();
        var users = await dbContext.UserAccountInfos.AsNoTracking()
            .CountAsync();
        var hot = await dbContext.BlogPosts.AsNoTracking()
            .OrderBy(e => e.Views + e.Likes * 5)
            .Take(5)
            .Select(e => new HotPostSlug
            {
                PostId = e.Id,
                Title = e.Title,
                IsAi = e.IsAi,
                HeatScore = e.Views + e.Likes * 5
            })
            .ToArrayAsync();
        return Ok(new ForumSummaryResponse
        {
            TotalPosts = total,
            TotalUsers = users,
            HotPostSlugs = hot
        });
    }

    [HttpGet("post")]
    [EndpointSummary("获取帖子分页列表")]
    [ProducesResponseType<PostSlugInfo[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostListAsync([FromQuery] SplitPageForumCommand command)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var posts = await dbContext.BlogPosts.AsNoTracking()
            .OrderBy(e => e.Views + e.Likes * 5)
            .Take(5)
            .Select(e => new PostSlugInfo
            {
                Id = e.Id,
                Title = e.Title,
                Excerpt = e.Data.Substring(0, 128),
                Tag = e.Tag,
                Author = new AccountPublicInfoResponse
                {
                    UserId = e.AuthorId,
                    UserName = e.Author.UserName,
                    Profile = e.Author.Profile,
                    Location = e.Author.Location,
                    Gender = e.Author.Gender.Code,
                    Interest = e.Author.Interest,
                },
                Stats = new PostStats
                {
                    CommentsCount = e.Comments.Count,
                    Likes = e.Likes,
                    Views = e.Views,
                }
            })
            .ToArrayAsync();
        //TODO: GetData
        return Ok(posts);
    }

    [HttpGet("post/{postId:long}")]
    [EndpointSummary("获取帖子具体内容")]
    [EndpointDescription("Data是markdown文本,应当使用MDC组件渲染")]
    [ProducesResponseType<PostDataResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostDataAsync(
        [FromRoute] long postId
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var post = await dbContext.BlogPosts
            .FirstOrDefaultAsync(e => e.Id == postId && e.DeleteAt == null);
        if (post is null) return NotFound();
        var account = await accountRepository.GetAccountAsync(post.AuthorId);
        var comments = await dbContext.BlogComments.AsNoTracking()
            .Where(e => e.PostId == postId)
            .CountAsync();

        post.Views++;
        dbContext.BlogPosts.Update(post);
        await dbContext.SaveChangesAsync();
        //TODO: GetData
        return Ok(new PostDataResponse
        {
            Id = postId,
            Title = post.Title,
            Tag = post.Tag,
            Data = post.Data,
            Author = account is null
                ? new AccountPublicInfoResponse
                {
                    UserId = 0,
                    UserName = "Not Found",
                    Profile = "",
                    Location = ""
                }
                : new AccountPublicInfoResponse
                {
                    UserId = account.UserId,
                    UserName = account.UserName,
                    Profile = account.Profile,
                    Gender = account.Gender.Code,
                    Location = account.Location,
                    Interest = account.Interest
                },
            Stats = new PostStats
            {
                CommentsCount = comments,
                Likes = post.Likes,
                Views = post.Views,
            }
        });
    }

    [HttpGet("post/{postId:long}/profile")]
    [EndpointSummary("获取帖子简介内容")]
    [ProducesResponseType<PostSlugInfo>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostSlugAsync(
        [FromRoute] long postId
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var post = await dbContext.BlogPosts
            .Where(e => e.Id == postId && e.DeleteAt == null)
            .Select(e => new PostSlugInfo
            {
                Id = e.Id,
                Title = e.Title,
                Excerpt = e.Data.Substring(0, 128),
                Tag = e.Tag,
                Author = new AccountPublicInfoResponse
                {
                    UserId = e.AuthorId,
                    UserName = e.Author.UserName,
                    Profile = e.Author.Profile,
                    Location = e.Author.Location,
                    Gender = e.Author.Gender.Code,
                    Interest = e.Author.Interest,
                },
                Stats = new PostStats
                {
                    CommentsCount = e.Comments.Count,
                    Likes = e.Likes,
                    Views = e.Views,
                }
            })
            .FirstOrDefaultAsync();
        if (post is null) return NotFound();
        //TODO: GetData
        return Ok(post);
    }

    [Authorize]
    [HttpPost("post/{postId:long}/comment")]
    [EndpointSummary("帖子发送评论")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PublishCommentAsync(
        [FromRoute] long postId,
        [FromBody] PublishCommentCommand command,
        [FromServices] IIdGenerator<long> idGenerator
    )
    {
        var account = await accountRepository.GetAccountAsync(command.UserId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        if (!await dbContext.BlogPosts.AsNoTracking()
                .AnyAsync(e => e.Id == postId && e.DeleteAt == null))
            return NotFound("Post doesn't exist.");
        var commentId = idGenerator.NextId() >> 8;
        var comment = new BlogComment
        {
            Id = commentId,
            PostId = postId,
            AuthorId = command.UserId,
            Data = command.Content,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        await dbContext.BlogComments.AddAsync(comment);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [HttpPost("publish")]
    [EndpointSummary("发布帖子")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PublishNewPostAsync(
        [FromBody] PublishPostCommand command,
        [FromServices] IIdGenerator<long> idGenerator
    )
    {
        var account = await accountRepository.GetAccountAsync(command.AuthorId);
        if (account is null)
            return NotFound("User doesn't exist.");
        var postId = idGenerator.NextId() >> 8;
        var post = new BlogPost
        {
            Id = postId,
            Title = command.Title,
            Tag = command.Tag,
            Data = command.Data,
            CreatedAt = DateTimeOffset.UtcNow,
            AuthorId = command.AuthorId,
        };
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.BlogPosts.Add(post);
        await dbContext.SaveChangesAsync();
        return Created();
    }
}